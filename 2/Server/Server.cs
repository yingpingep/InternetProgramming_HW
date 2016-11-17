using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Packet;

namespace Server
{
    class Server
    {
        static void Main(string[] args)
        {
            int listenPort = Convert.ToInt32(args[0]);
            int transPacketNumber = Convert.ToInt32(args[1]);
            // int listenPort = 3353;
            // int transPacketNumber = 12;
            IPEndPoint listenOn = new IPEndPoint(IPAddress.Any, listenPort);

            Socket udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            udpSocket.Bind(listenOn);

            byte[] buffer = new byte[1024];
            Random random = new Random();
            int sequenceNum = random.Next() % 200;            
            EndPoint remoteEP = Handshake(udpSocket, ref sequenceNum, buffer);

            bool ttt = Transfer(udpSocket, remoteEP, ref sequenceNum, transPacketNumber, buffer);            
        }
        
        static EndPoint Handshake(Socket udpSocket, ref int sequenceNum, byte[] buffer)
        {
            EndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);

            int len = udpSocket.ReceiveFrom(buffer, ref remoteEP);
            PacketFormate packet = Method.ByteArrayToPacket(buffer, len);
            Console.WriteLine("Start three-way handshake");          

            if (packet.syn)
            {                
                Array.Clear(buffer, 0, buffer.Length);
                packet = new PacketFormate(false, true, false, sequenceNum++, packet.sequenceNumber + 1, 0);            
                buffer = Method.PacketToByteArray(packet);                
                udpSocket.SendTo(buffer, remoteEP);                
            }

            Array.Clear(buffer, 0, buffer.Length);
            len = udpSocket.ReceiveFrom(buffer, ref remoteEP);            
            packet = Method.ByteArrayToPacket(buffer, len);

            // if (packet.ackNumber == ++startNumber)           
            return remoteEP;
        }

        static bool Transfer(Socket udpSocket, EndPoint remoteEP, ref int sequenceNum, int transPacketNumber, byte[] buffer)
        {
            bool[] reciveAck = new bool[transPacketNumber];
            PacketFormate packet;
            int backStart = sequenceNum;

            for (int i = 0, l = 0; i < 4; i++)
            {
                packet = new PacketFormate(false, false, false, sequenceNum, 0, 0);
                byte[] temp = Method.PacketToByteArray(packet);
                Array.Copy(temp, 0, buffer, l, temp.Length);
                l = l + temp.Length;
            }

            byte[] reBuffer = new byte[1024];

            while (true)
            {
                Array.Clear(reBuffer, 0, reBuffer.Length);
                int ackNumRecive = ReciveAck(udpSocket, remoteEP, reBuffer);
                if (reciveAck[ackNumRecive - 1])
                {
                    // TODO send next packet sequence number += 1

                    if (reciveAck[transPacketNumber - 1])
                    {
                        return true;
                    }            
                }
                else
                {
                    // TODO Re-transfer method
                }
            }            
        }

        static int ReciveAck(Socket udpSocket, EndPoint remoteEP, byte[] buffer)
        {
            int len = udpSocket.ReceiveFrom(buffer, ref remoteEP);
            return Method.ByteArrayToPacket(buffer, len).ackNumber;
        }
    }
}
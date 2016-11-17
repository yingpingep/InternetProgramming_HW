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

namespace Client
{
    class Client
    {
        static void Main(string[] args)
        {
            IPEndPoint serverIP = new IPEndPoint(IPAddress.Parse(args[0]), Convert.ToInt32(args[1]));
            // IPEndPoint serverIP = new IPEndPoint(IPAddress.Parse("140.118.138.236"), 3353);
            Socket udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
       
            Random random = new Random();
            int startNumber = random.Next() % 200;    
            byte[] buffer = new byte[1024];
            int packetNum = Handshake(udpSocket, serverIP, ref startNumber,buffer);

            // TODO Receive
            bool t = Receive(udpSocket, serverIP, packetNum, buffer);
        }

        static int Handshake(Socket udpSocket, IPEndPoint serverIP, ref int sequenceNum, byte[] buffer)
        {
            PacketFormate packet = new PacketFormate(true, false, false, sequenceNum++, 0, 4456789);
            buffer = Method.PacketToByteArray(packet);
            udpSocket.SendTo(buffer, serverIP);            

            Array.Clear(buffer, 0, buffer.Length);
            EndPoint remoteEP = serverIP;
            int len = udpSocket.ReceiveFrom(buffer, ref remoteEP);
            packet = Method.ByteArrayToPacket(buffer, len);
            int packetNum = packet.data; 

            if (packet.ackNumber == sequenceNum)
            {
                packet = new PacketFormate(false, true, false, sequenceNum++, packet.sequenceNumber + 1, 0);
                Array.Clear(buffer, 0, buffer.Length);
                buffer = Method.PacketToByteArray(packet);
                udpSocket.SendTo(buffer, serverIP);
            }

            return packetNum;
        }

        static bool Receive(Socket udpSocket, IPEndPoint serverIP, int packetNum, byte[] buffer)
        {
            bool[] receivePacket = new bool[packetNum];            
            int sequenceNum = 0;
            int ackNum = 0;
            EndPoint remoteEP = serverIP;            

            while(true)
            {
                // Array.Clear(buffer, 0, buffer.Length);
                int len = udpSocket.ReceiveFrom(buffer, ref remoteEP);
                PacketFormate packet = new PacketFormate();
                List<PacketFormate> packets = Method.ByteArrayToPacketArray(buffer, len);
                List<PacketFormate> sendPackets = new List<PacketFormate>();

                foreach (var item in packets)
                {
                    ackNum = item.sequenceNumber;
                    receivePacket[ackNum] = true;

                    packet = new PacketFormate(false, false, false, sequenceNum++, ackNum + 1, 0);
                    sendPackets.Add(packet);
                }
            
                buffer = Method.PacketArrayToByteArray(sendPackets);
                udpSocket.SendTo(buffer, serverIP);                
            }
        }
    }
}

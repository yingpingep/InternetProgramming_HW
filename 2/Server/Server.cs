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
            IPEndPoint listenOn = new IPEndPoint(IPAddress.Any, listenPort);

            Socket udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            udpSocket.Bind(listenOn);

            byte[] buffer = new byte[1024];
            Random random = new Random();
            int startNumber = random.Next() % 200;
            EndPoint remoteEP = Handshake(udpSocket, ref startNumber, buffer);

            // TODO Transfer
        }
        
        static EndPoint Handshake(Socket udpSocket, ref int startNumber, byte[] buffer)
        {
            EndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);

            int len = udpSocket.ReceiveFrom(buffer, ref remoteEP);
            PacketFormate packet = Method.ByteArrayToPacket(buffer, len);
            Console.WriteLine("Start three-way handshake");           
            
            if (packet.syn)
            {                
                Array.Clear(buffer, 0, buffer.Length);
                packet = new PacketFormate(false, true, false, startNumber++, packet.sequenceNumber + 1, 0);            
                buffer = Method.PacketToByteArray(packet);
                udpSocket.SendTo(buffer, remoteEP);                
            }

            Array.Clear(buffer, 0, buffer.Length);
            len = udpSocket.ReceiveFrom(buffer, ref remoteEP);            
            packet = Method.ByteArrayToPacket(buffer, len);

            // if (packet.ackNumber == ++startNumber)           
            return remoteEP;
        }
    }
}

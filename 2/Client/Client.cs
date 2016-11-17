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
            // IPEndPoint serverIP = new IPEndPoint(IPAddress.Parse("192.168.1.103"), 3353);
            Socket udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
       
            Random random = new Random();
            int startNumber = random.Next() % 200;    
            byte[] buffer = new byte[1024];

            int packetNum = Handshake(udpSocket, serverIP, ref startNumber,buffer);

            int windowSize = 5;
            int head = 0;
            int tail = head + windowSize;            
            PacketFormate[] isReceive = new PacketFormate[packetNum];
            while (true)
            {
                int oHead = head;
                int oTail = tail;
                ReceivePacket(udpSocket, serverIP, ref head, ref tail, ref isReceive, windowSize);
                Ack(udpSocket, serverIP, oHead, oTail, ref isReceive);
            }
        }

        static int Handshake(Socket udpSocket, EndPoint remoteEP, ref int sequenceNum, byte[] buffer)
        {
            PacketFormate packet = new PacketFormate(true, false, false, sequenceNum++, 0, 0);
            buffer = Method.PacketToByteArray(packet);
            udpSocket.SendTo(buffer, remoteEP);            

            Array.Clear(buffer, 0, buffer.Length);            
            int len = udpSocket.ReceiveFrom(buffer, ref remoteEP);
            packet = Method.ByteArrayToPacket(buffer, len);
            int packetNum = packet.data; 

            if (packet.ackNumber == sequenceNum)
            {
                packet = new PacketFormate(false, true, false, sequenceNum++, packet.sequenceNumber + 1, 0);
                Array.Clear(buffer, 0, buffer.Length);
                buffer = Method.PacketToByteArray(packet);
                udpSocket.SendTo(buffer, remoteEP);
            }

            return packetNum;
        }

        static void ReceivePacket(Socket udpSocket, EndPoint remoteEP, ref int head, ref int tail, ref PacketFormate[] isReceive, int windowSize)
        {
            byte[] buffer = new byte[1024];
            int len = udpSocket.ReceiveFrom(buffer, ref remoteEP);
            List<PacketFormate> packets = Method.ByteArrayToPacketList(buffer, len);

            head = tail;
            tail = head + windowSize;
            if (tail > isReceive.Count())
            {
                tail = isReceive.Count();
            }
            foreach (var item in packets)
            {
                isReceive[item.sequenceNumber].ack = true;
                Console.WriteLine("Receive sequence number {0}", item.sequenceNumber);                
            }
        }

        static void Ack(Socket udpSocket, EndPoint remoteEP, int head, int tail, ref PacketFormate[] isReceive)
        {
            bool isGot = true;
            List<PacketFormate> packets = new List<PacketFormate>();
            for (int i = head; i < tail; i++)
            {
                if (isReceive[i].ack && isReceive[i].data == 0)
                {
                    isReceive[i].data = 1;
                    PacketFormate packet = new PacketFormate(i, i + 1, 0);
                    packets.Add(packet);
                }
                else
                {
                    PacketFormate packet = new PacketFormate(i, i + 1, 0);
                    isGot = false;
                }
            }

            udpSocket.SendTo(Method.PacketListToByteArray(packets), remoteEP);

            if (!isGot)
            {
                ReAck(udpSocket, remoteEP, ref isReceive);   
            }
        }

        static void ReAck(Socket udpSocket, EndPoint remoteEP, ref PacketFormate[] isReceive)
        {
            byte[] buffer = new byte[1024];
            int len = udpSocket.ReceiveFrom(buffer, ref remoteEP);
            List<PacketFormate> packets = Method.ByteArrayToPacketList(buffer, len);
            List<PacketFormate> sendPackets = new List<PacketFormate>();

            foreach (var item in packets)
            {
                isReceive[item.sequenceNumber].ack = true;
                isReceive[item.sequenceNumber].data = 1;
                PacketFormate packet = new PacketFormate(item.sequenceNumber, item.sequenceNumber + 1, 0);
                sendPackets.Add(packet);
                Console.WriteLine("Receive sequence number {0}", item.sequenceNumber);
            }

            udpSocket.SendTo(Method.PacketListToByteArray(sendPackets), remoteEP);
        }
    }
}

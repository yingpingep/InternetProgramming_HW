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
            int startNumber = random.Next() % 200;            
            EndPoint remoteEP = Handshake(udpSocket, ref startNumber, transPacketNumber, buffer);
            PacketFormate[] isAck = new PacketFormate[transPacketNumber];

            int windowSize = 5;
            int head = 0;
            int tail = head + windowSize;
            int sequenceNum = 0;

            Console.WriteLine("Start transmission ...");             
            while(true)
            {
                int oHead = head;
                int oTail = tail;
                udpSocket.SendTo(TransSlideWindow(ref head, ref tail, windowSize, ref sequenceNum, transPacketNumber), remoteEP);
                while (true)
                {
                    ReceiveAck(udpSocket, remoteEP, ref isAck);
                    Retransfer(udpSocket, remoteEP, oHead, oTail, isAck);
                    if (ReceiveAck(udpSocket, remoteEP, ref isAck))
                    {
                        break;
                    }
                }                
            }          
        }
        
        static EndPoint Handshake(Socket udpSocket, ref int sequenceNum, int transPacketNumber, byte[] buffer)
        {            
            EndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);

            int len = udpSocket.ReceiveFrom(buffer, ref remoteEP);
            Console.WriteLine("Start three-way handshake ...");
            PacketFormate packet = Method.ByteArrayToPacket(buffer, len);           

            if (packet.syn)
            {                
                Array.Clear(buffer, 0, buffer.Length);
                packet = new PacketFormate(false, true, false, sequenceNum++, packet.sequenceNumber + 1, transPacketNumber);
                buffer = Method.PacketToByteArray(packet);
                udpSocket.SendTo(buffer, remoteEP);                
            }

            Array.Clear(buffer, 0, buffer.Length);
            len = udpSocket.ReceiveFrom(buffer, ref remoteEP);            
            packet = Method.ByteArrayToPacket(buffer, len);

            // if (packet.ackNumber == ++startNumber)
            return remoteEP;
        }

        static byte[] TransSlideWindow(ref int head, ref int tail, int windowSize, ref int sequenceNum, int transPacketNumber)
        {
            List<PacketFormate> packets = new List<PacketFormate>();
            for (int i = head; i < tail; i++)
            {
                //if (sequenceNum < transPacketNumber)
                //{
                //    PacketFormate packet = new PacketFormate(sequenceNum, 0, 0);
                //    packets.Add(packet);
                //    Console.WriteLine("Sent packet sequence number {0}", sequenceNum);
                //    sequenceNum += 1;
                //}
                //else
                //{
                //    break;
                //}                

                if (sequenceNum < transPacketNumber && sequenceNum % 3 != 0 || sequenceNum == 0)
                {
                    PacketFormate packet = new PacketFormate(sequenceNum, 0, 0);
                    packets.Add(packet);
                    Console.WriteLine("Sent packet sequence number {0}", sequenceNum);
                    sequenceNum += 1;
                }
                else if (sequenceNum < transPacketNumber && sequenceNum % 3 == 0)
                {
                    PacketFormate packet = new PacketFormate(++sequenceNum, 0, 0);
                    packets.Add(packet);
                    Console.WriteLine("Sent packet sequence number {0}", sequenceNum);
                    sequenceNum += 1;
                }
                else
                {
                    break;
                }

            }

            head = tail;       
            tail = head + windowSize;
            if (tail > transPacketNumber)
            {
                tail = transPacketNumber;
            }
            return Method.PacketListToByteArray(packets);
        }

        static bool ReceiveAck(Socket udpSocket, EndPoint remoteEP, ref PacketFormate[] isAck)
        {
            byte[] buffer = new byte[1024];            
            int len = udpSocket.ReceiveFrom(buffer, ref remoteEP);
            List<PacketFormate> packets = Method.ByteArrayToPacketList(buffer, len);

            foreach (var item in packets)
            {
                if (!isAck[item.ackNumber - 1].ack)
                {
                    isAck[item.ackNumber - 1].ack = true;                    
                }
                else
                {
                    isAck[item.ackNumber - 1].data += 1;
                }
                Console.WriteLine("Receive ack number {0}", item.ackNumber);
            }

            return true;
        }

        static void Retransfer(Socket udpSocket, EndPoint remoteEP, int head, int tail, PacketFormate[] isAck)
        {
            List<PacketFormate> packets = new List<PacketFormate>();
            for (int i = head; i < tail; i++)
            {
                if (!isAck[i].ack || isAck[i].data == 3)
                {
                    PacketFormate packet = new PacketFormate(i, 0, 0);
                    packets.Add(packet);
                    Console.WriteLine("Resent packet sequence number {0}", i);
                }
            }
          
            if (packets.Count != 0)
            {
                byte[] buffer = Method.PacketListToByteArray(packets);
                udpSocket.SendTo(buffer, remoteEP);
            }            
        }
    }
}
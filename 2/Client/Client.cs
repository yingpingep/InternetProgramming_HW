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
            Socket udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            Random random = new Random();
            int startNumber = random.Next() % 200;    
            byte[] buffer = new byte[1024];
            bool connected  = Handshake(udpSocket, serverIP, ref startNumber,buffer);

            // TODO Receive
        }

        static bool Handshake(Socket udpSocket, IPEndPoint serverIP, ref int startNumber, byte[] buffer)
        {
            PacketFormate packet = new PacketFormate(true, false, false, startNumber++, 0, 0);
            buffer = Method.PacketToByteArray(packet);
            udpSocket.SendTo(buffer, serverIP);            

            Array.Clear(buffer, 0, buffer.Length);
            EndPoint remoteEP = serverIP;
            int len = udpSocket.ReceiveFrom(buffer, ref remoteEP);   
            packet = Method.ByteArrayToPacket(buffer, len);        

            if (packet.ackNumber == startNumber)
            {
                packet = new PacketFormate(false, true, false, startNumber++, packet.sequenceNumber + 1, 0);
                Array.Clear(buffer, 0, buffer.Length);
                buffer = Method.PacketToByteArray(packet);
                udpSocket.SendTo(buffer, serverIP);                
            }

            return true;
        }
    }
}

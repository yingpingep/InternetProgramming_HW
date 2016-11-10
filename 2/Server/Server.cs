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

            EndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
            int revlen = udpSocket.ReceiveFrom(buffer, ref remoteEP);
            PacketFormate tt = Method.ByteArrayToPacket(buffer, revlen);

            Console.WriteLine(tt.data + "   " + revlen);
        }   
    }
}

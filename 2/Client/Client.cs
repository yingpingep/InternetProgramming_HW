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
            PacketFormate test = new PacketFormate(true, false, false, 1, 1, 3456789);
            byte[] buffer = Method.PacketToByteArray(test);

            udpSocket.SendTo(buffer, serverIP);
            Console.WriteLine("Sent");
        }     
    }
}

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ConsoleApplication
{
    public class Client
    {
        public static void Main(string[] args)
        {
            // args[0] for IP address
            // args[1] for Port
            // args[2] for File name

            // To creat a Server side Point
            // Get IP and Port from args[]
            IPAddress ServerIP = IPAddress.Parse(args[0]);
            int ServerPort = Convert.ToInt32(args[1]);
            IPEndPoint Server = new IPEndPoint(ServerIP, ServerPort);

            // Create a Socket used to connect to Server 
            Socket ConnectToServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ConnectToServer.Connect(Server);

            string FileName = args[2];

            // Send File name to Server            
            ConnectToServer.Send(Encoding.ASCII.GetBytes(args[2]));

            byte[] SureSize = new byte[1024];               // Used to store file size

            // Determice the length actually have data
            int tmp = ConnectToServer.Receive(SureSize);    

            // Findout the file size
            int DataSize = Convert.ToInt32(Encoding.ASCII.GetString(SureSize, 0, tmp));

            byte[] buffer = new byte[1024];
            
            // Creat a FileStream to store file
            Stream file = new FileStream(FileName, FileMode.Create);
            int UnGetSize = DataSize;       // Remind data
            
            while (UnGetSize > 0)
            {                
                int ReceiveData = ConnectToServer.Receive(buffer);
                UnGetSize -= ReceiveData;           
                file.Write(buffer, 0, ReceiveData);                
            }
            
            Console.WriteLine("Receive Sucessful!");
        }
    }
}

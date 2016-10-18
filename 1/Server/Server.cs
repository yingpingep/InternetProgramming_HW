using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication
{
    public class Server
    {
        public static void Main(string[] args)
        {
            // args[0] for IP address
            // args[1] for Port
            
            IPAddress ListenIP = IPAddress.Parse(args[0]);
            int ListenPort = Convert.ToInt32(args[1]);                
            
            // Create an end point with IP and Port 
            IPEndPoint ListenOn = new IPEndPoint(ListenIP, ListenPort);

            // Create a socket to handle connection
            Socket ConnectFromClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            
            ConnectFromClient.Bind(ListenOn);   // Bind Socket on end point which is listening
            ConnectFromClient.Listen(10);       // Only allow 10 connections

            byte[] buffer = new byte[1024];     // Use to receive the file name from client
            
            while(true)
            {
                Console.WriteLine("Wait for Client ... ");
                Socket Client = ConnectFromClient.Accept();     // Handle connection
                
                int len = Client.Receive(buffer);               // Get the data size (byte)

                // Decode the receive data from ASCII to string
                string FileName = Encoding.ASCII.GetString(buffer, 0, len);
                FileInfo FileCheck = new FileInfo(FileName);

                // Send file Size to Client
                Client.Send(Encoding.ASCII.GetBytes(FileCheck.Length.ToString()));                
                
                // Send File to Client 
                byte[] data = new byte[FileCheck.Length];
                File.ReadAllBytes(FileName).CopyTo(data, 0);             
                Client.Send(data);

                Console.WriteLine("Send Sucessful!");                    
                Console.WriteLine();
            }                    
        }
    }
}

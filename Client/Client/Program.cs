using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace Client
{


    class Program
    {

        private static Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        static void Main(string[] args)
        {
            connectLoop();
            sendRequests();
            Console.ReadLine();

        }

        private static void sendRequests()
        {
            while (true)
            {
                Console.Write("Input numbers to sort: ");
                string input = Console.ReadLine();
                byte[] sendBuff = Encoding.ASCII.GetBytes(input);
                clientSocket.Send(sendBuff);

                byte[] recBuff = new byte[1024];
                int rec = clientSocket.Receive(recBuff);
                byte[] data = new byte[rec];
                Array.Copy(recBuff, data, rec);
                Console.WriteLine(Encoding.ASCII.GetString(data));


            }



        }

        private static void connectLoop()
        {
            int attempts = 0;

            while (!clientSocket.Connected)
            {
                try
                {
                    attempts++;
                    clientSocket.Connect(IPAddress.Loopback, 100);

                }
                catch (SocketException)
                {
                    Console.Clear();
                    Console.WriteLine("Connecting.. Attempts: " + attempts);

                }



            }

            Console.Clear();
            Console.WriteLine("Connected");
        }
    }
}


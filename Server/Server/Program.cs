using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace Server
{
    class Program
    {

        private static byte[] buf = new byte[1024];
        private static List<Socket> clientSockets = new List<Socket>();
        private static Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        static void Main(string[] args)
        {
            setupServer();
            Console.ReadLine();

        }

        private static void setupServer()
        {
            Console.WriteLine("Setting up server");
            serverSocket.Bind(new IPEndPoint(IPAddress.Any, 100));
            serverSocket.Listen(1);
            serverSocket.BeginAccept(new AsyncCallback(acceptCallBack), null);


        }

        private static void acceptCallBack(IAsyncResult AR)
        {
            Socket socket = serverSocket.EndAccept(AR);
            clientSockets.Add(socket);
            socket.BeginReceive(buf, 0, buf.Length, SocketFlags.None, new AsyncCallback(recieveCallBack), socket);

            Console.WriteLine("Connected.");
            serverSocket.BeginAccept(new AsyncCallback(acceptCallBack), null);
          
        }

        private static void recieveCallBack(IAsyncResult AR)
        {
            Socket socket = (Socket)AR.AsyncState;
            int rec = socket.EndReceive(AR);
            byte[] data = new byte[rec];
            Array.Copy(buf, data, rec);
            string text = Encoding.ASCII.GetString(data);
            Console.Clear();
            Console.WriteLine("Recieved Data " + text);
            bool intCheck = true;

            string[] recievedData = text.Split(' ');
            int[] values = new int[recievedData.Length];

            for (int i = 0; i < values.Length; i++)
            {
                int check;
                if(int.TryParse(recievedData[i], out check)){
                    values[i] = Convert.ToInt32(recievedData[i]);
                }else{

                   intCheck = false;

                }

            }

            quickSort(values, 0, values.Length - 1);
            string response;
            if(intCheck){
                response = string.Join(" ", values);

            }else{
                response = "Please input valid numbers";
            }
            
            
            byte[] retData = Encoding.ASCII.GetBytes(response);
            socket.BeginSend(retData, 0, retData.Length, SocketFlags.None, new AsyncCallback(sendCallBack), socket);
            socket.BeginReceive(buf, 0, buf.Length, SocketFlags.None, new AsyncCallback(recieveCallBack), socket);

        }

        private static void sendCallBack(IAsyncResult AR)
        {
            Socket socket = (Socket)AR.AsyncState;
            socket.EndSend(AR);
        }

        private static void quickSort(int[] arr, int left, int right)
        {
            int i = left;
            int j = right;
            int middle = (i + j) / 2;
            int pivot = arr[middle];

            while (i <= j)
            {
                while (arr[i] < pivot)
                {
                    i++;

                }
                while (arr[j] > pivot)
                {
                    j--;

                }

                if (i <= j)
                {
                    int tmp = arr[i];
                    arr[i] = arr[j];
                    arr[j] = tmp;
                    i++;
                    j--;

                }



            }

            if (left < j)
            {
                quickSort(arr, left, j);


            }
            if (right > i)
            {
                quickSort(arr, i, right);


            }

        }
    }
}

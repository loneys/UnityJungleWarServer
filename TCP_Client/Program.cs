using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace TCP_Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //家里IP
            clientSocket.Connect(new IPEndPoint(IPAddress.Parse("192.168.3.39"), 88));
            //公司IP
            //clientSocket.Connect(new IPEndPoint(IPAddress.Parse("192.168.6.107"), 88));

            byte[] data = new byte[1024];
            int count = clientSocket.Receive(data);
            string msg = Encoding.UTF8.GetString(data, 0, count);
            Console.Write(msg);

            //while(true)
            //{
            //    string s = Console.ReadLine();
            //    clientSocket.Send(Encoding.UTF8.GetBytes(s));
            //}

            //模拟粘包的问题
            for (int i = 0; i < 100; i++)
            {
                clientSocket.Send(Message.GetBytes(i.ToString()));
            }

            string msgData = "";//这里的数据设置超级大就可以了
            clientSocket.Send(Encoding.UTF8.GetBytes(msgData.ToString()));

            Console.ReadKey();
            clientSocket.Close();
        }
    }
}

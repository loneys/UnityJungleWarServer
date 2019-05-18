using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace TCP_Server
{
    class Program
    {
        static void Main(string[] args)
        {
            StartServerAsync();
            Console.ReadKey();
        }

        static byte[] dataBuffer = new byte[1024];
        


        //异步的方式接收
        static void StartServerAsync()
        {
            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //192.168.6.107   127.0.0.1

            //IpAddress xxx.xx.xx.xx 
            //IpEndPoint xxx.xx.xx.xx:port
            //IPAddress ipAddress = new IPAddress(new byte[] { 192, 168, 6, 107 });   //这种方式不推荐

            //家里IP
            IPAddress ipAddress = IPAddress.Parse("192.168.3.39");
            //公司IP
            //IPAddress ipAddress = IPAddress.Parse("192.168.6.107");

            IPEndPoint ipEndpoint = new IPEndPoint(ipAddress, 88);
            serverSocket.Bind(ipEndpoint);//绑定ip和端口
            serverSocket.Listen(0);//开始监听端口号,数字0表示可以同时监听的个数。

            //这个只能同步接收一个客户端
            //Socket clientSocket = serverSocket.Accept();//接收一个客户端的链接

            serverSocket.BeginAccept(AcceptCallBack, serverSocket);

        }

        static Message msg = new Message();

        //接收多个客户端链接
        static void AcceptCallBack(IAsyncResult ar)
        {
            Socket serverSocket = ar.AsyncState as Socket;
            Socket clientSocket = serverSocket.EndAccept(ar);

            //向客户端发送一条消息（）
            //string msg = "Hello client! 你好...";
            //byte[] data = System.Text.Encoding.UTF8.GetBytes(msg);//需要将字符串转化成二进制数组
            //clientSocket.Send(data);//发送数据给客户端
            ////开始接收客户端发过来的数据(没有处理粘包)
            //clientSocket.BeginReceive(dataBuffer, 0, 1024, SocketFlags.None, ReceiveCallBack, clientSocket);

            string msgString = "Hello client! 你好...";
            byte[] data = System.Text.Encoding.UTF8.GetBytes(msgString);//需要将字符串转化成二进制数组
            clientSocket.Send(data);//发送数据给客户端

            //开始接收客户端发过来的数据(处理粘包情况)
            clientSocket.BeginReceive(msg.Data, msg.StartIndex, msg.RemainSize, SocketFlags.None, ReceiveCallBack, clientSocket);

            //继续接收新的客户端链接
            serverSocket.BeginAccept(AcceptCallBack, serverSocket);

        }


        //连续接收消息
        static void ReceiveCallBack(IAsyncResult ar)
        {
            Socket clientSocket = null;
            try
            {
                clientSocket = ar.AsyncState as Socket;
                int count = clientSocket.EndReceive(ar);
                if (count == 0) 
                {
                    clientSocket.Close();
                    return;
                }
                //string msg = Encoding.UTF8.GetString(dataBuffer, 0, count);
                //Console.WriteLine("从客户端接收到数据:" + count + " " + msg);
                //clientSocket.BeginReceive(dataBuffer, 0, 1024, SocketFlags.None, ReceiveCallBack, clientSocket);
                //Console.WriteLine("count" + count);
                msg.AddCount(count);
                msg.ReadMessage();
                //string msgStr = Encoding.UTF8.GetString(dataBuffer, 0, count);
                //Console.WriteLine("从客户端接收到数据:" + count + " " + msgStr);
                //clientSocket.BeginReceive(dataBuffer, 0, 1024, SocketFlags.None, ReceiveCallBack, clientSocket);

                clientSocket.BeginReceive(msg.Data, msg.StartIndex, msg.RemainSize, SocketFlags.None, ReceiveCallBack, clientSocket);

            }
            catch(Exception e)
            {
                Console.WriteLine(e);
                if (clientSocket != null) 
                {
                    clientSocket.Close();
                }
            }
            finally
            {

            }
        }

        //同步接收的方式
        static void StartServerSync()
        {
            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //192.168.6.107   127.0.0.1

            //IpAddress xxx.xx.xx.xx 
            //IpEndPoint xxx.xx.xx.xx:port
            //IPAddress ipAddress = new IPAddress(new byte[] { 192, 168, 6, 107 });   //这种方式不推荐

            IPAddress ipAddress = IPAddress.Parse("192.168.6.107");
            IPEndPoint ipEndpoint = new IPEndPoint(ipAddress, 88);
            serverSocket.Bind(ipEndpoint);//绑定ip和端口
            serverSocket.Listen(0);//开始监听端口号,数字0表示可以同时监听的个数。
            Socket clientSocket = serverSocket.Accept();//接收一个客户端的链接

            //向客户端发送一条消息
            string msg = "Hello client! 你好...";
            byte[] data = System.Text.Encoding.UTF8.GetBytes(msg);//需要将字符串转化成二进制数组
            clientSocket.Send(data);//发送数据给客户端

            //接收客户端的一条消息
            byte[] dataBuffer = new byte[1024];//定义一个1024个字节的数组,如果这个数组超级大,那么服务端发送过来的数据很大也不会分包
            int count = clientSocket.Receive(dataBuffer);//接收客户端发过来的消息
            string msgReceive = System.Text.Encoding.UTF8.GetString(dataBuffer, 0, count);//将接收的二进制数据装换为字符串
            Console.WriteLine(msgReceive);

            Console.ReadKey();
            clientSocket.Close();//关闭与客户端链接
            serverSocket.Close();//关闭服务端
        }
    }
}

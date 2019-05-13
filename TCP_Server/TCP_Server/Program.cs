﻿using System;
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
            byte[] dataBuffer = new byte[1024];//定义一个1024个字节的数组
            int count = clientSocket.Receive(dataBuffer);//接收客户端发过来的消息
            string msgReceive = System.Text.Encoding.UTF8.GetString(dataBuffer, 0, count);//将接收的二进制数据装换为字符串
            Console.WriteLine(msgReceive);

            Console.ReadKey();
            clientSocket.Close();//关闭与客户端链接
            serverSocket.Close();//关闭服务端

        }
    }
}
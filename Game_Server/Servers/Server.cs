using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using Game_Server.Controller;
using Common;

namespace Game_Server.Servers
{
    class Server
    {
        private IPEndPoint ipEndPoint;
        private Socket serverSocket;
        private List<Client> clientList = new List<Client>();
        private ControllerManager controllerManager;

        public Server() { }
        //初始化Server
        public Server(string ipStr,int port)
        {
            //创建一个controllerManager管理类
            controllerManager = new ControllerManager(this);
            //设置服务器IP和端口
            SetIpAndPort(ipStr,port);
        }
        public void SetIpAndPort(string ipStr,int port)
        {
            ipEndPoint = new IPEndPoint(IPAddress.Parse(ipStr), port);
        }

        public void Start()
        {
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream,ProtocolType.Tcp);
            serverSocket.Bind(ipEndPoint);
            serverSocket.Listen(0);
            serverSocket.BeginAccept(AcceptCallBack,null);
        }

        private void AcceptCallBack(IAsyncResult ar)
        {
            Socket clientSocket = serverSocket.EndAccept(ar);
            Client client = new Client(clientSocket,this);
            client.Start();
            clientList.Add(client);
        }

        public void RemoveClient(Client client)
        {
            lock(clientList)
            {
                clientList.Remove(client);
            }
        }

        //这个函数是controllerManager处理完消息后，调用此方法，发送消息给客户端
        public void SendResponse(Client client,ActionCode actionCode,string data)
        {
            client.Send(actionCode, data);
        }

        public void HandleRequest(RequestCode requestCode, ActionCode actioncode, string data, Client client)
        {
            controllerManager.HandleRequest(requestCode, actioncode, data, client);
        }

    }
}

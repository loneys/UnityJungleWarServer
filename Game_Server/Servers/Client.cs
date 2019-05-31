using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using Common;
using MySql.Data.MySqlClient;
using Game_Server.Tool;

namespace Game_Server.Servers
{
    class Client
    {
        private Socket clientSocket;
        private Server server;
        private Message msg = new Message();
        private MySqlConnection mysqlConn;

        public Client() { }

        public Client(Socket clientSocket,Server server)
        {
            this.clientSocket = clientSocket;
            this.server = server;
            mysqlConn = ConnHelper.Connect();
        }

        public void Start()
        {
            clientSocket.BeginReceive(msg.Data, msg.StartIndex, msg.RemainSize, SocketFlags.None,ReceviceCallBack,null);
        }

        private void ReceviceCallBack(IAsyncResult ar)
        {
            try
            {
                int count = clientSocket.EndReceive(ar);
                if (count == 0)
                {
                    Close();
                }
                //TODO 处理接收到的数据
                msg.ReadMessage(count,OnProcessMessage);
                Start();
                //clientSocket.BeginReceive(null, 0, 0, SocketFlags.None, ReceviceCallBack, null);
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
                Close();
            }

        }

        //创建一个解析完消息的回调函数
        private void OnProcessMessage(RequestCode requestCode,ActionCode actionCode,string data)
        {
            server.HandleRequest(requestCode, actionCode, data, this);
        }


        private void Close()
        {
            ConnHelper.CloseConnection(mysqlConn);
            if(clientSocket!=null)
            {
                clientSocket.Close();
                server.RemoveClient(this);
            }
        }

        public void Send(RequestCode requestCode,string data)
        {
            byte[] bytes = Message.PackData(requestCode, data);
            clientSocket.Send(bytes);
        }
    }
}

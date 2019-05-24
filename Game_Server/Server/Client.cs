using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace Game_Server.Server
{
    class Client
    {
        private Socket clientSocket;
        private Server server;

        public Client() { }

        public Client(Socket clientSocket,Server server)
        {
            this.clientSocket = clientSocket;
            this.server = server;
        }

        public void Start()
        {
            clientSocket.BeginReceive(null, 0, 0, SocketFlags.None,ReceviceCallBack,null);
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
                clientSocket.BeginReceive(null, 0, 0, SocketFlags.None, ReceviceCallBack, null);
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
                Close();
            }

        }

        private void Close()
        {
            if(clientSocket!=null)
            {
                clientSocket.Close();
                server.RemoveClient(this);
            }
        }
    }
}

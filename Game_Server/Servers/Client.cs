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
using Game_Server.Model;
using Game_Server.DAO;

namespace Game_Server.Servers
{
    class Client
    {
        private Socket clientSocket;
        private Server server;
        private Message msg = new Message();
        private MySqlConnection mysqlConn;

        private Room room;

        private User user;
        private Result result;
        private ResultDAO resultDAO = new ResultDAO();

        public int HP
        {
            get;set;
        }

        public bool TakeDamage(int damage)
        {
            HP -= damage;
            HP = Math.Max(HP, 0);
            if(HP <= 0)
            {
                return true;
            }
            return false;
        }
        public bool IsDie()
        {
            return HP <= 0;
        }

        public MySqlConnection MySQLConn
        {
            get
            {
                return mysqlConn;
            }
        }

        public void SetUserData(User user, Result result)
        {
            this.user = user;
            this.result = result;
        }

        public string GetUserData()
        {
            return user.Id + "," + user.Username + "," + result.TotalCount + "," + result.WinCount;
        }

        public Room Room
        {
            set { room = value; }
            get { return room; }
        }

        public int GetUserId()
        {
            return user.Id;
        }

        public Client() { }

        public Client(Socket clientSocket,Server server)
        {
            this.clientSocket = clientSocket;
            this.server = server;
            mysqlConn = ConnHelper.Connect();
        }

        public void Start()
        {
            if(clientSocket==null ||clientSocket.Connected==false)
            {
                return;
            }
            clientSocket.BeginReceive(msg.Data, msg.StartIndex, msg.RemainSize, SocketFlags.None,ReceviceCallBack,null);
        }

        private void ReceviceCallBack(IAsyncResult ar)
        {
            try
            {
                if (clientSocket == null || clientSocket.Connected == false)
                {
                    return;
                }
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
                if (room != null)
                {
                    room.QuitRoom(this);
                }
                server.RemoveClient(this);
            }
        }

        public void Send(ActionCode actionCode,string data)
        {
            try
            {
                byte[] bytes = Message.PackData(actionCode, data);
                Console.WriteLine("\n发送消息:\n" + "动作:" + actionCode + " 数据:" + data);
                clientSocket.Send(bytes);
            }
            catch(Exception e)
            {
                Console.WriteLine("无法发送消息:" + e);
            }
        }

        public bool IsHouseOwner()
        {
            return room.IsHouseOwner(this);
        }

        public void UpdateResult(bool isVictory)
        {
            UpdateResultToDB(isVictory);
            UpdateResultToClient();
        }

        public void UpdateResultToDB(bool isVictory)
        {
            result.TotalCount++;
            if (isVictory)
            {
                result.WinCount++;
            }
            resultDAO.UpdateOrAddResult(mysqlConn, result);
        }
        public void UpdateResultToClient()
        {
            Send(ActionCode.UpdateResult, string.Format("{0},{1}", result.TotalCount, result.WinCount));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace Game_Server.Servers
{
    enum RoomState
    {
        WaitingJoin,
        WaitingBattle,
        Battle,
        End
    }
    class Room
    {
        private List<Client> clientRoom = new List<Client>();
        private RoomState state = RoomState.WaitingJoin;
        private Server server;

        public Room(Server server)
        {
            this.server = server;
        }

        public bool IsWaitingJosin()
        {
            return state == RoomState.WaitingJoin;
        }

        public void AddClient(Client client)
        {
            clientRoom.Add(client);
            client.Room = this;
            if(clientRoom.Count>=2)
            {
                state = RoomState.WaitingBattle;
            }
        }

        public string GetHouseOwnerData()
        {
            return clientRoom[0].GetUserData();
        }

        public void Close(Client client)
        {
            if(client==clientRoom[0])
            {
                server.RemoveRoom(this);
            }
            else
            {
                clientRoom.Remove(client);
            }
            
        }

        public int GetId()
        {
            if(clientRoom.Count>0)
            {
                return clientRoom[0].GetUserId();
            }
            return -1;
        }

        public String GetRoomData()
        {
            StringBuilder sb = new StringBuilder();
            foreach(Client client in clientRoom)
            {
                sb.Append(client.GetUserData() + "|");
            }
            if(sb.Length>0)
            {
                sb.Remove(sb.Length - 1, 1);
            }
            return sb.ToString();
        }
        public void BroadcastMessage(Client excludeClient,ActionCode actionCode,string data)
        {
            foreach(Client client in clientRoom)
            {
                if(client!=excludeClient)
                {
                    server.SendResponse(client, actionCode, data);
                }
            }
        }
    }

}

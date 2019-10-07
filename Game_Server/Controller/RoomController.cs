using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Game_Server.Servers;

namespace Game_Server.Controller
{
    class RoomController:BaseController
    {
        public RoomController()
        {
            requestCode = RequestCode.Room;
        }

        public String CreateRoom(string data, Client client, Server server)
        {
            server.CreateRoom(client);
            return ((int)ReturnCode.Success).ToString() + ',' + ((int)RoleType.Blue).ToString();
        }

        public String ListRoom(string data, Client client, Server server)
        {
            StringBuilder sb = new StringBuilder();
            foreach (Room room in server.GetRoomList())
            {

                if(room.IsWaitingJosin())
                {
                    sb.Append(room.GetHouseOwnerData()+"|");
                }
            }
            if(sb.Length==0)
            {
                sb.Append("0");
            }
            else
            {
                sb.Remove(sb.Length - 1, 1);
            }
            return sb.ToString();
        }
        public String JoinRoom(string data, Client client, Server server)
        {
            int id = int.Parse(data);
            Room room = server.GetRoomById(id);
            if (room == null)
            {
                //TODO
                return ((int)ReturnCode.NotFound).ToString();
            }
            else if (room.IsWaitingJosin() == false)
            {
                //TODO
                return ((int)ReturnCode.Fail).ToString();
            }
            else
            {
                room.AddClient(client);
                string roomData = room.GetRoomData();//"returncode,roleType-id,username,totalCount,winCount|returncode-id,username,totalCount,winCount"
                room.BroadcastMessage(client, ActionCode.UpdateRoom, roomData);
                return ((int)ReturnCode.Success).ToString() + ',' + ((int)RoleType.Red).ToString() + "-" + roomData;
            }
        }

        public string QuitRoom(string data,Client client,Server server)
        {
            bool isHouseOwner = client.IsHouseOwner();
            Room room = client.Room;
            if (isHouseOwner)
            {
                room.BroadcastMessage(client, ActionCode.QuitRoom, ((int)ReturnCode.Success).ToString());
                room.Close();
                return ((int)ReturnCode.Success).ToString();
            }
            else
            {
                
                client.Room.RemoveClient(client);              
                
                room.BroadcastMessage(client, ActionCode.UpdateRoom, room.GetRoomData());

                return ((int)ReturnCode.Success).ToString();
            }
        }

        public string QuitBattle(string data, Client client, Server server)
        {
            Room room = client.Room;
            if (room != null)
            {
                room.BroadcastMessage(null, ActionCode.QuitBattle, "r");
                room.Close();
            }
            return null;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace Game_Server.Servers
{
    class Message
    {
        private byte[] data = new byte[1024];
        //我们存储了多少个字节在数组里,处理一条就减少。这个值相当于长度。
        private int startIndex = 0;

        //public void AddCount(int count)
        //{
        //    startIndex += count;
        //}

        public byte[] Data
        {
            get
            {
                return data;
            }
        }

        public int StartIndex
        {
            get
            {
                return startIndex;
            }
        }
        public int RemainSize
        {
            get
            {
                return data.Length - startIndex;
            }
        }

        //解析数据
        public void ReadMessage(int newDataAmount,Action<RequestCode,ActionCode,string> processDataCallBack)
        {
            startIndex += newDataAmount;
            while (true)
            {
                if (startIndex <= 4)
                {
                    return;
                }
                //解析出数据总长
                int count = BitConverter.ToInt32(data, 0);
                if ((startIndex - 4) >= count)
                {
                    //Console.WriteLine("startIndex" + startIndex);
                    //Console.WriteLine("count" + count);

                    //string s = Encoding.UTF8.GetString(data, 4, count);
                    //Console.WriteLine("解析出来一条数据:" + s);
                    //解析出requesCode,这里的RequestCode 类型是枚举类型，用 as 方式转型不行，采用强制转型的方式。
                    RequestCode requestCode = (RequestCode) BitConverter.ToInt32(data, 4);
                    //解析出actionCode
                    ActionCode actionCode = (ActionCode)BitConverter.ToInt32(data, 8);
                    //解析出数据
                    string s = Encoding.UTF8.GetString(data, 12, count-8);
                    Console.WriteLine("解析出来一条数据:" + s);
                    //触发回调
                    processDataCallBack(requestCode, actionCode, s);
                    Array.Copy(data, count + 4, data, 0, startIndex - 4 - count);
                    startIndex -= (count + 4);
                }
                else
                {
                    break;
                }
            }
        }

        public static byte[] PackData(RequestCode requestData,string data)
        {
            //将消息号转换为字节数组
            byte[] requestCodeBytes = BitConverter.GetBytes((int)requestData);
            //将消息内容转换为字节数组
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);
            //得到消息号和消息内容的长度和
            int dataAmount = requestCodeBytes.Length + dataBytes.Length;
            byte[] dataAmountBytes = BitConverter.GetBytes(dataAmount);
            return dataAmountBytes.Concat(requestCodeBytes).Concat(dataBytes) as byte[];
        }
    }
}

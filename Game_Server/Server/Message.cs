using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_Server.Server
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
        public void ReadMessage(int newDataAmount)
        {
            startIndex += newDataAmount;
            while (true)
            {
                if (startIndex <= 4)
                {
                    return;
                }
                int count = BitConverter.ToInt32(data, 0);
                if ((startIndex - 4) >= count)
                {
                    Console.WriteLine("startIndex" + startIndex);
                    Console.WriteLine("count" + count);

                    string s = Encoding.UTF8.GetString(data, 4, count);
                    Console.WriteLine("解析出来一条数据:" + s);
                    Array.Copy(data, count + 4, data, 0, startIndex - 4 - count);
                    startIndex -= (count + 4);
                }
                else
                {
                    break;
                }
            }
        }
    }
}

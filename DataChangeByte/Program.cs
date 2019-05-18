using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataChangeByte
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Text.Encoding.UTF8.GetBytes("100")
            //byte[] data = Encoding.UTF8.GetBytes("100");    //这种方式是按照字符来转换
            int count = 10000;
            byte[] data = BitConverter.GetBytes(count);     //这种方式根据类型来转换
            foreach(byte b in data)
            {
                Console.Write(b + ":");
            }
            Console.ReadKey();
        }
    }
}

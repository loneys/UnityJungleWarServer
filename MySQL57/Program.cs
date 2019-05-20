using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;


namespace MySQL57
{
    class Program
    {
        static void Main(string[] args)
        {
            string constr = "Database = test007;Data Source = 127.0.0.1;port = 3306;User Id =root;Password = root;";
            MySqlConnection conn = new MySqlConnection(constr);

            conn.Open();

            #region 查询
            //MySqlCommand cmd = new MySqlCommand("select * from user", conn);

            //MySqlDataReader reader = cmd.ExecuteReader();

            //while (reader.Read())
            //{
            //    string username = reader.GetString("username");
            //    string password = reader.GetString("password");
            //    Console.WriteLine(username + ":" + password);

            //}
            #endregion

            //reader.Close();

            #region 插入

            //string username = "liuhengying";string password = "liuhengying";


            ////这个会产生mysql注入问题
            ////string username = "haha"; string password = "haha;delete from user;";
            ////MySqlCommand cmd = new MySqlCommand("insert into user set username = '" + username + "'" + ",password ='" + password + "'", conn);
            //MySqlCommand cmd = new MySqlCommand("insert into user set username = @un,password = @pwd", conn);

            //cmd.Parameters.AddWithValue("un", username);
            //cmd.Parameters.AddWithValue("pwd", password);

            //cmd.ExecuteNonQuery();

            #endregion

            #region
            //删除
            //MySqlCommand cmd = new MySqlCommand("delete from user where id = @id", conn);

            //cmd.Parameters.AddWithValue("id", 6);
            //cmd.ExecuteNonQuery();
            #endregion





            conn.Close();

            Console.ReadKey();
        }
    }
}

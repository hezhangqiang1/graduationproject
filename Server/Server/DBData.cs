/* 
 * 数据库：Mysql的连接
 * 脚本功能：数据库的增删改查
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Socketunity
{
    class DBData
    {
        public static string constr = "server=localhost;Database=Graduation;User Id=root;password=hzq13576557892";
        public static string dbdata;
        public static  Dictionary<string, string> Dicdbdata = new Dictionary<string, string>();
        /// <summary>
        /// 查询数据库数据
        /// </summary>
        public static  void Connecttest()
        {
            try
            {
                MySqlConnection conn = new MySqlConnection(constr);
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("select * from user ", conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string username = reader.GetString("username");
                    string password = reader.GetString("password");
                    float demage = reader.GetFloat("demage");
                    float defance = reader.GetFloat("defance");
                    float speed = reader.GetFloat("speed");
                    dbdata = "DB" + "/" + username + "/" + password + "/" + demage + "/" + defance + "/" + speed;
                    Dicdbdata.Add(username, dbdata);   
                }
                //foreach (var item in Dicdbdata)
                //{
                //    Console.WriteLine("用户名{0},字符串{1}", item.Key, item.Value);
                //}
                reader.Close();
                conn.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine("数据库操作出现异常" + e);
            }
           

        }
        /// <summary>
        /// 数据库的插入操作
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="demage">攻击力</param>
        /// <param name="defence">防御力</param>
        /// <param name="speed">速度</param>
        public static void Insert(string username, string password, float  demage, float  defance,float speed)
        {
            try
            {
                MySqlConnection mycon = new MySqlConnection(constr);   
                mycon.Open();
                MySqlCommand mycmd = new MySqlCommand("insert into user(username,password,demage,defance,speed) " +
                    "values('" + username + "','" + password + "','" + demage + "','" + defance + "','" + speed + "')", mycon);
                if (mycmd.ExecuteNonQuery() > 0)
                    mycon.Close();
            }catch(Exception e)
            {
                Console.WriteLine("数据库插入操作出现异常" + e.Message);
            }
           
        }



    }
}

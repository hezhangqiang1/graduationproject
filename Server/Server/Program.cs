/* 
 * 服务器端：Socket Tcp协议编写服务器 
 * 脚本功能：持有各个客户端，广播消息等
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;


namespace Socketunity
{
    class Program
    { 
        static List<Client> clientlist = new List<Client>();
        static Dictionary<string, Client> ID = new Dictionary<string, Client>();
        static List<Client> startfightclient = new List<Client> ();
        public static string clientID;
        
        /// <summary>
        /// 广播消息的方法
        /// </summary>
        /// <param name="message"></param>
        public static void BroadcastMessage(string message)
        {
            var NotConectesList = new List<Client>();
            foreach (var client in clientlist)
            {
                if (client.Connected)
                    client.SendMessage(message);
                else
                {
                    NotConectesList.Add(client);
                }
            }
            foreach (var temp in NotConectesList)
            {
                clientlist.Remove(temp);
            }
        }
        /// <summary>
        /// 将开始匹配的客户端加入集合
        /// </summary>
        public static  void StartFighting()
        {
            var FightingList = new List<Client>();
            foreach (var client in clientlist)
            {
                if (client.mClientState == ClientState.StateFight)
                {
                    startfightclient.Add(client);
                    client.mClientState = ClientState.Fighting;
                    if (startfightclient.Count >= 2)
                    {
                        Console.WriteLine("开始游戏");
                        BroadcastMessage("开始游戏");
                        if(client.mClientState == ClientState.Fighting)
                        {
                            FightingList.Add(client);
                            startfightclient.Remove(client);
                            Console.WriteLine("移除成功，剩余匹配队列还剩：" + startfightclient.Count.ToString());
                        }
                    }
                    Console.WriteLine("有一个客户端开始匹配");
                }
            }
            foreach (var client in FightingList)
            {
                startfightclient.Remove(client);
            }
        }
        static void Main(string[] args)
        {

            Socket TcpServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            TcpServer.Bind(new IPEndPoint(IPAddress.Parse("192.168.56.1"), 7788));
            TcpServer.Listen(100); 
            Console.WriteLine("服务器已启动");
            DBData.Connecttest();//数据库连接测试
            
            while (true)
            {

                Socket clientSocket = TcpServer.Accept();
                Console.WriteLine("有一个客户端连接进来");
                //把与每个客户端通信的逻辑放到cilent类中进行处理
                Client client = new Socketunity.Client(clientSocket);
                clientlist.Add(client);
                //Console.WriteLine(clientlist.Count);
                //Console.WriteLine(client.GetAdress());
                clientID = client.GetAdress();
                client.SetID(clientID);
                ID.Add(clientID, client);
               
            }

        }



    }

}
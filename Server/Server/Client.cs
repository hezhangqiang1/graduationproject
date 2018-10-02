/* 
 * 服务器端：客户端
 * 脚本功能：处理单个连接的客户端
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ServerCommon;

namespace Socketunity
{
    /// <summary>
    /// 新建类用来跟客户端做通信
    /// </summary>
    class Client
    {
        private Socket clientSocket;
        private byte[] data = new byte[1024];
        private byte[] senddata = new byte[1024];
        //private List<string > playerlist = new List<string>();
        public string message;
        public string CurrentPlayer;
        public ClientState mClientState;
        private string userdata;                           //用户数据
        private List<string> IsloganClient;


        #region 同步接收调用的方法
        ////创建一个线程
        //private Thread t;

        //public Client(Socket s)
        //{
        //    clientSocket = s;
        //    //启动一个线程，处理数据的接收
        //    t = new Thread(ReceiveMessage);
        //    t.Start();
        //}
        ///// <summary>
        ///// 接收客户端数据的方法
        ///// </summary>
        //private void ReceiveMessage()
        //{
        //    //一直接收客户端数据
        //    while (true)
        //    {
        //        //在接收数据之前判断Socket连接是否断开
        //        if (clientSocket.Poll(10, SelectMode.SelectRead))
        //        {
        //            clientSocket.Close();
        //            break;
        //        }
        //        int length = clientSocket.Receive(data);
        //        string message = Encoding.UTF8.GetString(data, 0, length);
        //        //收到数据的时候把数据分发到客户端，广播消息
        //        Program.BroadcastMessage(message);
        //        Console.WriteLine("收到消息：" + message);
        //    }
        //}
        #endregion

        #region 异步接收消息方式
        public Client(Socket s)
        {
            clientSocket = s;
            clientSocket.BeginReceive(data, 0, data.Length, SocketFlags.None, Ireceieve, clientSocket);
            mClientState = ClientState.None;
            IsloganClient = new List<string>() ;
        }

        /// <summary>
        /// 接收消息的回调函数
        /// </summary>
        /// <param name="result"></param>
        public void Ireceieve(IAsyncResult result)
        {
            if (clientSocket.Poll(10, SelectMode.SelectRead))
            {
                clientSocket.Close();
                return;
            }
            int count = clientSocket.EndReceive(result);
            message = Encoding.UTF8.GetString(data, 0, count);
            ReplyClient(message);
            Program.BroadcastMessage(message);
   
            //Console.WriteLine("收到客户端消息" + message);
            clientSocket.Send(Encoding.UTF8.GetBytes(message));
            //string[] str = StringSplit(message, "-");
            //if (str[1] == "Pos")
            //{
            // Console.WriteLine("角色ID"+str[0]+"角色位置" + str[2] + str[3] + str[4]);
            //}
            clientSocket.BeginReceive(data, 0, data.Length, SocketFlags.None, Ireceieve, clientSocket);
        }
        #endregion
        /// <summary>
        /// 向客户端发送消息
        /// </summary>
        /// <param name="message"></param>
        public void SendMessage(string message)
        {
            senddata = Encoding.UTF8.GetBytes(message);
            clientSocket.Send(senddata);
            //Console.WriteLine("向客户端发送消息" + message);
        }
        /// <summary>
        /// 统一回应客户端
        /// </summary>
        /// <param name="message"></param>
        public void ReplyClient(string message)
        {
            ReplyLogin(message);
            ReplyRegister(message);
            ReplyStartFight(message);
            JudgeFight(message);
        }
        /// <summary>
        /// 回应客户端是否可以登录
        /// </summary>
        /// <param name="LoginMessage">客户端数据</param>
        public void ReplyLogin(string LoginMessage)
        {
            if (Common.StringSplit(LoginMessage, "/")[0] == "请求登录")
            {
                bool IsLogin = Login.IsAllowLogin(Common.StringSplit(LoginMessage, "/")[1], Common.StringSplit(LoginMessage, "/")[2]);
                string Reply = "";
                if (IsLogin)
                {
                    if (IsloganClient.Count > 0)
                    {
                        foreach (var item in IsloganClient)
                        {
                            if (item != Common.StringSplit(LoginMessage, "/")[1])
                            {
                                Reply = "登录成功";
                            }
                            else
                            {
                                Reply = "当前用户在线";
                            }
                           
                        }
                    }
                    else
                    {
                        Reply = "登录成功";
                    }
            
                    IsloganClient.Add(Common.StringSplit(LoginMessage, "/")[1]);

                    foreach (var data in DBData.Dicdbdata)
                    {
                        if(data.Key== Common.StringSplit(LoginMessage, "/")[1])
                        {
                            userdata = data.Value;
                            SendMessage(userdata);
                        }
                    }
                }
                else
                {
                    Reply = "登录失败";
                }
                SendMessage(Reply);
            }
        }
        /// <summary>
        /// 回应客户端注册操作
        /// </summary>
        /// <param name="Register"></param>
        public void ReplyRegister(string RegisterMessage)
        {
            if (Common.StringSplit(RegisterMessage, "/")[0] == "请求注册")
            {
                
                if (Login.IsInsert(Common.StringSplit(RegisterMessage, "/")[1]))
                {
                    DBData.Insert(Common.StringSplit(RegisterMessage, "/")[1],
                        Common.StringSplit(RegisterMessage, "/")[2], 1000, 800, 300);
                    DBData.Dicdbdata.Add(Common.StringSplit(RegisterMessage, "/")[1], "/"+Common.StringSplit(RegisterMessage, "/")[1]+"/"
                        + Common.StringSplit(RegisterMessage, "/")[2]);
                    SendMessage("注册成功");
                }
                else
                {
                    SendMessage("注册失败");
                }
            }
        }
        /// <summary>
        /// 回应客户端开始匹配
        /// </summary>
        /// <param name="StartMessage"></param>
        public void ReplyStartFight(string StartMessage)
        {
            if (Common.StringSplit(StartMessage, "/")[0] == "请求匹配")
            {    
                CurrentPlayer = Common.StringSplit(StartMessage, "/")[1];
                if (mClientState != ClientState.Fighting)
                {
                    mClientState = ClientState.StateFight;
                    Program.StartFighting();
                   
                }
              
                //playerlist.Add(CurrentPlayer);
                //Console.WriteLine(playerlist.Count.ToString()+CurrentPlayer);
                //if(playerlist.Count > 2)
                //{
                //    if (playerlist[0] != playerlist[1])
                //    {
                //        Program.BroadcastMessage("开始游戏");
                //    }
                //}

            }
        }
        /// <summary>
        /// 战斗结算
        /// </summary>
        /// <param name="FightMessage"></param>
        public void JudgeFight(string FightMessage)
        {
            if(Common.StringSplit(FightMessage, "/")[1] == "Pos")
            {
                if (float.Parse( Common.StringSplit(FightMessage, "/")[11]) <= 0)
                {
                    Console.WriteLine(Common.StringSplit(FightMessage, "/")[0] + "WIN");
                    Program.BroadcastMessage("战斗结算/"+Common.StringSplit(FightMessage, "/")[0] + "/" + "WIN");
                }
            }
        }
        /// <summary>
        /// 给客户端标记ID
        /// </summary>
        /// <param name="id"></param>
        public void SetID(string id)
        {
            byte[] ID = Encoding.UTF8.GetBytes(id);
            clientSocket.Send(ID);
        }
        //判断是否还在连接
        public bool Connected
        {
            get { return clientSocket.Connected; }
        }

        /// <summary>
        /// 获取客户端ID
        /// </summary>
        public string GetAdress()
        {
            if (!Connected)
                return "无法获取地址";
            return clientSocket.RemoteEndPoint.ToString();
        }


    }

}
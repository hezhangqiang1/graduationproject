/*
 * 脚本功能：处理客户端登录注册请求
 * 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerCommon;

namespace Socketunity
{
    class Login
    {
        
        /// <summary>
        /// 判断是否可以登录
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        public static bool IsAllowLogin(string username,string password)
        {
            foreach (var item in DBData.Dicdbdata )
            {
                if(username==item.Key&&username==Common.StringSplit(item.Value,"/")[1]&&
                    password== Common.StringSplit(item.Value, "/")[2])
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 判断用户名是否存在
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public static bool IsInsert(string username)
        {
            foreach (var item in DBData.Dicdbdata)
            {
                if (username == item.Key)
                {
                    return false;
                }
            }
            return true;
        }
    }
}

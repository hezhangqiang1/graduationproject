/* 
 * 服务器端：枚举
 * 脚本功能：定义客户端的状态
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socketunity
{
        /// <summary>
        /// 匹配状态枚举
        /// </summary>
        public enum ClientState
        {
            None,
            StateFight,
            Fighting 

        }        
}

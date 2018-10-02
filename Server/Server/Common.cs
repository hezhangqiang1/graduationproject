/* 
 * 服务器端：公共层
 * 脚本功能：需要多次使用的公共方法
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCommon
{
    class Common
    {
        /// <summary>
        /// 分割字符串
        /// </summary>
        /// <param name="s">要分割的字符串</param>
        /// <param name="c">分割条件</param>
        /// <returns></returns>
        public static string[] StringSplit(string s, string c)
        {
            string[] strArr = s.Split(Convert.ToChar(c));
            return strArr;
        }
    }
}

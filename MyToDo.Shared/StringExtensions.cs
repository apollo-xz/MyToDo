using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MyToDo.Shared
{
    public static class StringExtensions
    {
        public static string GetMD5(this string data)
        {
            if (string.IsNullOrWhiteSpace(data))
                // nameof 表达式可生成变量、类型或成员的名称作为字符串常量
                throw new ArgumentNullException(nameof(data));

            var hash = MD5.Create().ComputeHash(Encoding.Default.GetBytes(data));
            return Convert.ToBase64String(hash);//将加密后的字节数组转换为加密字符串
        }
    }
}

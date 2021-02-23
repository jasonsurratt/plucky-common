using System;
using System.Security.Cryptography;
using System.Text;

namespace Plucky.Common
{
    public static class HashUtils
    {
        static SHA1 sha1 = new SHA1CryptoServiceProvider();

        public static long ToLong(string str)
        {
            var bytes = Encoding.UTF8.GetBytes(str);

            // just use the first 8 bytes for the hash. We'll assume there aren't any hash
            // collisions. (fingers crossed!)
            return Math.Abs(BitConverter.ToInt64(sha1.ComputeHash(bytes), 0));
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FHSDK.Services
{
    class HashService : IHashService
    {
        public HashService()
        {
        }

        public string GenerateSHA1Hash(string str)
        {
            SHA1Managed sha1 = new SHA1Managed();
            var hash = sha1.ComputeHash(StringToAscii(str));
            string hex = BitConverter.ToString(hash);
            return hex.Replace("-", "").ToLower();
        }

        public static byte[] StringToAscii(string s)
        {
            byte[] retval = new byte[s.Length];
            for (int ix = 0; ix < s.Length; ++ix)
            {
                char ch = s[ix];
                if (ch <= 0x7f) retval[ix] = (byte)ch;
                else retval[ix] = (byte)'?';
            }
            return retval;
        }
    }
}

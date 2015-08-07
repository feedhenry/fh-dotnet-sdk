using System;
using System.Security.Cryptography;

namespace FHSDK.Services.Hash
{
    internal class HashService : IHashService
    {
        public string GenerateSha1Hash(string str)
        {
            var sha1 = new SHA1Managed();
            var hash = sha1.ComputeHash(StringToAscii(str));
            var hex = BitConverter.ToString(hash);
            return hex.Replace("-", "").ToLower();
        }

        public static byte[] StringToAscii(string s)
        {
            var retval = new byte[s.Length];
            for (var ix = 0; ix < s.Length; ++ix)
            {
                var ch = s[ix];
                if (ch <= 0x7f) retval[ix] = (byte) ch;
                else retval[ix] = (byte) '?';
            }
            return retval;
        }
    }
}
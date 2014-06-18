using System;
using System.Security.Cryptography;

namespace FHSDK.Services
{
    public class HashService : IHashService
    {
        public HashService()
        {
        }

        public string GenerateSHA1Hash(string str)
        {
            SHA1Managed sha1 = new SHA1Managed();
            var hash = sha1.ComputeHash(System.Text.Encoding.ASCII.GetBytes(str));
            string hex = BitConverter.ToString(hash);
            return hex.Replace("-", "");
        }
    }
}


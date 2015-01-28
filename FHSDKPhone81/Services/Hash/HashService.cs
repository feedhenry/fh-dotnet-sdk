using FHSDK.Services;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;

namespace FHSDK81.Services
{
    class HashService : IHashService
    {
        public HashService()
        {
        }

        public string GenerateSHA1Hash(string str)
        {
            HashAlgorithmProvider hashProvider = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha1);
            IBuffer hash = hashProvider.HashData(CryptographicBuffer.ConvertStringToBinary(str, BinaryStringEncoding.Utf8));
            return CryptographicBuffer.EncodeToBase64String(hash);
        }
    }
}

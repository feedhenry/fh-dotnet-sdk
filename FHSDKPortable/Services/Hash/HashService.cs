using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;

namespace FHSDK.Services.Hash
{
    internal class HashService : IHashService
    {
        public string GenerateSha1Hash(string str)
        {
            var hashProvider = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha1);
            var hash = hashProvider.HashData(CryptographicBuffer.ConvertStringToBinary(str, BinaryStringEncoding.Utf8));
            return CryptographicBuffer.EncodeToHexString(hash);
        }
    }
}
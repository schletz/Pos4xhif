using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace StoreManager.Application.Infrastructure
{
    public class CryptService : ICryptService
    {
        public string GenerateHash(string key, string data) =>
            GenerateHash(Encoding.UTF8.GetBytes(key), data);
        public string GenerateHash(byte[] key, string data) =>
            GenerateHash(key, Encoding.UTF8.GetBytes(data));
        public string GenerateHash(byte[] key, byte[] data)
        {
            using var hmac = new HMACSHA512(key);
            var hash = hmac.ComputeHash(data);
            return Convert.ToBase64String(hash);
        }
        public string GenerateSecret(int bits = 256)
        {
            var rnd = RandomNumberGenerator.GetBytes(bits / 8);
            return Convert.ToBase64String(rnd);
        }
    }
}

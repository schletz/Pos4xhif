namespace StoreManager.Application.Infrastructure
{
    public interface ICryptService
    {
        string GenerateHash(byte[] key, byte[] data);
        string GenerateHash(byte[] key, string data);
        string GenerateHash(string key, string data);
        string GenerateSecret(int bits = 256);
    }
}

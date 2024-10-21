using System.Security.Cryptography;

namespace HighscoreClient;

public class EncryptionHandler
{
    public byte[] GenerateEncryptionKey()
    {
            byte[] key = new byte[32]; // 256-bit key
            RandomNumberGenerator.Fill(key);
            return key;
    }

    public byte[] Encrypt(byte[] data, byte[] key)
    {
        using (var aes = Aes.Create())
        {
            aes.Key = key;
            aes.GenerateIV();
            
            // Check if IV is set
            if (aes.IV == null)
            {
                throw new InvalidOperationException("IV not generated.");
            }

            using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
            using (var ms = new MemoryStream())
            {
                ms.Write(aes.IV, 0, aes.IV.Length);
                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                using (var bw = new BinaryWriter(cs))
                {
                    bw.Write(data);
                }
                return ms.ToArray();
            }
        }
    }

    public byte[] Decrypt(byte[] data, byte[] key)
    {
        using (var aes = Aes.Create())
        {
            aes.Key = key;
            using (var ms = new MemoryStream(data))
            {
                byte[] iv = new byte[aes.BlockSize / 8];
                ms.Read(iv, 0, iv.Length);
                aes.IV = iv;
                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (var br = new BinaryReader(cs))
                {
                    return br.ReadBytes(data.Length - iv.Length);
                }
            }
        }
    }
}
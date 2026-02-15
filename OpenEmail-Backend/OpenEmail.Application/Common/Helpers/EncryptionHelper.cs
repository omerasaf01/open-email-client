using System.Security.Cryptography;
using System.Text;

namespace OpenEmail.Application.Common.Helpers;

public class EncryptionHelper
{
    private static readonly byte[] Key = GetEnvironmentVariable("AES_KEY");
    private static readonly byte[] IV = GetEnvironmentVariable("AES_IV");
    
    private static byte[] GetEnvironmentVariable(string key)
    {
        var value = Environment.GetEnvironmentVariable(key);
        ArgumentNullException.ThrowIfNull(value);
        
        return Encoding.UTF8.GetBytes(value);
    }
    
    public static string Encrypt(string plainText)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(plainText);
        using var aes = Aes.Create();
        aes.Key = Key;
        aes.IV = IV;

        using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        using var memoryStream = new MemoryStream();
        using var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
        using var streamWriter = new StreamWriter(cryptoStream);
        streamWriter.Write(plainText);
        
        return Convert.ToBase64String(memoryStream.ToArray());
    }
    
    public static string Decrypt(string cipherText)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(cipherText);
        using var aes = Aes.Create();
        aes.Key = Key;
        aes.IV = IV;

        using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        using var ms = new MemoryStream(Convert.FromBase64String(cipherText));
        using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
        using var sr = new StreamReader(cs);
        
        return sr.ReadToEnd();
    }
}
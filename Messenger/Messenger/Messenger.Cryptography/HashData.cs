using CryptoHelper;

namespace Messenger.Cryptography;

public static class HashData
{
    public static string EncryptData(string data) =>
        Crypto.HashPassword(data);

    public static bool VerifyData(string encryptedData, string data) =>
        Crypto.VerifyHashedPassword(encryptedData, data);
}
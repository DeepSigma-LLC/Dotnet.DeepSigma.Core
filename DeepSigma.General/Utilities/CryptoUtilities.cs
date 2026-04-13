
using DeepSigma.Core.Encode;
using System.Security.Cryptography;

namespace DeepSigma.Core.Utilities;

/// <summary>
/// Provides utility methods for cryptographic operations such as RSA and AES encryption/decryption, key generation, and digital signatures.
/// </summary>
public static class CryptoUtilities
{
    /// <summary>
    /// Generates a new RSA key pair.
    /// </summary>
    /// <param name="keySize"></param>
    /// <returns></returns>
    public static (RSAParameters publicKey, RSAParameters privateKey) GenerateRSAKeys(int keySize = 2048)
    {
        using (RSA rsa = RSA.Create(keySize))
        {
            return (rsa.ExportParameters(false), rsa.ExportParameters(true));
        }
    }

    /// <summary>
    /// Encrypts the given plain text using the provided RSA public key.
    /// </summary>
    /// <param name="plainText"></param>
    /// <param name="publicKey"></param>
    /// <param name="text_encoding_type"></param>
    /// <returns></returns>
    public static byte[] RSAEncrypt(string plainText, RSAParameters publicKey, EncodingType text_encoding_type = EncodingType.UTF8)
    {
        using RSA rsa = RSA.Create();
        rsa.ImportParameters(publicKey);
        byte[] plainBytes = Encoder.DecodeFromString(plainText, text_encoding_type);
        byte[] encryptedBytes = rsa.Encrypt(plainBytes, RSAEncryptionPadding.OaepSHA256);
        return encryptedBytes;
    }

    /// <summary>
    /// Decrypts the given encrypted text using the provided RSA private key.
    /// </summary>
    /// <param name="encryptedBytes"></param>
    /// <param name="privateKey"></param>
    /// <returns></returns>
    public static byte[] RSADecrypt(byte[] encryptedBytes, RSAParameters privateKey)
    {
        using RSA rsa = RSA.Create();
        rsa.ImportParameters(privateKey);

        byte[] decryptedBytes = rsa.Decrypt(encryptedBytes, RSAEncryptionPadding.OaepSHA256);
        return decryptedBytes;
    }

    /// <summary>
    /// Encrypts the given plain text using the provided AES key and IV.
    /// </summary>
    /// <param name="plain_text"></param>
    /// <param name="key"></param>
    /// <param name="iv"></param>
    /// <returns></returns>
    public static byte[] AESEncrypt(string plain_text, byte[] key, byte[] iv)
    {
        using Aes aes = Aes.Create();
        aes.Key = key;
        aes.IV = iv;

        ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        using MemoryStream ms = new();
        using CryptoStream cs = new(ms, encryptor, CryptoStreamMode.Write);
        using StreamWriter sw = new(cs);
        sw.Write(plain_text);
        sw.Flush();
        cs.FlushFinalBlock();
        return ms.ToArray();
    }

    /// <summary>
    /// Decrypts the given cipher text using the provided AES key and IV.
    /// </summary>
    /// <param name="cipher_text"></param>
    /// <param name="key"></param>
    /// <param name="iv"></param>
    /// <returns></returns>
    public static string AESDecrypt(byte[] cipher_text, byte[] key, byte[] iv)
    {
        using Aes aes = Aes.Create();
        aes.Key = key;
        aes.IV = iv;

        ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        using MemoryStream ms = new(cipher_text);
        using CryptoStream cs = new(ms, decryptor, CryptoStreamMode.Read);
        using StreamReader sr = new(cs);
        return sr.ReadToEnd();
    }


    /// <summary>
    /// Generates a new AES key and IV.
    /// </summary>
    /// <param name="keySize"> Key size in bytes (16, 24, or 32 for AES-128, AES-192, or AES-256)</param>
    /// <param name="ivSize"> IV size in bytes (should be 16 for AES)</param>
    /// <returns></returns>
    public static (byte[] key, byte[] iv) GenerateAESKeyAndIV(int keySize = 32, int ivSize = 16)
    {
        using Aes aes = Aes.Create();
        aes.KeySize = keySize * 8; // bits
        aes.BlockSize = ivSize * 8; // bits
        aes.GenerateKey();
        aes.GenerateIV();
        return (aes.Key, aes.IV);
    }

    /// <summary>
    /// Signs the given data using the provided ECDsa private key.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="privateKey"></param>
    /// <param name="encodingType"></param>
    /// <returns></returns>
    public static string EllipticCurveDigitalSignData(string data, ECDsa privateKey, EncodingType encodingType = EncodingType.Base64)
    {
        byte[] dataBytes = System.Text.Encoding.UTF8.GetBytes(data);
        byte[] signature = privateKey.SignData(dataBytes, HashAlgorithmName.SHA256);
        return Encoder.EncodeToString(signature, encodingType);
    }

    /// <summary>
    /// Signs the given data using the provided ECDsa private key.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="privateKey"></param>
    /// <returns></returns>
    public static byte[] EllipticCurveDigitalSignData(byte[] data, ECDsa privateKey)
    {
        byte[] signed_data = privateKey.SignData(data, HashAlgorithmName.SHA256);
        return signed_data;
    }

    /// <summary>
    /// Verifies the given signature for the data using the provided ECDsa public key.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="signature"></param>
    /// <param name="publicKey"></param>
    /// <param name="signiture_encoding_type"></param>
    /// <returns></returns>
    public static bool EllipticCurveDigitalVerifyData(string data, string signature, ECDsa publicKey, EncodingType signiture_encoding_type = EncodingType.Base64)
    {
        byte[] dataBytes = System.Text.Encoding.UTF8.GetBytes(data);
        byte[] signatureBytes = Encoder.DecodeFromString(signature, signiture_encoding_type);
        return publicKey.VerifyData(dataBytes, signatureBytes, HashAlgorithmName.SHA256);
    }

    /// <summary>
    /// Verifies the given signature for the data using the provided ECDsa public key.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="signature"></param>
    /// <param name="publicKey"></param>
    /// <param name="hash_algorithm"></param>
    /// <returns></returns>
    public static bool EllipticCurveDigitalVerifyData(byte[] data, byte[] signature, ECDsa publicKey, HashAlgorithmName hash_algorithm)
    {
        return publicKey.VerifyData(data, signature, hash_algorithm);
    }

    /// <summary>
    /// Generates a new ECDsa key pair.
    /// </summary>
    /// <returns></returns>
    public static (ECDsa publicKey, ECDsa privateKey) GenerateECDsaKeys()
    {
        ECDsa privateKey = ECDsa.Create(ECCurve.NamedCurves.nistP256);
        ECDsa publicKey = ECDsa.Create(privateKey.ExportParameters(false));
        return (publicKey, privateKey);
    }
}

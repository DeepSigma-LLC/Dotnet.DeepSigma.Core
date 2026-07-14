using DeepSigma.Core.Encode;
using DeepSigma.Core.Extensions;
using System.Security.Cryptography;

namespace DeepSigma.Core.Cryptography.Symmetric;

/// <summary>
/// Provides methods for symmetric encryption and decryption using AES (Advanced Encryption Standard).
/// </summary>
public static class AesCryptography
{
    // Tune this: higher = slower for you AND for an attacker guessing the password.
    private const int Pbkdf2Iterations = 600_000;

    /// <summary>
    /// Generates a new AES key. Store this; reuse it.
    /// </summary>
    public static byte[] GenerateAESKey(int keySizeInBytes = 32)
    {
        if (keySizeInBytes is not (16 or 24 or 32))
            throw new ArgumentOutOfRangeException(nameof(keySizeInBytes), "Must be 16, 24, or 32.");

        // Generate a random AES key. Yes, this is a secure way to generate a key.
        // We can use RNGCryptoServiceProvider or RandomNumberGenerator to generate a secure random key simply by entropy. This is a standard practice in cryptography.
        // RandomNumberGenerator.GetBytes pulls from the OS CSPRNG (BCryptGenRandom on Windows, getrandom on Linux). These are considered secure sources of randomness.
        // Unlike using a password, System.Random, or a predictable seed. 
        return RandomNumberGenerator.GetBytes(keySizeInBytes);
    }

    /// <summary>
    /// Encrypts the given plain text using the provided AES key and IV.
    /// </summary>
    /// <param name="plain_text"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public static byte[] AESEncrypt(string plain_text, byte[] key) => AESEncrypt(Encoder.DecodeFromString(plain_text, EncodingType.UTF8), key);

    /// <summary>
    /// Encrypts the given plain text bytes using the provided AES key and IV.
    /// </summary>
    /// <param name="cipher_text">The encrypted data to decrypt.</param>
    /// <param name="key">The AES key to use for decryption.</param>
    /// <returns>The decrypted data as a string.</returns>
    public static string AESDecryptToString(byte[] cipher_text, byte[] key) => Encoder.EncodeToString(AESDecrypt(cipher_text, key), EncodingType.UTF8);

    /// <summary>
    /// Encrypts the given data using the provided AES key and IV. Note: In this implementation, you must generate a new IV for each encryption operation and store it alongside the ciphertext for decryption.
    /// </summary>
    /// <remarks>
    /// Note: Do not reuse initialization vectors (IVs) for multiple encryption operations. Prefer to use AESEncrypt(string plain_text, byte[] key) instead.
    /// Initialization vectors (IVs) should be unique and random for each encryption operation to ensure security.
    /// The IV is typically prepended to the ciphertext for use during decryption. They are not secret and can be safely stored or transmitted alongside the ciphertext.
    /// Rotating IVs ensures that the same plaintext encrypted multiple times with the same key will produce different ciphertexts and helps prevent certain types of attacks, such as replay attacks and pattern analysis.
    /// </remarks>
    /// <param name="data">The data to encrypt.</param>
    /// <param name="key">The AES key to use for encryption.</param>
    /// <param name="iv">The initialization vector (IV) to use for encryption.</param>
    /// <returns>The encrypted data as a byte array.</returns>
    public static byte[] AESEncrypt(byte[] data, byte[] key, byte[] iv)
    {
        using Aes aes = Aes.Create();
        aes.Key = key;
        return aes.EncryptCbc(data, iv);   // CBC + PKCS7 by default
    }

    /// <summary>
    /// Decrypts the given encrypted data using the provided AES key.
    /// </summary>
    /// <param name="data">The encrypted data to decrypt.</param>
    /// <param name="key">The AES key to use for decryption.</param>
    /// <returns>The decrypted data as a byte array. The result includes the IV prepended to the ciphertext.</returns>
    public static byte[] AESEncrypt(byte[] data, byte[] key)
    {
        byte[] iv = RandomNumberGenerator.GetBytes(16);   // fresh, every call

        using Aes aes = Aes.Create();
        aes.Key = key;
        byte[] cipher = aes.EncryptCbc(data, iv);

        byte[] result = new byte[iv.Length + cipher.Length];
        iv.CopyTo(result, 0);
        cipher.CopyTo(result, iv.Length);
        return result;   // [IV][ciphertext]
    }

    /// <summary>
    /// Decrypts the given encrypted data using the provided AES key.
    /// </summary>
    /// <param name="payload">The encrypted data to decrypt.</param>
    /// <param name="key">The AES key to use for decryption.</param>
    /// <returns>The decrypted data as a byte array. The result includes the IV prepended to the ciphertext.</returns>
    public static byte[] AESDecrypt(byte[] payload, byte[] key)
    {
        byte[] iv = payload[..16];
        byte[] cipher = payload[16..];

        using Aes aes = Aes.Create();
        aes.Key = key;
        return aes.DecryptCbc(cipher, iv);
    }

    /// <summary>
    /// Decrypts the given encrypted data using the provided AES key and IV.
    /// </summary>
    /// <param name="cipher_bytes">The encrypted data to decrypt.</param>
    /// <param name="key">The AES key to use for decryption.</param>
    /// <param name="iv">The initialization vector (IV) to use for decryption.</param>
    /// <returns>The decrypted data as a byte array.</returns>
    public static byte[] AESDecrypt(byte[] cipher_bytes, byte[] key, byte[] iv)
    {
        using Aes aes = Aes.Create();
        aes.Key = key;
        return aes.DecryptCbc(cipher_bytes, iv);
    }


    /// <summary>
    /// Encrypts the given data using the provided AES key and IV, and returns the result as a string using the specified text encoding type.
    /// </summary>
    /// <param name="data">The data to encrypt.</param>
    /// <param name="key">The AES key to use for encryption.</param>
    /// <param name="nonce">The nonce (number used once) to use for encryption.</param>
    /// <returns>A tuple containing the encrypted data and the authentication tag.</returns>
    public static (byte[] cipher, byte[] tag) AesGcmEncrypt(byte[] data, byte[] key, byte[] nonce)
    {
        using var aes = new AesGcm(key, AesGcm.TagByteSizes.MaxSize);
        var cipher = new byte[data.Length];
        var tag = new byte[AesGcm.TagByteSizes.MaxSize];
        aes.Encrypt(nonce, data, cipher, tag);
        return (cipher, tag);
    }

    /// <summary>
    /// Decrypts the given encrypted data using the provided AES key, nonce, and authentication tag, and returns the decrypted data as a byte array.
    /// </summary>
    /// <param name="cipher">The encrypted data to decrypt.</param>
    /// <param name="tag">The authentication tag associated with the encrypted data.</param>
    /// <param name="key">The AES key to use for decryption.</param>
    /// <param name="nonce">The nonce (number used once) to use for decryption.</param>
    /// <returns>The decrypted data as a byte array.</returns>
    public static byte[] AesGcmDecrypt(byte[] cipher, byte[] tag, byte[] key, byte[] nonce)
    {
        using var aes = new AesGcm(key, AesGcm.TagByteSizes.MaxSize);
        var plain = new byte[cipher.Length];
        aes.Decrypt(nonce, cipher, tag, plain);   // throws if tampered
        return plain;
    }

    /// <summary>
    /// Generates a new AES key file at the specified path with the given key and IV sizes.
    /// </summary>
    /// <remarks>
    /// Important: Do not share the generated AES key file with anyone. It is crucial for the security of your key chain. Keep it in a secure location and ensure that only authorized personnel have access to it.
    /// </remarks>
    /// <param name="aes_key_json_file_path">The file path where the AES key file will be created.</param>
    /// <param name="keySizeInBytes">The size of the AES key in bytes.</param>
    /// <param name="textEncodingType">The encoding type to use for the AES key file.</param>
    /// <param name="id">The unique identifier for the AES key file.</param>
    /// <returns>An exception if an error occurs, otherwise null.</returns>
    public static Exception? GenerateAesKeyFile(string aes_key_json_file_path, int keySizeInBytes = 32, EncodingType textEncodingType = EncodingType.Base64, string? id = null)
    {
        if (string.IsNullOrWhiteSpace(aes_key_json_file_path))
        {
            return new Exception($"AES key file path is null or empty: {nameof(aes_key_json_file_path)}");
        }
        if (File.Exists(aes_key_json_file_path))
        {
            return new Exception($"AES key file already exists: {aes_key_json_file_path}");
        }

        byte[] aesKey = GenerateAESKey(keySizeInBytes);
        string encoded_key = Encoder.EncodeToString(aesKey, textEncodingType);
        AesKeyFile aesKeyFile = new(encoded_key, new AesKeyFileMetadata(keySizeInBytes, textEncodingType, textEncodingType.ToDescriptionString(), "1.0.0", id ?? Guid.NewGuidTimeOrdered().ToString()));

        string json = System.Text.Json.JsonSerializer.Serialize(aesKeyFile);
        File.WriteAllText(aes_key_json_file_path, json);
        return null;
    }

    /// <summary>
    /// Reads the AES key and IV from the specified key file.
    /// </summary>
    /// <param name="jsonFilePath">The json file path of the AES key file.</param>
    /// <returns>A tuple containing an exception if an error occurs, the AES key, and the IV.</returns>
    public static (Exception? error, byte[]? aesKey, EncodingType? encodingType) GetAesKeyFromFile(string jsonFilePath)
    {
        if (jsonFilePath.IsNullOrEmpty() || Path.Exists(jsonFilePath) == false)
        {
            return (new Exception($"AES key file path does not exist: {jsonFilePath}"), null, null);
        }

        string combinedKeyAndIv = File.ReadAllText(jsonFilePath);

        AesKeyFile? aesKeyFile = System.Text.Json.JsonSerializer.Deserialize<AesKeyFile>(combinedKeyAndIv);

        if (aesKeyFile == null)
        {
            return (new Exception($"Failed to deserialize AES key file: {jsonFilePath}"), null, null);
        }

        byte[] aesKey = Encoder.DecodeFromString(aesKeyFile.Key, aesKeyFile.Metadata.TextEncodingType);
        return (null, aesKey, aesKeyFile.Metadata.TextEncodingType);
    }
}

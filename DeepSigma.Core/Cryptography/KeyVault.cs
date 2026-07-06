using DeepSigma.Core.Encode;
using DeepSigma.Core.Extensions;
using DeepSigma.Core.Serialization;
using System.Diagnostics.CodeAnalysis;

namespace DeepSigma.Core.Cryptography;

/// <summary>
/// Manages a collection of keys, potentially for API access or encryption purposes.
/// </summary>
public class KeyVault
{
    private byte[]? AesKey { get; set; }
    private byte[]? IV { get; set; }
    private static EncodingType KeyTextEncodingType { get; set; } = EncodingType.Base64;

    private readonly Dictionary<string, KeyVaultItem> Keys = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// The full file path to the file storing the key chain.
    /// </summary>
    public string? KeyChainFullFilePath { get; init; }

    /// <summary>
    /// The file path to the AES key file used for encryption and decryption of the key chain data.
    /// </summary>
    public required string AesJsonKeyFilePath { get; init; }

    /// <summary>
    /// The file path to the initialization vector (IV) file used for encryption and decryption of the key chain data.
    /// </summary>
    private bool OverwriteFiles { get; init; } = true;

    /// <summary>
    /// Initializes a new instance of the KeyChain class.
    /// </summary>
    /// <param name="aesJsonKeyFilePath">Required AES key file path for encryption/decryption</param>
    /// <param name="keychain_full_file_path">Required file path for storing the key chain</param>
    /// <param name="overwriteFiles">Indicates whether to overwrite existing files</param>
    [SetsRequiredMembers]
    public KeyVault(string aesJsonKeyFilePath, string keychain_full_file_path, EncodingType keyTextEncodingType, bool overwriteFiles)
    {
        AesJsonKeyFilePath = aesJsonKeyFilePath;
        KeyChainFullFilePath = keychain_full_file_path;
        OverwriteFiles = overwriteFiles;
        KeyTextEncodingType = keyTextEncodingType;
        Exception? aes_valid = ValidateExistingFilePath(AesJsonKeyFilePath);
        if(aes_valid != null)
        {
            throw aes_valid;
        }
        LoadAesKeyValues();

        bool keyChainFileExists = Path.Exists(keychain_full_file_path);
        if (!keyChainFileExists)
        {
            File.WriteAllText(keychain_full_file_path, string.Empty);
        }
        else
        {
            ValidateExistingFilePath(keychain_full_file_path);
            LoadKeysFromFile();
        }
    }

    private void LoadAesKeyValues()
    {
        var (error, aesKey, iv) = CryptoUtilities.GetAesKeyAndIvFromFile(AesJsonKeyFilePath, KeyTextEncodingType);
        if (error != null)
        {
            throw error; // Throwing the exception if the AES key file is invalid
        }
        AesKey = aesKey;
        IV = iv;
    }

    /// <summary>
    /// Attempts to add a new key to the key chain.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool TryToAddKey(string name, string key)
    {
        if (!Keys.ContainsKey(name))
        {
            Keys[name] = new KeyVaultItem(name, key);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Retrieves a key by its name from the key chain.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public KeyVaultItem? GetKey(string name)
    {
        if (Keys.TryGetValue(name, out var keyItem))
        {
            return keyItem;
        }
        return null;
    }

    /// <summary>
    /// Attempts to remove a key from the key chain by its name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool TryToRemoveKey(string name)
    {
        return Keys.Remove(name);
    }

    /// <summary>
    /// Retrieves all keys stored in the key chain.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<KeyVaultItem> GetAllKeys() => Keys.Values;

    /// <summary>
    /// Generates a new key chain file at the specified path with the provided keys.
    /// </summary>
    /// <param name="full_file_path"></param>
    public Exception? ExportToNewFile(string full_file_path)
    {
        var result = ValidateNewFilePath(full_file_path);
        if (result != null)
        {
            return result;
        }
        ExportToFile(full_file_path, OverwriteFiles);
        return null;
    }

    /// <summary>
    /// Saves the current key chain to the existing file path, overwriting any existing file.
    /// </summary>
    public Exception? Save()
    {
        if(KeyChainFullFilePath is null)
        {
            return new Exception("KeyChainFullFilePath is null. Cannot save key chain.");
        }

        var result = ValidateExistingFilePath(KeyChainFullFilePath);
        if (result is not null)
        {
            return result;
        }
        ExportToFile(KeyChainFullFilePath, OverwriteFiles);
        return null;    
    }

    private void ExportToFile(string full_file_path, bool overwrite)
    {
        string text = JsonSerializer.GetSerializedString(Keys);
        byte[] encrypted_data = CryptoUtilities.AESEncrypt(text, AesKey!, IV!);
        if (overwrite || !File.Exists(full_file_path))
        {
            File.WriteAllBytes(full_file_path, encrypted_data);
        }
        else
        {
            throw new Exception("File already exists and overwrite is set to false.");
        }
    }

    private Exception? LoadKeysFromFile()
    {
        if (KeyChainFullFilePath.IsNullOrEmpty())
        {
            return new Exception("KeyChainFullFilePath is null. Cannot load key chain.");
        }

        byte[] encryptedBytes = File.ReadAllBytes(KeyChainFullFilePath!);

        string json_text = CryptoUtilities.AESDecrypt(encryptedBytes, AesKey!, IV!);
        var deserialized_results = JsonSerializer.GetDeserializedObject<Dictionary<string, KeyVaultItem>>(json_text);
        deserialized_results?.ForEach(x => Keys[x.Key] = x.Value); // Loop through the deserialized results and add to dict. Maintains string comparison rules by using StringComparer.OrdinalIgnoreCase in the dictionary initialization.
        return null;
    }

    private static Exception? ValidateExistingFilePath(string full_file_path)
    {
        if (string.IsNullOrWhiteSpace(full_file_path))
        {
            return new Exception($"File path is null or empty: {nameof(full_file_path)}");
        }

        if (File.Exists(full_file_path) == false)
        {
            return new Exception($"File does not exists: {full_file_path}");

        }
        return null;
    }

    private static Exception? ValidateNewFilePath(string full_file_path)
    {
        if (string.IsNullOrWhiteSpace(full_file_path))
        {
            return new Exception($"File path is null or empty: {nameof(full_file_path)}");
        }

        if (File.Exists(full_file_path))
        {
            return new Exception($"File already exists: {full_file_path}");
        }
        return null;
    }
}

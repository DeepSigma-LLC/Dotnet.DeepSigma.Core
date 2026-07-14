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
    public string AesJsonKeyFilePath { get; init; }

    /// <summary>
    /// The file path to the initialization vector (IV) file used for encryption and decryption of the key chain data.
    /// </summary>
    private bool OverwriteFiles { get; init; } = false;

    /// <summary>
    /// Indicates whether updating existing keys in the key chain is allowed. If set to false, attempts to update an existing key will be ignored.
    /// </summary>
    private bool AllowKeyUpdate { get; init; } = false;

    /// <summary>
    /// Initializes a new instance of the KeyChain class.
    /// </summary>
    /// <param name="aesJsonKeyFilePath">Required AES key file path for encryption/decryption</param>
    /// <param name="keychain_full_file_path">Required file path for storing the key chain</param>
    /// <param name="TextEncodingType">Optional encoding type for the AES key file (default is Base64)</param>
    /// <param name="overwriteFiles">Indicates whether to overwrite existing files</param>
    public KeyVault(string aesJsonKeyFilePath, string keychain_full_file_path, EncodingType TextEncodingType = EncodingType.Base64, bool overwriteFiles = false, bool allowKeyUpdate = false)
    {
        AesJsonKeyFilePath = aesJsonKeyFilePath;
        KeyChainFullFilePath = keychain_full_file_path;
        OverwriteFiles = overwriteFiles;
        AllowKeyUpdate = allowKeyUpdate;
        Exception? aes_valid_file_path_error = ValidateExistingFilePath(AesJsonKeyFilePath);
        if(aes_valid_file_path_error is not null)
        {
            throw aes_valid_file_path_error;
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
        var (error, aesKey, encodingType) = Symmetric.AesCryptography.GetAesKeyFromFile(AesJsonKeyFilePath);
        if (error != null)
        {
            throw error; // Throwing the exception if the AES key file is invalid
        }
        AesKey = aesKey;
        KeyTextEncodingType = encodingType ?? KeyTextEncodingType;
    }

    /// <summary>
    /// Attempts to add a new key to the key chain.
    /// </summary>
    /// <param name="item">The key vault item to add.</param>
    /// <returns></returns>
    public bool TryToAddKey(KeyVaultItem item)
    {
        if (!Keys.ContainsKey(item.Name))
        {
            Keys[item.Name] = item;
            return true;
        }
        return false;
    }

    /// <inheritdoc cref="TryToAddKey(KeyVaultItem)"/>
    public bool TryToAddKey(string name, string key)
    {
        var newItem = new KeyVaultItem(name, key);
        return TryToAddKey(newItem);
    }

    /// <summary>
    /// Attempts to update an existing key in the key chain. If updating keys is not allowed, it will return false.
    /// </summary>
    /// <param name="item">The key vault item to update.</param>
    /// <returns></returns>
    public bool TryToUpdateKey(KeyVaultItem item)
    {
        if(AllowKeyUpdate == false)
        {
            return false; // Updating keys is not allowed
        }

        if (Keys.ContainsKey(item.Name))
        {
            Keys[item.Name] = item;
            return true;
        }
        return false;
    }

    /// <inheritdoc cref="TryToUpdateKey(KeyVaultItem)"/>
    public bool TryToUpdateKey(string name, string key)
    {
        var newItem = new KeyVaultItem(name, key);
        return TryToUpdateKey(newItem);
    }


    /// <summary>
    /// Attempts to add or update a key in the key chain. If updating keys is not allowed and the key already exists, it will return false.
    /// </summary>
    /// <param name="item">The key vault item to add or update.</param>
    /// <returns>True if the key was added or updated; false otherwise.</returns>
    public bool TryToUpsert(KeyVaultItem item)
    {
        if (AllowKeyUpdate == false && Keys.ContainsKey(item.Name))
        {
            return false; // Updating keys is not allowed
        }

        Keys[item.Name] = item;
        return true;
    }

    /// <inheritdoc cref="TryToUpsert(KeyVaultItem)"/>
    public bool TryToUpsert(string name, string key)
    {
        var newItem = new KeyVaultItem(name, key);
        return TryToUpsert(newItem);
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
        byte[] encrypted_data = Symmetric.AesCryptography.AESEncrypt(text, AesKey!);
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

        string json_text = Encoder.EncodeToString(Symmetric.AesCryptography.AESDecrypt(encryptedBytes, AesKey!), KeyTextEncodingType);
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

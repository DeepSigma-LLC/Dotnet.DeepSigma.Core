namespace DeepSigma.Core.Cryptography;

/// <summary>
/// Represents an AES key file containing the AES key, initialization vector (IV), and associated metadata.
/// </summary>
/// <param name="Key"></param>
/// <param name="InitializationVector"></param>
/// <param name="Metadata"></param>
public record AesKeyFile(string Key, string InitializationVector, AesKeyFileMetadata Metadata);

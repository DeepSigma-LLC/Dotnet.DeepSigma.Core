using DeepSigma.Core.Encode;

namespace DeepSigma.Core.Cryptography.Symmetric;

/// <summary>
/// Represents an AES key file containing the AES key, initialization vector (IV), and associated metadata.
/// </summary>
/// <param name="Key"></param>
/// <param name="Metadata"></param>
public record AesKeyFile(string Key, AesKeyFileMetadata Metadata)
{
    /// <summary>
    /// Gets the AES key as a byte array by decoding the string using the specified text encoding type from the metadata.
    /// </summary>
    public byte[] KeyBytes => Encoder.DecodeFromString(Key, Metadata.TextEncodingType);
}


using DeepSigma.Core.Encode;

namespace DeepSigma.Core.Cryptography;

/// <summary>
/// Represents an AES key file containing the AES key, initialization vector (IV), and associated metadata.
/// </summary>
/// <param name="Key"></param>
/// <param name="InitializationVector"></param>
/// <param name="Metadata"></param>
public record AesKeyFile(string Key, string InitializationVector, AesKeyFileMetadata Metadata)
{
    /// <summary>
    /// Gets the AES key as a byte array by decoding the string using the specified text encoding type from the metadata.
    /// </summary>
    public byte[] KeyBytes => Encoder.DecodeFromString(Key, Metadata.TextEncodingType);

    /// <summary>
    /// Gets the initialization vector (IV) as a byte array by decoding the string using the specified text encoding type from the metadata.
    /// </summary>
    public byte[] InitializationVectorBytes => Encoder.DecodeFromString(InitializationVector, Metadata.TextEncodingType);
}


using DeepSigma.Core.Encode;

namespace DeepSigma.Core.Cryptography;

/// <summary>
/// Represents metadata for an AES key file, including key size, IV size, text encoding type, and a description.
/// </summary>
/// <param name="KeySize"></param>
/// <param name="IVSize"></param>
/// <param name="TextEncodingType"></param>
/// <param name="EncodingName"></param>
/// <param name="Description"></param>
public record AesKeyFileMetadata(int KeySize, int IVSize, EncodingType TextEncodingType, string EncodingName, string Description = "AES key file")
{
    /// <summary>
    /// Gets the creation timestamp of the AES key file metadata in UTC.
    /// </summary>
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}
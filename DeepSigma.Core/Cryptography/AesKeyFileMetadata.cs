using DeepSigma.Core.Encode;

namespace DeepSigma.Core.Cryptography;

/// <summary>
/// Represents metadata for an AES key file, including key size, IV size, text encoding type, and a description.
/// </summary>
/// <param name="KeySizeInBytes">The size of the AES key in bytes.</param>
/// <param name="IVSizeInBytes">The size of the initialization vector (IV) in bytes.</param>
/// <param name="TextEncodingType">The encoding type used for the AES key file.</param>
/// <param name="EncodingName">The name of the encoding used for the AES key file.</param>
/// <param name="FileVersion">The version of the AES key file format.</param>
/// <param name="Id">The unique identifier of the AES key file.</param>
/// <param name="Description">A description of the AES key file.</param>
public record AesKeyFileMetadata(int KeySizeInBytes, int IVSizeInBytes, EncodingType TextEncodingType, string EncodingName, string FileVersion, string Id, string Description = "AES key file")
{
    /// <summary>
    /// Gets the creation timestamp of the AES key file metadata in UTC.
    /// </summary>
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}
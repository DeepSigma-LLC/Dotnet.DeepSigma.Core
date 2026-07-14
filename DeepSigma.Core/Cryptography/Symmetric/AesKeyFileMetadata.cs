using DeepSigma.Core.Encode;

namespace DeepSigma.Core.Cryptography.Symmetric;

/// <summary>
/// Represents metadata for an AES key file, including key size, IV size, text encoding type, and a description.
/// </summary>
/// <param name="KeySizeInBytes">The size of the AES key in bytes.</param>
/// <param name="TextEncodingType">The encoding type used for the AES key file.</param>
/// <param name="EncodingName">The name of the encoding used for the AES key file.</param>
/// <param name="FileVersion">The version of the AES key file format.</param>
/// <param name="Id">The unique identifier of the AES key file.</param>
/// <param name="Description">A description of the AES key file.</param>
public record AesKeyFileMetadata(int KeySizeInBytes, EncodingType TextEncodingType, string EncodingName, string FileVersion, string Id, string Description = "AES key file")
{
    /// <summary>
    /// Gets the creation timestamp of the AES key file metadata in UTC.
    /// </summary>
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// Gets the size of the initialization vector (IV) in bytes. The default IV size for AES is 16 bytes (128 bits).
    /// </summary>
    /// <remarks>
    /// Using any other size may lead to security vulnerabilities or incompatibilities with AES encryption standards. It is recommended to use the default size unless there is a specific requirement to change it.
    /// </remarks>
    public int InitializationVectorSizeInBytes { get; init; } = 16; // Default IV size for AES is 16 bytes (128 bits)
}
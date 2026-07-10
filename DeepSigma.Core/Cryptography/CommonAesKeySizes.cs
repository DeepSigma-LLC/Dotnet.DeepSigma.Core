
namespace DeepSigma.Core.Cryptography;

/// <summary>
/// Represents common AES key sizes in bytes.
/// </summary>
public enum CommonAesKeySizes
{
    /// <summary>
    /// Represents the AES-128 key size, which is 16 bytes (128 bits).
    /// </summary>
    /// <remarks>
    /// AES-128 is widely used due to its balance between security and performance. It is suitable for most applications and provides a good level of security for sensitive data.
    /// It is highly secure, exceptionally fast, and the default standard for most general computing, secure file sharing, and basic data protection.
    /// </remarks>
    AES128 = 16,
    /// <summary>
    /// Represents the AES-192 key size, which is 24 bytes (192 bits).
    /// </summary>
    /// <remarks>
    /// AES-192 is less commonly used than AES-128 and AES-256, but it provides a balance between security and performance for certain applications.
    /// It provides higher security than AES-128 but is slightly slower, often utilized in specific regulatory or industry communications.
    /// </remarks>
    AES192 = 24,
    /// <summary>
    /// Represents the AES-256 key size, which is 32 bytes (256 bits).
    /// </summary>
    /// <remarks>
    /// AES-256 is considered the most secure option among the three common AES key sizes and is widely used in various applications requiring strong encryption.
    /// It offers the highest level of security and is typically required for government, military, and top-secret data
    /// </remarks>
    AES256 = 32
}

/// <summary>
/// Provides extension methods for the CommonAesKeySizes enum.
/// </summary>
public static class CommonAesKeySizeExtensions
{
    extension(CommonAesKeySizes value)
    {
        /// <summary>
        /// Converts the specified AES key size to its corresponding byte length.
        /// </summary>
        /// <returns>The byte length corresponding to the AES key size.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public int ToEquivalentByteSize() => value switch
        {
            CommonAesKeySizes.AES128 => 16,
            CommonAesKeySizes.AES192 => 24,
            CommonAesKeySizes.AES256 => 32,
            _ => throw new ArgumentOutOfRangeException(
                nameof(value),
                value,
                "Unsupported AES key size.")
        };
    }
}
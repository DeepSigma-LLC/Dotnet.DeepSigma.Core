namespace DeepSigma.Core.Cryptography.Symmetric;


/// <summary>
/// Represents common AES initialization vector (IV) sizes in bytes.
/// </summary>
public enum AesInitializationVectorSizes
{
    /// <summary>
    /// Represents the AES initialization vector (IV) size, which is 16 bytes (128 bits).
    /// </summary>
    Default = 16, // 16 bytes (128 bits) is the standard size for AES IVs
}

/// <summary>
/// Provides extension methods for the AesInitializationVectorSizes enum.
/// </summary>
public static class AesInitializationVectorSizeExtensions
{
    extension(AesInitializationVectorSizes value)
    {
        /// <summary>
        /// Converts the specified AES IV size to its corresponding byte length.
        /// </summary>
        /// <returns>The byte length corresponding to the AES IV size.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public int ToEquivalentByteSize() => value switch
        {
            AesInitializationVectorSizes.Default => 16,
            _ => throw new ArgumentOutOfRangeException(
                nameof(value),
                value,
                "Unsupported AES IV size.")
        };
    }
}


using System.Security.Cryptography;

namespace DeepSigma.Core.Utilities;

/// <summary>
/// Provides utility methods for computing hashes using various algorithms.
/// </summary>
public static class HashUtilities
{
    /// <summary>
    /// Computes the hash of the given input string using the specified hash algorithm.
    /// </summary>
    /// <param name="input">Input string assumes UTF8 encoded string.</param>
    /// <param name="algorithm_name"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static byte[] ComputeHash(string input, HashAlgorithmName algorithm_name)
    {
        byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(input);
        return ComputeHash(inputBytes, algorithm_name);
    }

    /// <summary>
    /// Computes the hash of the given input byte array using the specified hash algorithm.
    /// </summary>
    /// <param name="input"></param>
    /// <param name="algorithm_name"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static byte[] ComputeHash(byte[] input, HashAlgorithmName algorithm_name)
    {
        HashAlgorithm hashAlgorithm = algorithm_name.Name switch
        {
            "MD5" => MD5.Create(),
            "SHA1" => SHA1.Create(),
            "SHA256" => SHA256.Create(),
            "SHA384" => SHA384.Create(),
            "SHA512" => SHA512.Create(),
            _ => throw new ArgumentException("Unsupported hash algorithm", nameof(algorithm_name)),
        };

        using (hashAlgorithm)
        {
            byte[] hashBytes = hashAlgorithm.ComputeHash(input);
            return hashBytes;
        }
    }
}

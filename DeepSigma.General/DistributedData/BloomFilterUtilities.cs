using System.Text;
using System.Security.Cryptography;

namespace DeepSigma.Core.DistributedData;

internal class BloomFilterUtilities
{
    /// <summary>
    /// Computes two 64-bit hash values derived from the SHA-256 digest of the specified string. Derive two 64-bit hashes from a single SHA-256 digest.
    /// </summary>
    /// <remarks>The returned hash values are positive and suitable for use in hash-based algorithms,
    /// such as double hashing. The second hash value (<c>hash_2</c>) is adjusted to avoid being zero, ensuring it can
    /// be used effectively in scenarios where a non-zero value is required (e.g., as a step size in probing
    /// algorithms).</remarks>
    /// <param name="s">The input string to hash. Cannot be <see langword="null"/> or empty.</param>
    /// <returns>A tuple containing two 64-bit hash values. The first value (<c>hash_1</c>) is derived from the first 64 bits of
    /// the digest,  and the second value (<c>hash_2</c>) is derived from the next 64 bits. The second value is
    /// guaranteed to be non-zero.</returns>
    internal static (ulong hash_1, ulong hash_2) HashPair(string s)
    {
        byte[] data = Encoding.UTF8.GetBytes(s);
        Span<byte> digest = stackalloc byte[32];
        using (var sha = SHA256.Create())
        {
            sha.TryComputeHash(data, digest, out _);
        }

        ulong Read64(ReadOnlySpan<byte> span, int offset)
            => BitConverter.ToUInt64(span.Slice(offset, 8));

        // Use two different 64-bit chunks to seed double hashing.
        ulong a = Read64(digest, 0);
        ulong b = Read64(digest, 8);

        // Avoid zero h2 (which would collapse the k probes to the same index).
        b = b == 0 ? 0x9E3779B97F4A7C15UL : b;

        // Make them positive modulo arithmetic by clearing the sign bit when cast to long.
        a &= 0x7FFF_FFFF_FFFF_FFFFUL;
        b &= 0x7FFF_FFFF_FFFF_FFFFUL;

        return (a, b);
    }

    internal static int GetComputedIndex(ulong hash_1, ulong hash_2, int hash_function_index, int number_of_slots_in_filter)
    {
        int result = (int)((hash_1 + (ulong)hash_function_index * hash_2) % (ulong)number_of_slots_in_filter);
        return result;
    }
}

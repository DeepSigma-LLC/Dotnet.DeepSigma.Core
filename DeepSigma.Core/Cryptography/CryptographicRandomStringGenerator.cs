
using System.Security.Cryptography;

namespace DeepSigma.Core.Cryptography;

/// <summary>
/// Generates unbaised cryptographically secure random strings.
/// </summary>
/// <remarks>
/// Why "unbiased" matters — this is the trap the hand-rolled version falls into. The obvious approach is to grab a random byte and take it mod the alphabet length:
/// With a 74 - character alphabet, 256 doesn't divide evenly by 74 — so the first 34 characters come up slightly more often than the rest. 
/// Each character leaks a fraction of a bit, which chips away at your real entropy. The fix is rejection sampling (discard values in the biased tail and redraw), and GetString does it for you.
/// </remarks>
public static class CryptographicRandomStringGenerator
{
    /// <summary>
    /// Alphabet of characters that can be used to generate random strings.
    /// </summary>
    const string Alphabet =
    "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*-_=+";

    /// <summary>
    /// Alphabet of characters that can be used to generate random strings without special characters.
    /// </summary>
    const string AlphabetNoSpecialChars =
        "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

    /// <summary>
    /// Alphabet without I, l, 1, O, 0 to avoid confusion.
    /// </summary>
    const string AlphabetWithoutHardToReadCharacters =
        "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz23456789";

    /// <summary>
    /// Generates an unbiased cryptographically secure random string of the specified length using the defined alphabet.
    /// </summary>
    /// <param name="length">The length of the random string to generate.</param>
    /// <returns>A cryptographically secure random string.</returns>
    public static string GenerateRandomString(int length)
    {
        string result = RandomNumberGenerator.GetString(Alphabet, length);
        return result;
    }

    /// <summary>
    /// Generates an unbiased cryptographically secure random string of the specified length using the defined alphabet.
    /// </summary>
    /// <param name="length">The length of the random string to generate.</param>
    /// <returns>A cryptographically secure random string.</returns>
    public static string GenerateRandomStringHumanReadable(int length)
    {
        string result = RandomNumberGenerator.GetString(AlphabetWithoutHardToReadCharacters, length);
        return result;
    }

    /// <summary>
    /// Generates an unbiased cryptographically secure random string of the specified length using the defined alphabet.
    /// </summary>
    /// <param name="length">The length of the random string to generate.</param>
    /// <param name="allowedSpecialCharacters">The special characters to include in the random string.</param>
    /// <returns>A cryptographically secure random string.</returns>
    public static string GenerateRandomString(int length, string? allowedSpecialCharacters = null)
    {
        string alphabet = AlphabetNoSpecialChars + (allowedSpecialCharacters ?? string.Empty);
        string result = RandomNumberGenerator.GetString(alphabet, length);
        return result;
    }

    /// <summary>
    /// Generates an unbiased cryptographically secure random string of the specified length using the defined alphabet.
    /// </summary>
    /// <param name="length">The length of the random string to generate.</param>
    /// <param name="alphabet">The alphabet to use for generating the random string.</param>
    /// <returns>A cryptographically secure random string.</returns>
    public static string GenerateRandomStringCustomAlphabet(int length, string alphabet)
    {
        string result = RandomNumberGenerator.GetString(alphabet, length);
        return result;
    }
}

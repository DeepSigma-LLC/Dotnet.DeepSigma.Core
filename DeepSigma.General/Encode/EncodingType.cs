
namespace DeepSigma.Core.Encode;

/// <summary>
/// Specifies the type of encoding to be used.
/// </summary>
public enum EncodingType
{
    /// <summary>
    /// Hexadecimal encoding. Represents binary data as a string of hexadecimal digits (0-9, A-F).
    /// </summary>
    Hex,
    /// <summary>
    /// Base64 encoding. The most common encoding scheme that represents binary data in an ASCII string format by translating it into a radix-64 representation.
    /// </summary>
    Base64,
    /// <summary>
    /// UTF-8 encoding. The most widely used encoding for Unicode characters. Represents text as a sequence of bytes using the UTF-8 encoding scheme.
    /// </summary>
    UTF8,
    /// <summary>
    /// UTF-32 encoding. The encoding that uses four bytes for each character. Represents text as a sequence of bytes using the UTF-32 encoding scheme.
    /// </summary>
    UTF32,
    /// <summary>
    /// ASCII encoding. The encoding that represents characters using a single byte for each character. Represents text as a sequence of bytes using the ASCII encoding scheme.
    /// </summary>
    ASCII,
    /// <summary>
    /// Base58 encoding. Includes characters from the Bitcoin Base58 alphabet. Excludes characters that can be easily confused, such as '0' (zero), 'O' (capital o), 'I' (capital i), and 'l' (lowercase L).
    /// </summary>
    Base58,
    /// <summary>
    /// Base32 encoding. Encodes binary data into a text representation using a set of 32 different ASCII characters.
    /// </summary>
    Base32
}

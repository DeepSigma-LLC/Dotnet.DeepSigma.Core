
namespace DeepSigma.Core.Encode;

/// <summary>
/// Provides methods for encoding and decoding byte arrays to and from strings using various encoding schemes.
/// </summary>
public static class Encoder
{
    /// <summary>
    /// Encodes a byte array into a string using the specified encoding type.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="encodingType"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static string EncodeToString(byte[] data, EncodingType encodingType)
    {
        return encodingType switch
        {
            EncodingType.Hex => Convert.ToHexStringLower(data),
            EncodingType.Base64 => Convert.ToBase64String(data),
            EncodingType.UTF8 => System.Text.Encoding.UTF8.GetString(data),
            EncodingType.UTF32 => System.Text.Encoding.UTF32.GetString(data),
            EncodingType.ASCII => System.Text.Encoding.ASCII.GetString(data),
            EncodingType.Base58 => Base58Encoder.Encode(data),
            EncodingType.Base32 => Base32Encoder.Encode(data),
            _ => throw new ArgumentOutOfRangeException(nameof(encodingType), "Unsupported encoding type"),
        };
    }

    /// <summary>
    /// Decodes a string encoded in the specified encoding type back to a byte array.
    /// </summary>
    /// <param name="encodedData"></param>
    /// <param name="encodingType"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static byte[] DecodeFromString(string encodedData, EncodingType encodingType)
    {
        return encodingType switch
        {
            EncodingType.Hex => Convert.FromHexString(encodedData),
            EncodingType.Base64 => Convert.FromBase64String(encodedData),
            EncodingType.UTF8 => System.Text.Encoding.UTF8.GetBytes(encodedData),
            EncodingType.UTF32 => System.Text.Encoding.UTF32.GetBytes(encodedData),
            EncodingType.ASCII => System.Text.Encoding.ASCII.GetBytes(encodedData),
            EncodingType.Base58 => Base58Encoder.Decode(encodedData),
            EncodingType.Base32 => Base32Encoder.Decode(encodedData),
            _ => throw new ArgumentOutOfRangeException(nameof(encodingType), "Unsupported encoding type"),
        };
    }
}

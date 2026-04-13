using System.Numerics;
using System.Text;

namespace DeepSigma.Core.Encode;

internal static class Base58Encoder
{
    private const string Alphabet = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";

    internal static string Encode(byte[] bytes)
    {
        var intData = bytes.Aggregate<byte, BigInteger>(0, (current, t) => current * 256 + t);
        var result = new StringBuilder();
        while (intData > 0)
        {
            int remainder = (int)(intData % 58);
            intData /= 58;
            result.Insert(0, Alphabet[remainder]);
        }

        // Preserve leading zeros
        foreach (byte b in bytes)
        {
            if (b == 0) result.Insert(0, Alphabet[0]);
            else break;
        }

        return result.ToString();
    }

    internal static byte[] Decode(string input)
    {
        BigInteger intData = 0;
        foreach (char c in input)
        {
            int digit = Alphabet.IndexOf(c);
            if (digit < 0) throw new ArgumentException($"Invalid Base58 character `{c}`", nameof(input));
            intData = intData * 58 + digit;
        }

        // Convert to byte array
        var bytes = intData.ToByteArray(isUnsigned: true, isBigEndian: true);

        // Add leading zeros
        int leadingZeroCount = input.TakeWhile(ch => ch == Alphabet[0]).Count();
        return Enumerable.Repeat((byte)0, leadingZeroCount).Concat(bytes).ToArray();
    }
}

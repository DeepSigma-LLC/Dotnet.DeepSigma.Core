using System;
using System.Collections.Generic;
using System.Text;

namespace DeepSigma.Core.Encode;

internal static class Base32Encoder
{
    private const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";

    internal static string Encode(byte[] data)
    {
        StringBuilder result = new((data.Length + 4) / 5 * 8);
        int buffer = data[0];
        int next = 1;
        int bitsLeft = 8;

        while (bitsLeft > 0 || next < data.Length)
        {
            if (bitsLeft < 5)
            {
                if (next < data.Length)
                {
                    buffer <<= 8;
                    buffer |= data[next++] & 0xFF;
                    bitsLeft += 8;
                }
                else
                {
                    int pad = 5 - bitsLeft;
                    buffer <<= pad;
                    bitsLeft += pad;
                }
            }

            int index = 0x1F & (buffer >> (bitsLeft - 5));
            bitsLeft -= 5;
            result.Append(Alphabet[index]);
        }

        return result.ToString();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    internal static byte[] Decode(string input)
    {
        input = input.TrimEnd('=');
        int byteCount = input.Length * 5 / 8;
        byte[] result = new byte[byteCount];

        int buffer = 0;
        int bitsLeft = 0;
        int count = 0;

        foreach (char c in input.ToUpperInvariant())
        {
            if (c == ' ') continue;
            int val = Alphabet.IndexOf(c);
            if (val < 0) throw new ArgumentException("Invalid Base32 character", nameof(input));

            buffer <<= 5;
            buffer |= val & 0x1F;
            bitsLeft += 5;

            if (bitsLeft >= 8)
            {
                result[count++] = (byte)((buffer >> (bitsLeft - 8)) & 0xFF);
                bitsLeft -= 8;
            }
        }

        return result;
    }
}

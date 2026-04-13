
namespace DeepSigma.Core.Extensions;

/// <summary>
/// Extension methods for byte arrays.
/// </summary>
public static class ByteArrayExtension
{
    /// <summary>
    /// Combines two byte arrays into one.
    /// </summary>
    /// <param name="Data"></param>
    /// <param name="NewData"></param>
    /// <returns></returns>
    public static byte[] Combine(this byte[] Data, byte[] NewData)
    {
        return Combine2ByteArrays(Data, NewData);
    }

    private static byte[] Combine2ByteArrays(byte[] first, byte[] second)
    {
        byte[] ret = new byte[first.Length + second.Length];
        Buffer.BlockCopy(first, 0, ret, 0, first.Length);
        Buffer.BlockCopy(second, 0, ret, first.Length, second.Length);
        return ret;
    }
}

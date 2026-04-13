
namespace DeepSigma.Core.Extensions;

/// <summary>
/// Extension methods for KeyValuePairs.
/// </summary>
public static class KeyValuePairExtension
{
    /// <summary>
    /// Converts a KeyValuePair to a tuple.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="kvp"></param>
    /// <returns></returns>
    public static (TKey Key, TValue Value) ToTuple<TKey, TValue>(this KeyValuePair<TKey, TValue> kvp)
    {
        return (kvp.Key, kvp.Value);
    }
}

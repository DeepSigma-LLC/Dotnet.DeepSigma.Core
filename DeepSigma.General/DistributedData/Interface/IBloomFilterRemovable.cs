
namespace DeepSigma.Core.DistributedData.Interface;

/// <summary>
/// Interface for a Bloom filter that supports item removal.
/// </summary>
public interface IBloomFilterRemovable : IBloomFilter
{
    /// <summary>
    /// Removes an item from the bloom filter.
    /// </summary>
    /// <param name="item"></param>
    void Remove(string item);
}

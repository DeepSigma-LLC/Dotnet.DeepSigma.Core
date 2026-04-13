using DeepSigma.Core.DistributedData.Interface;

namespace DeepSigma.Core.DistributedData;

/// <summary>
/// A scalable counting Bloom filter that grows as more items are added, maintaining a target false-positive rate and allowing item removal.
/// </summary>
public sealed class BloomFilterScalableCounting : BloomFilterScalableAbstract<IBloomFilterRemovable>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BloomFilterScalableCounting"/> class.
    /// </summary>
    /// <param name="initialCapacity"></param>
    /// <param name="targetFalsePositive"></param>
    /// <param name="fillThreshold"></param>
    /// <param name="growthFactor"></param>
    /// <param name="tighteningRatio"></param>
    public BloomFilterScalableCounting(
        int initialCapacity, double targetFalsePositive = 0.01, double fillThreshold = 0.85, double growthFactor = 2.0, double tighteningRatio = 0.5)
        : base(initialCapacity, targetFalsePositive, fillThreshold, growthFactor, tighteningRatio)
    {
        _filters.Add(new BloomFilterCounting(initialCapacity, targetFalsePositive));
    }

    /// <summary>
    /// Add to the newest filter. If it’s getting “full”, spawn a larger, tighter filter.
    /// </summary>
    /// <param name="item"></param>
    public void Add(string item)
    {
        Add(item, CreateNewBloomFilter);
    }

    private BloomFilterCounting CreateNewBloomFilter(int capacity, double falsePositiveProbability)
    {
        return new BloomFilterCounting(capacity, falsePositiveProbability);
    }

    /// <summary>
    /// Best-effort remove: decrement in every layer where the item might be present.
    /// This keeps state consistent even if inserts happened in older layers.
    /// </summary>
    public void Remove(string item)
    {
        if (item is null) throw new ArgumentNullException(nameof(item));
        lock (_lock)
        {
            for (int i = _filters.Count - 1; i >= 0; i--)
            {
                var layer = _filters[i];
                if (layer.MightContain(item))
                {
                    layer.Remove(item); // We do not break here; we want to remove from all layers where it might be present
                }
            }
        }
    }
}
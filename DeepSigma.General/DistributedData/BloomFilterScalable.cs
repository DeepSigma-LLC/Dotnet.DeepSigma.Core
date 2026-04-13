using DeepSigma.Core.DistributedData.Interface;

namespace DeepSigma.Core.DistributedData;

/// <summary>
/// A scalable Bloom filter that grows as needed.
/// </summary>
public sealed class BloomFilterScalable : BloomFilterScalableAbstract<IBloomFilter>
{
    /// <param name="initial_capacity">Expected initial items (e.g., 100_000).</param>
    /// <param name="target_false_positive">Overall target false positive rate for the first filter (e.g., 0.01).</param>
    /// <param name="fill_threshold">When estimated fill exceeds this (0-1), spawn a new filter. 
    /// This is to prevent overfilling any given filter since over filled filters will decrease accuracy. Typical 0.5–0.9.</param>
    /// <param name="growth_factor">How much to scale capacity each time (>= 1.5, often 2.0).</param>
    /// <param name="tightening_ratio">How much to tighten false postive probability each time (0-1). E.g., 0.5 halves false postive probability each level.</param>
    public BloomFilterScalable(
        int initial_capacity, double target_false_positive = 0.01, double fill_threshold = 0.85, double growth_factor = 2.0, double tightening_ratio = 0.5)
        : base(initial_capacity, target_false_positive, fill_threshold, growth_factor, tightening_ratio)
    {
        _filters.Add(new BloomFilter(initial_capacity, target_false_positive));
    }

    /// <summary>
    /// Add to the newest filter. If it’s getting “full”, spawn a larger, tighter filter.
    /// </summary>
    public void Add(string item)
    {
        Add(item, CreateNewBloomFilter);
    }

    private BloomFilter CreateNewBloomFilter(int capacity, double false_positive_probability)
    {
        return new BloomFilter(capacity, false_positive_probability);
    }
}

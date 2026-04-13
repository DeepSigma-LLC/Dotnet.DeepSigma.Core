using DeepSigma.Core.DistributedData.Interface;

namespace DeepSigma.Core.DistributedData;

/// <summary>
/// A scalable Bloom filter that grows as more items are added, maintaining a target false-positive rate.
/// </summary>
public class BloomFilterScalableAbstract<T> where T : IBloomFilter
{
    private protected readonly List<T> _filters = [];

    private protected readonly Lock _lock = new(); // refactored from private readonly object _lock = new();

    /// <summary>
    /// When estimated fill exceeds this (0..1), spawn a new filter. Typical 0.5–0.9.
    /// </summary>
    public double FillThreshold { get; }

    /// <summary>
    /// How much to scale capacity each time (>= 1.5, often 2.0).
    /// </summary>
    public double GrowthFactor { get; }

    /// <summary>
    /// How much to tighten the false positive probability each time (0-1). E.g., 0.5 halves probability each level.
    /// </summary>
    public double TighteningRatio { get; }

    /// <summary>
    /// How many filters are currently in use.
    /// </summary>
    public int FilterCount => _filters.Count;

    /// <param name="initial_capacity">Expected initial items (e.g., 100_000).</param>
    /// <param name="target_false_positive">Overall target false positive rate for the first filter (e.g., 0.01).</param>
    /// <param name="fill_threshold">When estimated fill exceeds this (0-1), spawn a new filter. 
    /// This is to prevent overfilling any given filter since over filled filters will decrease accuracy. Typical 0.5–0.9.</param>
    /// <param name="growth_factor">How much to scale capacity each time (>= 1.5, often 2.0).</param>
    /// <param name="tightening_ratio">How much to tighten false postive probability each time (0-1). E.g., 0.5 halves false postive probability each level.</param>
    public BloomFilterScalableAbstract(
        int initial_capacity,
        double target_false_positive = 0.01,
        double fill_threshold = 0.85,
        double growth_factor = 2.0,
        double tightening_ratio = 0.5)
    {
        if (initial_capacity <= 0) throw new ArgumentOutOfRangeException(nameof(initial_capacity));
        if (target_false_positive <= 0 || target_false_positive >= 1) throw new ArgumentOutOfRangeException(nameof(target_false_positive));
        if (fill_threshold <= 0 || fill_threshold >= 1) throw new ArgumentOutOfRangeException(nameof(fill_threshold));
        if (growth_factor <= 1.0) throw new ArgumentOutOfRangeException(nameof(growth_factor), "Must be > 1.0");
        if (tightening_ratio <= 0 || tightening_ratio >= 1) throw new ArgumentOutOfRangeException(nameof(tightening_ratio), "In (0, 1)");

        FillThreshold = fill_threshold;
        GrowthFactor = growth_factor;
        TighteningRatio = tightening_ratio;
    }


    /// <summary>
    /// Add to the newest filter. If it’s getting “full”, spawn a larger, tighter filter.
    /// </summary>
    public void Add(string item, Func<int, double, T> CreateNewBloomFilter)
    {
        if (item is null) throw new ArgumentNullException(nameof(item));
        lock (_lock)
        {
            var current_filter = _filters[_filters.Count() - 1];
            current_filter.Add(item);

            if (current_filter.EstimatedFill() >= FillThreshold)
            {
                // Compute the next filter’s parameters
                var (number_of_bits, number_of_hashes, inserts, _, _) = current_filter.Parameters();
                // Derive previous logical capacity from parameters: approx k = (m/n)*ln2 => n ~= m*ln2/k
                double ln2 = Math.Log(2.0);
                int approx_capacity = Math.Max(1, (int)Math.Round(number_of_bits * ln2 / number_of_hashes));

                int next_capacity = (int)Math.Ceiling(approx_capacity * GrowthFactor);
                double next_false_positive_probability = Math.Max(1e-12, GetCurrentBaseProbability() * TighteningRatio); // Tighten false positive probability geometrically

                _filters.Add(CreateNewBloomFilter(next_capacity, next_false_positive_probability));
            }
        }
    }

    /// <summary>
    /// Query all filters (newest first). If any says "maybe", return true.
    /// </summary>
    public bool MightContain(string item)
    {
        if (item is null) throw new ArgumentNullException(nameof(item));

        // Read without locking list contents (benign race: at worst we miss a brand-new filter this call)
        for (int i = _filters.Count - 1; i >= 0; i--)
        {
            if (_filters[i].MightContain(item)) return true;
        }
        return false;
    }

    /// <summary>
    /// Inspect current stack (index 0 = oldest).
    /// </summary>
    public IReadOnlyList<(int number_of_bits, int number_of_hashes, long inserts, double estimated_fill, int non_zero_slots)> Describe()
    {
        var list = new List<(int, int, long, double, int)>(_filters.Count);
        foreach (var filter in _filters)
        {
            var (bits, hashes, inserts, _, non_zero_slots) = filter.Parameters();
            list.Add((bits, hashes, inserts, filter.EstimatedFill(), non_zero_slots));
        }
        return list;
    }

    /// <summary>
    /// Estimate the base false positive probability of the newest filter by inverting (number_of_bits, number_of_hashs, inserts) if needed.
    /// </summary>
    /// <returns></returns>
    private protected double GetCurrentBaseProbability()
    {
        var newest_filter = _filters[_filters.Count - 1];
        var (_, number_of_hashes, _, _, _) = newest_filter.Parameters();
        // prob ~= (1 - e^{-hashes* inserts/bits})^hashes at design time with n=capacity.
        // We don’t know the exact original probability; approximate via the number of bits ,number of hashes, capacity relation:
        // At creation: hashes = (bits/inserts)*ln2  => inserts = bits*ln2/hashes,  and prob = (0.5)^(hashes) approximately optimal.
        double prob = Math.Pow(0.5, number_of_hashes); // good approximation at optimal number of hashes
        return Math.Min(0.25, Math.Max(1e-12, prob)); // Clamp to sane range
    }
}
using System.Numerics;

namespace DeepSigma.Core.DistributedData;

/// <summary>
/// Base class for Bloom filters.
/// </summary>
public abstract class BloomFilterAbstract
{
    /// <summary>
    /// How many slots in the filter. Often called 'm' in literature.
    /// </summary>
    private protected readonly int _number_of_slots_in_filter;
    private protected long _number_of_item_inserts = 0;
    private protected long _number_of_item_removes = 0;
    private protected int _non_zero_slot_count;

    /// <summary>
    /// How many bits each item touches. Often called 'k' in literature.
    /// </summary>
    private protected readonly int _number_of_hash_functions;

    /// <summary>
    /// Number of bits in the filter.
    /// </summary>
    public int Capacity { get; }

    /// <summary>
    /// Acceptable false-positive rate (e.g., 0.01).
    /// </summary>
    public double FalsePositiveRate { get; }

    /// <summary>
    /// Initializes object.
    /// </summary>
    /// <param name="capacity"></param>
    /// <param name="false_positive_rate"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public BloomFilterAbstract(int capacity, double false_positive_rate = 0.01)
    {
        if (capacity <= 0) throw new ArgumentOutOfRangeException(nameof(capacity));
        if (false_positive_rate <= 0 || false_positive_rate >= 1) throw new ArgumentOutOfRangeException(nameof(false_positive_rate), "False positive rate must be in (0,1).");

        Capacity = capacity;
        FalsePositiveRate = false_positive_rate;

        // m = ceil(-(n ln p) / (ln 2)^2), k = round((m/n) ln 2)
        var ln2 = Math.Log(2.0);
        _number_of_slots_in_filter = (int)Math.Ceiling(-(capacity * Math.Log(false_positive_rate)) / (ln2 * ln2));
        _number_of_hash_functions = Math.Max(1, (int)Math.Round(_number_of_slots_in_filter / (double)capacity * ln2));
    }

    /// <summary>
    /// Estimated fraction of bits set using the standard Bloom formula:
    /// E[zero] = exp(-k*n/m)  =>  fill = 1 - E[zero]
    /// </summary>
    public double EstimatedFill()
    {
        double n = Math.Max(0, _number_of_item_inserts - _number_of_item_removes);
        double zeros = Math.Exp(-_number_of_hash_functions * n / _number_of_slots_in_filter);
        return 1.0 - zeros;
    }

    /// <summary>
    /// Exact current fill fraction (how many slots are > 0).
    /// </summary>
    public double CurrentFill() => _non_zero_slot_count / (double)_number_of_slots_in_filter;

    /// <summary>
    /// For inspection/testing.
    /// </summary>
    public (int number_of_slots, int number_of_hashes, long number_of_adds, long number_of_removes, int non_zero_slots) Parameters()
       => (_number_of_slots_in_filter, _number_of_hash_functions, _number_of_item_inserts, _number_of_item_removes, _non_zero_slot_count);

    /// <summary>
    /// Add an item, using provided delegates to get and set values at indices.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="item"></param>
    /// <param name="GetSlotValueAtIndex">Gets value by delegating function.</param>
    /// <param name="AddItemAtSlotIndex">Adds value at index by delegate.</param>
    /// <exception cref="ArgumentNullException"></exception>
    private protected void Add<T>(string item, Func<int, T> GetSlotValueAtIndex, Action<int, T> AddItemAtSlotIndex) where T : INumber<T>
    {
        if (item is null) throw new ArgumentNullException(nameof(item));
        (ulong hash1, ulong hash2) = BloomFilterUtilities.HashPair(item);
        for (int i = 0; i < _number_of_hash_functions; i++)
        {
            int index = BloomFilterUtilities.GetComputedIndex(hash1, hash2, i, _number_of_slots_in_filter);
            T prev_value = GetSlotValueAtIndex(index);
            if (prev_value == T.Zero) _non_zero_slot_count++;          // we’re turning a zero into non-zero
            AddItemAtSlotIndex(index, prev_value);
        }
        _number_of_item_inserts++;
    }

    /// <summary>
    /// Query: true => possibly present, false => definitely not present.
    /// </summary>
    private protected bool MightContain(string item, Func<int, bool> IsSlotActivated)
    {
        if (item is null) throw new ArgumentNullException(nameof(item));
        (ulong hash_1, ulong hash_2) = BloomFilterUtilities.HashPair(item);

        for (int i = 0; i < _number_of_hash_functions; i++)
        {
            int index = BloomFilterUtilities.GetComputedIndex(hash_1, hash_2, i, _number_of_slots_in_filter);

            if (IsSlotActivated(index) == false) //If any of the bits at the computed indices is 0, the item is definitely not present.
            {
                return false;
            }
        }
        return true;
    }
}

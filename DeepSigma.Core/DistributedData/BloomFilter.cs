
using System.Collections;
using DeepSigma.Core.DistributedData.Interface;

namespace DeepSigma.Core.DistributedData;

/// <summary>
/// A simple Bloom filter for strings.
/// </summary>
public sealed class BloomFilter : BloomFilterAbstract, IBloomFilter
{
    /// <summary>
    /// The bit array representing the filter activations.
    /// </summary>
    private readonly BitArray _activation_slot_filter;

    /// <summary>
    /// Create a Bloom filter sized for ~capacity items with target false-positive rate p (e.g., 0.01).
    /// </summary>
    public BloomFilter(int capacity, double false_positive_rate = 0.01) : base(capacity, false_positive_rate)
    {
        _activation_slot_filter = new BitArray(_number_of_slots_in_filter);
    }

    /// <summary>
    /// Add a string to the filter.
    /// </summary>
    public void Add(string item)
    {
        Add(item, GetSlotValue, SetSlotValue);
    }

    /// <summary>
    /// Check if the item might be in the filter (may return false positives, but no false negatives). Query: true => possibly present, false => definitely not present.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool MightContain(string item)
    {
        bool result = MightContain(item, IsSlotActivated);
        return result;
    }

    private int GetSlotValue(int index)
    {
        return _activation_slot_filter.Get(index) ? 1 : 0;
    }

    private void SetSlotValue(int index, int previous_value)
    {
        _activation_slot_filter.Set(index, true);
    }

    private bool IsSlotActivated(int index)
    {
        return _activation_slot_filter.Get(index);
    }
}
using DeepSigma.Core.DistributedData.Interface;

namespace DeepSigma.Core.DistributedData;

/// <summary>
/// A counting Bloom filter. Implmentation enables bloom filter with deletions. Alternatively, you can use cuckoo filter.
/// </summary>
public sealed class BloomFilterCounting : BloomFilterAbstract, IBloomFilter, IBloomFilterRemovable
{
    /// <summary>
    /// Rather than turning on bits, we increment bytes to finger print the added items.
    /// </summary>
    private readonly byte[] _activation_slot_filter;   // per-slot counters (0-255)

    /// <summary>
    /// Initializes object.
    /// </summary>
    /// <param name="capacity"></param>
    /// <param name="p"></param>
    public BloomFilterCounting(int capacity, double p = 0.01) : base(capacity, p)
    {
        _activation_slot_filter = new byte[_number_of_slots_in_filter];
        _non_zero_slot_count = 0;
    }

    /// <summary>
    /// Add item to the filter.
    /// </summary>
    /// <param name="item"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public void Add(string item)
    {
        Add(item, GetSlotValue, IncrementSlotValue);
    }

    /// <summary>
    /// Best-effort deletion: decrements counters at the item’s hash function positions.
    /// Safe for false-removes (won’t go below 0 or cause false negatives for other items).
    /// </summary>
    public void Remove(string item)
    {
        if (item is null) throw new ArgumentNullException(nameof(item));

        (ulong hash1, ulong hash2) = BloomFilterUtilities.HashPair(item);
        for (int i = 0; i < _number_of_hash_functions; i++)
        {
            int index = BloomFilterUtilities.GetComputedIndex(hash1, hash2, i, _number_of_slots_in_filter);
            byte previous_value = _activation_slot_filter[index];
            if (previous_value > 0)
            {
                byte next_value = (byte)(previous_value - 1);
                _activation_slot_filter[index] = next_value;
                if (next_value == 0) _non_zero_slot_count--;      // slot just became zero again
            }
        }
        _number_of_item_removes++;
    }

    /// <summary>
    /// Query: true => possibly present, false => definitely not present.
    /// </summary>
    public bool MightContain(string item)
    {
        bool result = MightContain(item, IsSlotActivated);
        return result;
    }

    /// <summary>
    /// Determines if slot is activated from provided index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    private bool IsSlotActivated(int index)
    {
        if (_activation_slot_filter[index] == 0)
        {
            return false;
        }
        return true;
    }


    private byte GetSlotValue(int index)
    {
        return _activation_slot_filter[index];
    }

    private void IncrementSlotValue(int index, byte previous_value)
    {
        _activation_slot_filter[index] = (byte)(previous_value + 1);
    }
}
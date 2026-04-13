namespace DeepSigma.Core.DistributedData.Interface;

/// <summary>
/// Bloom filter interface.
/// </summary>
public interface IBloomFilter
{
    /// <summary>
    /// Adds item to the bloom filter.
    /// </summary>
    /// <param name="item"></param>
    void Add(string item);
    /// <summary>
    /// Determines if a bloom filter contains an element. Returns true if it is probabilistically likely or false only if 
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    bool MightContain(string item);
    /// <summary>
    /// Returns parameters from the bloom filter.
    /// </summary>
    /// <returns></returns>
    (int number_of_slots, int number_of_hashes, long number_of_adds, long number_of_removes, int non_zero_slots) Parameters();
    /// <summary>
    /// Calculates the estimated fill percentage.
    /// </summary>
    /// <returns></returns>
    public double EstimatedFill();
}
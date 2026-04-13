namespace DeepSigma.Core.Utilities;

/// <summary>
/// A helper class for learning and performing common bitwise operations.
/// Each method demonstrates a common use of bit manipulation.
/// </summary>
public static class BitManipulationHelper
{
    /// <summary>
    /// Returns true if the nth bit (0-indexed) is set in the given number.
    /// </summary>
    public static bool IsBitSet(int number, int bitPosition)
    {
        int target_bit_test = 1 << bitPosition; // 0...0001 shifted left by bitPosition. For example, if bitPosition is 3, test will be 0...1000
        return (number & target_bit_test) != 0; // A bitwise AND (&) only keeps bits that are set (1) in both numbers. If the result is not zero, the bit is set.
    }

    /// <summary>
    /// Sets (turns on) the nth bit in the number.
    /// </summary>
    public static int SetBit(int number, int bitPosition)
    {
        int target_bit_flip = 1 << bitPosition; // 0...0001 shifted left by bitPosition. For example, if bitPosition is 3, target_bit_flip will be 0...1000
        return number | target_bit_flip; // A bitwise OR (|) sets bits to 1 if either number has a 1 in that position.
    }

    /// <summary>
    /// Clears (turns off) the nth bit in the number.
    /// </summary>
    public static int ClearBit(int number, int bitPosition)
    {
        int target_bit_flip = 1 << bitPosition; // 0...0001 shifted left by bitPosition. For example, if bitPosition is 3, target_bit_flip will be 0...1000
        return number & ~target_bit_flip; // A bitwise AND (&) with the negated target bit clears that bit position.
        // For example, if target_bit_flip is 0...1000, ~target_bit_flip is 1...0111.
    }

    /// <summary>
    /// Toggles (flips) the nth bit in the number.
    /// </summary>
    public static int ToggleBit(int number, int bitPosition)
    {
        int target_bit_flip = 1 << bitPosition; // 0...0001 shifted left by bitPosition. For example, if bitPosition is 3, target_bit_flip will be 0...1000
        return number ^ target_bit_flip; // A bitwise XOR (^) flips bits where the target bit is 1.
    }

    /// <summary>
    /// Counts how many bits are set to 1 (also known as the Hamming weight).
    /// </summary>
    public static int CountSetBits(int number)
    {
        int count = 0;
        while (number != 0)
        {
            number &= (number - 1); // clears the least significant 1 bit
            count++;
        }
        return count;
    }

    /// <summary>
    /// Checks if the number is a power of two (only one bit set).
    /// </summary>
    public static bool IsPowerOfTwo(int number)
    {
        bool is_positive = number > 0; // Powers of two are positive numbers.
        bool is_power_of_two = (number & (number - 1)) == 0; // A power of two has exactly one bit set, so number & (number - 1) should be 0.
        return is_positive && is_power_of_two;
    }

    /// <summary>
    /// Returns the value of the nth bit (0 or 1).
    /// </summary>
    public static int GetBitValue(int number, int bitPosition)
    {
        int number_with_target_bit_shifted_completely_to_right = number >> bitPosition; // Shift right to bring the target bit to the least significant position.
        return number_with_target_bit_shifted_completely_to_right & 1; // AND with 1 to isolate the least significant bit. Since 1 is 0...0001, this will return either 0 or 1 based on the target bit.
    }

    /// <summary>
    /// Performs a circular left shift (rotate left).
    /// </summary>
    public static int RotateLeft(int number, int bits, int totalBits = 32)
    {
        int number_shifted_left_by_bits = number << bits; // Shift the number left by the specified bits.
        int number_with_overflowed_bits_shifted_to_right_side = number >> (totalBits - bits); // This shifts the bits that overflowed on the left back to the right side.
        return number_shifted_left_by_bits | number_with_overflowed_bits_shifted_to_right_side; // Combine both parts using OR to complete the rotation.
    }

    /// <summary>
    /// Performs a circular right shift (rotate right).
    /// </summary>
    public static int RotateRight(int number, int bits, int totalBits = 32)
    {
        int number_shifted_right_by_bits = number >> bits; // Shift the number right by the specified bits.
        int number_with_overflowed_bits_shifted_to_left_side = number << (totalBits - bits); // This shifts the bits that overflowed on the right back to the left side.
        return number_shifted_right_by_bits | number_with_overflowed_bits_shifted_to_left_side; // Combine both parts using OR to complete the rotation.
    }

    /// <summary>
    /// Displays the binary string representation of a number with leading zeros.
    /// </summary>
    public static string ToBinaryString(int number, int totalBits = 8)
    {
        return Convert.ToString(number, 2).PadLeft(totalBits, '0');
    }
}

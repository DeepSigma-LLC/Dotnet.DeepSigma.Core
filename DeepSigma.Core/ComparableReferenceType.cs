
namespace DeepSigma.Core;

/// <summary>
/// A reference type that implements value-based equality comparison.
/// </summary>
public abstract class ComparableReferenceType
{
    /// <summary>
    /// A unique identifier for this instance.
    /// </summary>
    public required string UniqueId { get; init; }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        if (obj is not ComparableReferenceType other) return false;
        return UniqueId == other.UniqueId;
    }

    /// <summary>
    /// Equality operator overload.
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator ==(ComparableReferenceType? left, ComparableReferenceType? right)
    {
        if (left is null && right is null) return true;
        if (left is null || right is null) return false;
        return left.Equals(right);
    }

    /// <summary>
    /// Inequality operator overload.
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator !=(ComparableReferenceType? left, ComparableReferenceType? right) => !(left == right);

    /// <inheritdoc/>
    public override int GetHashCode() => UniqueId.GetHashCode();
}

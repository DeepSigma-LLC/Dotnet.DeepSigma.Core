using System.Numerics;

namespace DeepSigma.Core;

/// <summary>
/// A generic quantity struct that stores the absolute value of a quantity.
/// </summary>
public readonly struct AbsoluteValue<T>() where T : INumber<T>
{
    /// <inheritdoc cref="AbsoluteValue{T}"/>
    public AbsoluteValue(T value) : this()
    {
        this.Value = value;
    }

    /// <summary>
    /// The value stored as an absolute value.
    /// </summary>
    public T Value
    {
        get => field;
        init => field = T.Abs(value);
    } = T.Zero;

    /// <summary>
    /// Implicit conversion to the underlying type.
    /// </summary>
    /// <param name="absoluteValue"></param>
    public static implicit operator T(AbsoluteValue<T> absoluteValue) => absoluteValue.Value;

    /// <summary>
    /// Implicit conversion from the underlying type.
    /// </summary>
    /// <param name="value"></param>
    public static implicit operator AbsoluteValue<T>(T value) => new(value);    
}

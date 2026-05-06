using System.Reflection;

namespace DeepSigma.Core;

/// <summary>
/// Provides a base class for creating strongly typed enumeration types with named values and associated integer
/// identifiers.
/// </summary>
/// <remarks>This class enables the creation of enumeration types that offer additional functionality and type
/// safety compared to standard enums. Derived types define static instances representing valid values. Instances are
/// compared by their type and value. Use the FromValue and FromName methods to retrieve enumeration instances by their
/// integer value or name, respectively.</remarks>
/// <typeparam name="TEnum">The type of the derived enumeration. Must inherit from StronglyTypedEnumeration<TEnum>.</typeparam>
public abstract class StronglyTypedEnumeration<TEnum> : IEquatable<StronglyTypedEnumeration<TEnum>>
    where TEnum : StronglyTypedEnumeration<TEnum>
{
    private static readonly Dictionary<int, TEnum> Enumerations = CreateEnumerations();

    /// <summary>
    /// Initializes a new instance of the StronglyTypedEnumeration class with the specified integer value and name.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="name"></param>
    protected StronglyTypedEnumeration(int value, string name)
    {
        Value = value;
        Name = name;
    }

    /// <summary>
    /// Gets the integer value associated with this instance.
    /// </summary>
    public int Value { get; protected init; }

    /// <summary>
    /// Gets the name associated with the current instance.
    /// </summary>
    public string Name { get; protected init; } = string.Empty;

    /// <summary>
    /// Retrieves the enumeration instance of type TEnum that corresponds to the specified integer value.
    /// </summary>
    /// <remarks>Use this method to convert an integer value to its corresponding enumeration instance when
    /// working with custom enumeration patterns. If the value does not correspond to any defined enumeration, the
    /// method returns null.</remarks>
    /// <param name="value">The integer value to locate in the enumeration mapping.</param>
    /// <returns>An instance of TEnum that matches the specified value, or null if no matching enumeration is found.</returns>
    public static TEnum? FromValue(int value)
    {
        return Enumerations.TryGetValue(value, out TEnum? enumeration)
            ? enumeration
            : default;
    }

    /// <summary>
    /// Retrieves an enumeration value of type TEnum that matches the specified name, using a case-insensitive
    /// comparison.
    /// </summary>
    /// <param name="name">The name of the enumeration value to locate. The comparison is case-insensitive.</param>
    /// <returns>The enumeration value of type TEnum that matches the specified name, or null if no match is found.</returns>
    public static TEnum? FromName(string name)
    {
        return Enumerations.Values
            .SingleOrDefault(e => e.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Determines whether the current instance is equal to another strongly typed enumeration of the same type and
    /// value.
    /// </summary>
    /// <param name="other">The strongly typed enumeration to compare with the current instance. Can be null.</param>
    /// <returns>true if the specified enumeration is not null and has the same type and value as the current instance;
    /// otherwise, false.</returns>
    public bool Equals(StronglyTypedEnumeration<TEnum>? other)
    {
        if (other is null) return false;

        return GetType() == other.GetType() && Value == other.Value;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return obj is StronglyTypedEnumeration<TEnum> other && Equals(other);
    }

    /// <inheritdoc/>
    public override int GetHashCode() => Value.GetHashCode();

    /// <summary>
    /// Uses reflection to find all public static fields of the enumeration type and creates a dictionary mapping their integer values to the corresponding enumeration instances.
    /// </summary>
    /// <returns></returns>
    private static Dictionary<int, TEnum> CreateEnumerations()
    {
        var enumerationType = typeof(TEnum);

        var fieldsForType = enumerationType.GetFields(
            BindingFlags.Public |
            BindingFlags.Static |
            BindingFlags.FlattenHierarchy)
            .Where(fieldInfo => enumerationType.IsAssignableFrom(fieldInfo.FieldType))
            .Select(fieldInfo => (TEnum)fieldInfo.GetValue(default)!);

        return fieldsForType.ToDictionary(e => e.Value);
    }
}


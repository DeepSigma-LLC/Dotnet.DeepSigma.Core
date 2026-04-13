using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace DeepSigma.Core.Extensions;

/// <summary>
/// Extension methods for enums to retrieve their description strings.
/// </summary>
public static class EnumExtension
{
    public enum Status
    {
        /// <summary>
        /// The operation is currently running or in progress.
        /// </summary>
        [Description("In Progress")]
        InProgress,

        /// <summary>
        /// The operation has finished successfully.
        /// </summary>
        [Display(Name = "Completed Successfully")]
        Completed,

        /// <summary>
        /// The operation encountered an error and did not finish successfully.
        /// </summary>
        Failed
    }


    /// <summary>
    /// Checks if the enum value is defined in the enum type.
    /// </summary>
    /// <remarks>
    /// <code>
    /// // Example usage:
    /// enum Color { Red, Green, Blue }
    /// Color myColor = Color.Red;
    /// bool isDefined = myColor.IsDefined(); // returns true
    /// 
    /// Color invalidColor = (Color)100;
    /// bool isDefinedInvalid = invalidColor.IsDefined(); // returns false
    /// </code>
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool IsDefined<T>(this T value) where T : struct, Enum => Enum.IsDefined(typeof(T), value);


    /// <summary>
    /// Converts an enum value to its description string, using the DescriptionAttribute or DisplayAttribute if available.
    /// </summary>
    /// <remarks>
    /// <code>
    /// // Example usage:
    /// enum Status
    /// {
    ///     [Description("In Progress")]
    ///     InProgress,
    ///     [Display(Name = "Completed Successfully")]
    ///     Completed,
    ///     Failed
    /// }
    /// 
    /// Status currentStatus = Status.InProgress;
    /// string description = currentStatus.ToDescriptionString(); // returns "In Progress"
    /// </code>
    /// </remarks>
    /// <param name="value"></param>
    /// <param name="fallbackToName"></param>
    /// <returns></returns>
    public static string ToDescriptionString<T>(this T value, bool fallbackToName = true) where T : struct, Enum
    {
        var type = value.GetType();
        var name = Enum.GetName(type, value);

        // For [Flags] combinations, Enum.GetName can be null
        if (name is null) return fallbackToName ? value.ToString() : string.Empty;

        var field = type.GetField(name);
        if (field is null) return fallbackToName ? name : string.Empty;

        return field.GetCustomAttribute<DescriptionAttribute>()?.Description
            ?? field.GetCustomAttribute<DisplayAttribute>()?.Name
            ?? (fallbackToName ? name : string.Empty);
    }

    /// <summary>
    /// Checks if the enum value matches any of the provided options.
    /// </summary>
    /// <remarks>
    /// <code>
    /// //Example usage:
    /// enum Color { Red, Green, Blue, Yellow }
    /// Color myColor = Color.Green;
    /// bool isPrimary = myColor.IsAnyOf(Color.Red, Color.Green, Color.Blue); // returns true
    /// </code>
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static bool IsAnyOf<T>(this T value, params T[] options) where T : struct, Enum => options.Contains(value);

    /// <summary>
    /// Converts an enum value to its display string using the DisplayAttribute if available.
    /// </summary>
    /// <remarks>
    /// <code>
    /// //Example usage:
    /// enum Status
    /// {
    ///     [Display(Name = "In Progress")]
    ///     InProgress,
    ///     [Display(Name = "Completed Successfully")]
    ///     Completed,
    ///     Failed
    /// }
    /// Status currentStatus = Status.InProgress;
    /// string displayString = currentStatus.ToDisplayString(); // returns "In Progress"
    /// </code>
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string ToDisplayString<T>(this T value) where T : struct, Enum
    {
        var field = typeof(T).GetField(value.ToString());
        var attr = field?.GetCustomAttribute<DisplayAttribute>();
        return attr?.Name ?? value.ToString();
    }

    /// <summary>
    /// Converts an enum value to its integer representation.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    public static int ToInt<T>(this T value) where T : struct, Enum => Convert.ToInt32(value);

    /// <summary>
    /// Converts an enum value to its long representation.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    public static long ToLong<T>(this T value) where T : struct, Enum => Convert.ToInt64(value);

    /// <summary>
    /// Converts an integer to its corresponding enum value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    public static T ToEnum<T>(this int value) where T : struct, Enum => (T)Enum.ToObject(typeof(T), value);

    /// <summary>
    /// Converts an integer to its corresponding enum value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    public static T ToEnum<T>(this long value) where T : struct, Enum => (T)Enum.ToObject(typeof(T), value);


    /// <summary>
    /// Checks if the enum value has the specified flag set (for [Flags] enums).
    /// This is an allocation-free and fast alternative to Enum.HasFlag. 
    /// Note: This method works for enums decorated with the [Flags] attribute.
    /// Flags are enum values that can be multiple values combined using bitwise OR operations.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <param name="flag"></param>
    /// <returns></returns>
    public static bool HasFlagFast<T>(this T value, T flag) where T : struct, Enum
    {
        var val = Convert.ToUInt64(value);
        var flg = Convert.ToUInt64(flag);
        return (val & flg) == flg;
    }

    /// <summary>
    /// Adds the specified flag to the enum value (for [Flags] enums).
    /// Note: This method works for enums decorated with the [Flags] attribute.
    /// Flags are enum values that can be multiple values combined using bitwise OR operations.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <param name="flag"></param>
    /// <returns></returns>
    public static T AddFlag<T>(this T value, T flag) where T : struct, Enum
    {
        var val = Convert.ToUInt64(value);
        var flg = Convert.ToUInt64(flag);
        return (T)Enum.ToObject(typeof(T), val | flg);
    }

    /// <summary>
    /// Removes the specified flag from the enum value (for [Flags] enums).
    /// Note: This method works for enums decorated with the [Flags] attribute.
    /// Flags are enum values that can be multiple values combined using bitwise OR operations.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <param name="flag"></param>
    /// <returns></returns>
    public static T RemoveFlag<T>(this T value, T flag) where T : struct, Enum
    {
        var val = Convert.ToUInt64(value);
        var flg = Convert.ToUInt64(flag);
        return (T)Enum.ToObject(typeof(T), val & ~flg);
    }

    /// <summary>
    /// Toggles the specified flag on the enum value (for [Flags] enums).
    /// Toggle means if the flag is set, it will be removed; if it is not set, it will be added.
    /// Note: This method works for enums decorated with the [Flags] attribute.
    /// Flags are enum values that can be multiple values combined using bitwise OR operations.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <param name="flag"></param>
    /// <returns></returns>
    public static T ToggleFlag<T>(this T value, T flag) where T : struct, Enum
    {
        var val = Convert.ToUInt64(value);
        var flg = Convert.ToUInt64(flag);
        return (T)Enum.ToObject(typeof(T), val ^ flg);
    }

    /// <summary>
    /// Checks if the enum value has any of the specified flags set (for [Flags] enums).
    /// Note: This method works for enums decorated with the [Flags] attribute.
    /// Flags are enum values that can be multiple values combined using bitwise OR operations.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <param name="flags"></param>
    /// <returns></returns>
    public static bool HasAnyFlag<T>(this T value, params T[] flags) where T : struct, Enum
    {
        var val = Convert.ToUInt64(value);
        foreach (var flag in flags)
            if ((val & Convert.ToUInt64(flag)) != 0)
                return true;
        return false;
    }

    extension<T>(T) where T : struct, Enum
    {
        /// <summary>
        /// Gets all enum values of the specified enum type.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<T> GetAllValues() => Enum.GetValues(typeof(T)).Cast<T>();

        /// <summary>
        /// Gets all enum names of the specified enum type.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> GetNames() => Enum.GetNames(typeof(T));

        /// <summary>
        /// Gets a dictionary mapping enum values to their string representations.
        /// Helpful for populating dropdowns or selection lists.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Dictionary<T, string> ToDictionary() =>
        Enum.GetValues(typeof(T))
            .Cast<T>()
            .ToDictionary(v => v, v => v.ToString());
    }
}


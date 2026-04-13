using System.ComponentModel;

namespace DeepSigma.Core.DateTimeUnification;

/// <summary>
/// Type converter for CustomDateTypeConverter.
/// </summary>
/// <typeparam name="T"></typeparam>
public class CustomDateTypeConverter<T> : TypeConverter where T : struct, IDateTime<T>
{
    /// <summary>
    /// Determines whether this converter can convert an object of the given type to the type of this converter.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="sourceType"></param>
    /// <returns></returns>
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        => sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);

    /// <summary>
    /// Converts the given object to the type of this converter.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="culture"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public override object? ConvertFrom(ITypeDescriptorContext? context, System.Globalization.CultureInfo? culture, object value)
    {
        if (value is string s) return T.Parse(s);
        return base.ConvertFrom(context, culture, value);
    }
}


namespace DeepSigma.Core;

/// <summary>
/// ColorRGB is a structure that represents a color in RGB format.
/// </summary>
public readonly struct ColorRGB(byte red, byte green, byte blue)
{
    /// <summary>
    /// The red component of the color, ranging from 0 to 255.
    /// </summary>
    public byte Red { get; }  = red;

    /// <summary>
    /// The green component of the color, ranging from 0 to 255.
    /// </summary>
    public byte Green { get; } = green;

    /// <summary>
    /// The blue component of the color, ranging from 0 to 255.
    /// </summary>
    public byte Blue { get; } = blue;

    /// <summary>
    /// Returns a string representation of the ColorRGB instance.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return $"ColorRGB(R: {Red}, G: {Green}, B: {Blue})";
    }

    /// <summary>
    /// Creates a ColorRGB instance from a byte array.
    /// </summary>
    /// <param name="values"></param>
    /// <returns></returns>
    public static ColorRGB FromByteArray((byte red, byte green, byte blue) values)
    {
        return new ColorRGB(values.red, values.green, values.blue);
    }

    /// <summary>
    /// Interpolates between two ColorRGB instances based on a given fraction.
    /// </summary>
    /// <param name="color1">
    /// The starting color (fraction = 0).
    /// </param>
    /// <param name="color2">
    /// The ending color (fraction = 1).
    /// </param>
    /// <param name="fraction">
    /// A value between 0 and 1 where 0 corresponds to color1 and 1 corresponds to color2.
    /// </param>
    /// <returns></returns>
    public static ColorRGB Interpolate(ColorRGB color1, ColorRGB color2, float fraction)
    {
        byte red = (byte)(color1.Red + (color2.Red - color1.Red) * fraction);
        byte green = (byte)(color1.Green + (color2.Green - color1.Green) * fraction);
        byte blue = (byte)(color1.Blue + (color2.Blue - color1.Blue) * fraction);
        return new ColorRGB(red, green, blue);
    }

    /// <summary>
    /// Calculates the Euclidean distance between two ColorRGB instances.
    /// </summary>
    /// <param name="color1">
    /// The first color.
    /// </param>
    /// <param name="color2">
    /// The second color.
    /// </param>
    /// <returns></returns>
    public static double Difference(ColorRGB color1, ColorRGB color2)
    {
        int redDiff = color1.Red - color2.Red;
        int greenDiff = color1.Green - color2.Green;
        int blueDiff = color1.Blue - color2.Blue;
        return Math.Sqrt(redDiff * redDiff + greenDiff * greenDiff + blueDiff * blueDiff);
    }
}

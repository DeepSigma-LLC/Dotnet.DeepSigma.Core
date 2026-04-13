
namespace DeepSigma.Core.Utilities;


/// <summary>
/// Utility class for image-related operations.
/// </summary>
public static class ImageUtilities
{
    /// <summary>
    /// Converts an image file to a Base64 string representation.
    /// </summary>
    /// <param name="FullFilePath"></param>
    /// <returns></returns>
    public static string ConvertImageFileToBase64String(string FullFilePath)
    {
        byte[] imageArray = File.ReadAllBytes(FullFilePath);
        string base64ImageRepresentation = Convert.ToBase64String(imageArray);
        return base64ImageRepresentation;
    }

    /// <summary>
    /// Converts an image file to a byte array.
    /// </summary>
    /// <param name="FullFilePath"></param>
    /// <returns></returns>
    public static byte[] ConvertImageFileToBase64ByteArray(string FullFilePath)
    {
        byte[] imageArray = File.ReadAllBytes(FullFilePath);
        return imageArray;
    }
}
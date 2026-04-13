
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace DeepSigma.Core.Extensions;

/// <summary>
/// Extension methods for string operations.
/// </summary>
public static class StringExtension
{
    /// <summary>
    /// Checks if the string is null or empty.
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static bool IsNullOrEmpty(this string? s) => string.IsNullOrEmpty(s);

    /// <summary>
    /// Checks if the string is null, empty, or consists only of white-space characters.
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static bool IsNullOrWhiteSpace(this string? s) => string.IsNullOrWhiteSpace(s);

    /// <summary>
    /// Returns the original string if it is not null or empty; otherwise, returns the specified default value.
    /// </summary>
    /// <param name="s"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static string DefaultIfEmpty(this string? s, string defaultValue) => string.IsNullOrEmpty(s) ? defaultValue : s!;

    /// <summary>
    /// Removes all whitespace characters from the string.
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string RemoveWhitespace(this string s) => new(s.Where(c => !char.IsWhiteSpace(c)).ToArray());

    /// <summary>
    /// Converts the string to title case.
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string ToTitleCase(this string s)
    {
        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(s.ToLower());
    }

    /// <summary>
    /// Normalizes spaces in the string by replacing multiple consecutive whitespace characters with a single space and trimming leading/trailing spaces.
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string NormalizeSpaces(this string s) => Regex.Replace(s, @"\s+", " ").Trim();


    /// <summary>
    /// Capitalizes the first letter of the string.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string CapitalizeFirstLetter(this string value)
    {
        if (value.IsNullOrEmpty()) return value;
        return char.ToUpper(value[0]) + value.Substring(1);
    }

    /// <summary>
    /// Converts a string to snake_case.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string ToSnakeCase(this string value)
    {
        if (value.IsNullOrEmpty()) return value;

        StringBuilder stringBuilder = new();
        for (int i = 0; i < value.Length; i++)
        {
            char c = value[i];
            if (char.IsUpper(c))
            {
                if (i > 0)
                {
                    stringBuilder.Append('_');
                }
                stringBuilder.Append(char.ToLower(c));
            }
            else
            {
                stringBuilder.Append(c);
            }
        }
        return stringBuilder.ToString();
    }

    /// <summary>
    /// Converts a string to camelCase.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string ToCamelCase(this string value)
    {
        if (value.IsNullOrEmpty()) return value;
        string[] words = value.Split(['_', ' '], StringSplitOptions.RemoveEmptyEntries);
        for (int i = 1; i < words.Length; i++)
        {
            words[i] = char.ToUpper(words[i][0]) + words[i].Substring(1).ToLower();
        }
        return string.Join(string.Empty, words);
    }

    /// <summary>
    /// Gets the substring between two specified strings.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    public static string Between(this string value, string start, string end)
    {
        int startIdx = value.IndexOf(start);
        if (startIdx == -1) return string.Empty;

        startIdx += start.Length;
        int endIdx = value.IndexOf(end, startIdx);
        if (endIdx == -1) return string.Empty;

        return value[startIdx..endIdx];
    }

    /// <summary>
    /// Gets the leftmost characters of a string up to the specified length.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    public static string Left(this string value, int length) => value.Length <= length ? value: value[..length];

    /// <summary>
    /// Gets the rightmost characters of a string up to the specified length.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    public static string Right(this string value, int length) => value.Length <= length ? value : value[^length..];

    /// <summary>
    /// Gets the substring after the specified delimiter.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="delimiter"></param>
    /// <returns></returns>
    public static string After(this string value,string delimiter)
    {
        int idx = value.IndexOf(delimiter);
        if (idx == -1) return string.Empty;

        idx += delimiter.Length;
        return value[idx..];
    }

    /// <summary>
    /// Gets the substring before the specified delimiter.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="delimiter"></param>
    /// <returns></returns>
    public static string Before(this string value, string delimiter)
    {
        int idx = value.IndexOf(delimiter);
        if (idx == -1) return string.Empty;
        return value[..idx];
    }

    /// <summary>
    /// Reverses the characters in a string.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string Reverse(this string value)
    {
        if (value.IsNullOrEmpty()) return value;
        char[] charArray = value.ToCharArray();

        Array.Reverse(charArray);
        return new string(charArray);
    }

    /// <summary>
    /// Splits a string into an array of lines based on newline characters.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string[] ToLines(this string value)
    {
        return value.Split(Environment.NewLine);
    }

    /// <summary>
    /// Joins an enumerable of strings into a single string with newline characters.
    /// </summary>
    /// <param name="lines"></param>
    /// <returns></returns>
    public static string FromLines(this IEnumerable<string> lines)
    {
        return string.Join(Environment.NewLine, lines);
    }

    /// <summary>
    /// Converts a string to a DateTime object.
    /// </summary>
    /// <param name="dateTimeString"></param>
    /// <returns></returns>
    public static DateTime? ToDateTime(this string dateTimeString)
    {
        bool success = DateTime.TryParse(dateTimeString, out DateTime result);
        if (success) return result;
        return null;
    }

    /// <summary>
    /// Converts a string to an enum of type T. Returns defaultValue if conversion fails.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    public static T? ToEnum<T>(this string? value) where T : struct, Enum
    {
        if (value.IsNullOrWhiteSpace()) return null;

        return Enum.TryParse<T>(value, true, out var result) ? result : null;
    }

    /// <summary>
    /// Determines if a string is a valid email address.
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    public static bool IsValidEmail(this string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Determines if a string is a valid URL.
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public static bool IsValidURL(this string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out Uri? uriResult)
               && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }

    /// <summary>
    /// Determines if a string is a valid GUID.
    /// </summary>
    /// <param name="guidString"></param>
    /// <returns></returns>
    public static bool IsValidGUID(this string guidString)
    {
        return Guid.TryParse(guidString, out _);
    }

    /// <summary>
    /// Determines if a string is a valid JSON format.
    /// </summary>
    /// <param name="jsonString"></param>
    /// <returns></returns>
    public static bool IsValidJson(this string jsonString)
    {
        jsonString = jsonString.Trim();
        if ((jsonString.StartsWith("{") && jsonString.EndsWith("}")) || //For object
            (jsonString.StartsWith("[") && jsonString.EndsWith("]")))   //For array
        {
            try
            {
                var obj = System.Text.Json.JsonDocument.Parse(jsonString);
                return true;
            }
            catch
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// String replacement method to replace "1 = 1" in a sql query string with a new value.
    /// </summary>
    /// <param name="SQLString"></param>
    /// <param name="ReplacementString"></param>
    /// <returns></returns>
    public static string ReplaceSQL(this string SQLString, string ReplacementString)
    {
        return SQLString.Replace("1 = 1", ReplacementString);
    }

    /// <summary>
    /// Determines if string is a numeric value.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool IsNumeric(this string value)
    {
        return decimal.TryParse(value, out _);
    }

    /// <summary>
    /// Truncates a string to a specific max length
    /// </summary>
    /// <param name="value"></param>
    /// <param name="maxLength"></param>
    /// <param name="truncationSuffix"></param>
    /// <returns></returns>
    public static string Truncate(this string value, int maxLength, string truncationSuffix = "…")
    {
        if (value.IsNullOrEmpty()) return value;

        if (value.Length <= maxLength)
        {
            maxLength = value.Length;
        }

        return value.Substring(0, maxLength) + truncationSuffix;
    }

    /// <summary>
    /// Gets file extention from end of string if string is a file name.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string? GetFileExtention(this string value)
    {
        string[] strings = value.Split('.');

        if (strings.Length <= 1) return null;
        return strings[^1];
    }

    /// <summary>
    /// Multiplies a string by defined number. For example, "A".Multiply(2) = "AA".
    /// </summary>
    /// <param name="value"></param>
    /// <param name="MultiplicationCount"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static string Multiply(this string value, int MultiplicationCount)
    {
        if (MultiplicationCount < 0) throw new ArgumentException("You may only multiply a string by a positive integer");

        return String.Concat(Enumerable.Repeat(value, MultiplicationCount));
    }

    /// <summary>
    /// Parses string as given data type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="input"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="FormatException"></exception>
    /// <exception cref="OverflowException"></exception>
    public static T ParseAs<T>(this string input) where T : IParsable<T> => T.Parse(input, null);


    /// <summary>
    /// Tries to parse string as given data type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="input"> Input string to parse.</param>
    /// <param name="result"> Result of parsing if successful; otherwise, default value of T.</param>
    /// <returns>Returns a tuple indicating success and any exception encountered.</returns>
    public static (bool Success, Exception? Error) TryParseAs<T>(this string input, out T? result) 
        where T : IParsable<T>
    {
        try
        {
            result = input.ParseAs<T>();
            return (true, null);
        }
        catch(Exception ex)
        {
            result = default;
            return (false, ex);
        }
    }
}
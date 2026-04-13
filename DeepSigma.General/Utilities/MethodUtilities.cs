using System.Reflection;

namespace DeepSigma.Core.Utilities;

/// <summary>
/// Utility class for method-related operations.
/// </summary>
public static class MethodUtilities
{
    /// <summary>
    /// Gets the parameter name from a method and selected parameter index.
    /// </summary>
    /// <param name="method"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public static string? GetParameterName(MethodInfo method, int index)
    {
        string? retrieved_value = null;
        if (method != null && method.GetParameters().Length > index)
        {
            retrieved_value = method.GetParameters()[index].Name;
        }
        return retrieved_value;
    }

    /// <summary>
    /// Gets parameter type from method and selected parameter index.
    /// </summary>
    /// <param name="method"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public static Type? GetParameterType(MethodInfo method, int index)
    {
        Type? parameterType = null;
        if (method != null && method.GetParameters().Length > index)
        {
            parameterType = method.GetParameters()[index].ParameterType;
        }
        return parameterType;
    }
}

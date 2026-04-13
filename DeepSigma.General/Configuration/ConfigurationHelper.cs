using Microsoft.Extensions.Configuration;

namespace DeepSigma.Core.Configuration;

/// <summary>
/// Helper class for loading configuration sections into strongly typed objects. Dependency injection method should be preferred but this is useful for some scenarios.
/// </summary>
public static class ConfigurationHelper
{
    /// <summary>
    /// Loads a configuration section into a strongly typed object.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="configuration"></param>
    /// <param name="section_names"></param>
    /// <returns></returns>
    public static T? LoadSection<T>(this IConfiguration configuration, string[] section_names) where T : new()
    {
        IConfigurationSection section = configuration.GetSection(section_names[0]);
        for (int i = 1; i < section_names.Length; i++)
        {
            section = section.GetSection(section_names[i]);
            if (!section.Exists())
            {
                break;
            }
        }
        T? settings = section.Get<T>();
        return settings;
    }
}
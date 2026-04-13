using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace DeepSigma.Core.Configuration;

/// <summary>
/// A generic class for loading plugins from DLL files in a specified directory.
/// </summary>
public static class PluginDLLLoader
{
    /// <summary>
    /// Loads and returns all plugins of type T from the specified directory.
    /// </summary>
    /// <param name="directory"></param>
    /// <returns></returns>
    public static IEnumerable<T> LoadPluginsAllAvailable<T>(string directory) where T : class
    {
        ServiceProvider serviceProvider = GetServiceProviderForPlugInLoader(directory);

        IEnumerable<T> plugins = serviceProvider.GetServices<T>();
        return plugins;
    }

    /// <summary>
    /// Loads a single plugin of type T from the specified directory.
    /// </summary>
    /// <param name="directory"></param>
    /// <param name="is_required_serivce"></param>
    /// <returns></returns>
    public static T? LoadPluginSingle<T>(string directory, bool is_required_serivce = false) where T : class
    {
        ServiceProvider serviceProvider = GetServiceProviderForPlugInLoader(directory);

        T? plugin;
        if (is_required_serivce)
        {
            plugin = serviceProvider.GetRequiredService<T>();
            return plugin;
        }

        plugin = serviceProvider.GetService<T>();
        return plugin;
    }

    /// <summary>
    /// Creates a ServiceProvider for loading plugins from the specified directory.
    /// </summary>
    /// <param name="directory"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    private static ServiceProvider GetServiceProviderForPlugInLoader(string directory)
    {
        if (string.IsNullOrEmpty(directory))
        {
            throw new ArgumentException("Directory cannot be null or empty.", nameof(directory));
        }

        Assembly[] assemblies = Directory.GetFiles(directory, "*.dll")
          .Select(Assembly.LoadFrom)
          .ToArray();

        ServiceCollection services = new();
        services.Scan(x => x
            .FromAssemblies(assemblies)
            .AddClasses()
            .AsSelfWithInterfaces()
            .WithSingletonLifetime());

        ServiceProvider serviceProvider = services.BuildServiceProvider();
        return serviceProvider;
    }
}

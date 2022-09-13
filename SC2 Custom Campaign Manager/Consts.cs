using System.Reflection;

namespace SC2_Custom_Campaign_Manager;

/// <summary>
/// Program-wide constants
/// </summary>
public static class Consts
{
    /// <summary>
    /// Name of the application
    /// </summary>
    public const string AppName = "SC2 Custom Campaign Manager: Maui Edition";
    
    /// <summary>
    /// Version of the appication
    /// </summary>
    public static string Version => Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "0.1";
}
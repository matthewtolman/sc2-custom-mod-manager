using System.Reflection;

namespace SC2_Custom_Campaign_Manager;

public static class Consts
{
    public static string AppName { get; } = "SC2 Custom Campaign Manager: Maui Edition";
    public static string Version { get; } = Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "0.1";
}
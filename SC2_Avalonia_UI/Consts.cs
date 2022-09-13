using System.Reflection;

namespace SC2_Avalonia_UI;

public static class Consts
{
    public static string AppName { get; } = "SC2 Custom Campaign Manager: Avalon Edition";
    public static string Version { get; } = Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "0.1";
}
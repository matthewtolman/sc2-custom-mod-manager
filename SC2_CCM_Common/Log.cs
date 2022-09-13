using Serilog;
using Serilog.Core;

namespace SC2_CCM_Common;

/// <summary>
/// Logging Class
/// </summary>
public static class Log
{
    /// <summary>
    /// Logger used by SC2_CCM_Common
    /// </summary>
    public static Logger Logger { get; set; } = new LoggerConfiguration()
        .WriteTo.Console()
        .CreateLogger();
}
using Serilog;
using Serilog.Core;

namespace SC2_CCM_Common;

public static class Log
{
    public static Logger Logger { get; set; } = new LoggerConfiguration()
        .WriteTo.Console()
        .CreateLogger();
}
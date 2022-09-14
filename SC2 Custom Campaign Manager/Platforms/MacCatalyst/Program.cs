using ObjCRuntime;
using Serilog;
using Serilog.Events;
using UIKit;
using Log = SC2_CCM_Common.Log;

namespace SC2_Custom_Campaign_Manager;

/// <summary>
/// Main program for Mac
/// </summary>
public class Program
{
    // This is the main entry point of the application.
    static void Main(string[] args)
    {
        // Select our log file
        var logFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            Path.Combine("SC2CCM", "SC2CCM.log"));

        // Rotate away the old log file (keep a history of 1)
        try
        {
            if (File.Exists(logFile))
            {
                var oldLogFile = $"{logFile}.old";
                if (File.Exists(oldLogFile))
                {
                    File.Delete(oldLogFile);
                }
                File.Move(logFile, oldLogFile);
            }
        }
        catch(Exception e)
        {
            Console.WriteLine("Failed to rotate logs! " + e.Message);
        }

        // Initialize our logger
        Log.Logger = new LoggerConfiguration()
            .WriteTo.File(
                logFile,
                fileSizeLimitBytes: 2 * 1024 * 1024, // 2MBÏ
                restrictedToMinimumLevel: LogEventLevel.Debug
            )
            .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Verbose)
            .CreateLogger();
        Console.WriteLine($"Log file {logFile}");

        try
        {
            // if you want to use a different Application Delegate class from "AppDelegate"
            // you can specify it here.
            UIApplication.Main(args, null, typeof(AppDelegate));
        }
        catch (Exception ex)
        {
            // Make sure we log program crashes
            Log.Logger.Fatal(ex, "Program Crash Detected!");
        }
    }
}
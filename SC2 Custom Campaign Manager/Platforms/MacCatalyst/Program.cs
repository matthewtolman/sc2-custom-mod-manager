using ObjCRuntime;
using Serilog;
using Serilog.Events;
using UIKit;
using Log = SC2_CCM_Common.Log;

namespace SC2_Custom_Campaign_Manager;

public class Program
{
    // This is the main entry point of the application.
    static void Main(string[] args)
    {
        var logFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            Path.Combine("SC2CCM", "SC2CCM.log"));

        try
        {
            if (File.Exists(logFile))
            {
                var oldLogFile = logFile + ".old";
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

        Log.Logger = new LoggerConfiguration()
            .WriteTo.File(
                logFile,
                fileSizeLimitBytes: 2 * 1024 * 1024, // 2MB
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
            Log.Logger.Fatal(ex, "Program Crash Detected!");
        }
    }
}
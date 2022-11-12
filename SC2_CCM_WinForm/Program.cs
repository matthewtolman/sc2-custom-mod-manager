using Serilog.Events;
using Serilog;
using Log = SC2_CCM_Common.Log;

namespace SC2_CCM_WinForm
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
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
            catch (Exception e)
            {
                Console.WriteLine("Failed to rotate logs! " + e.Message);
            }

            // Initialize our logger
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
                // To customize application configuration such as set high DPI settings or default font,
                // see https://aka.ms/applicationconfiguration.
                ApplicationConfiguration.Initialize();
                Application.Run(new MainPage());
            }
            catch (Exception ex)
            {
                // Make sure we log program crashes
                Log.Logger.Fatal(ex, "Program Crash Detected!");
            }
        }
    }
}
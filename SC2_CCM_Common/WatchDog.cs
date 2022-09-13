namespace SC2_CCM_Common;

/// <summary>
/// Watch Dog checks for unresponsiveness in the app and will log messages if that happens
/// Goal is to not block main thread (and make it unresponsive)
/// This helps detect that
/// 
/// It also detects if the app freezes during startup
/// </summary>
public class WatchDog : IDisposable
{
    /// <summary>
    /// Default number of milliseconds for a Watch Dog thread to check
    /// </summary>
    private const long DefaultWatchDogThreadMs = 150;

    /// <summary>
    /// Thread that the watch dog runs on
    /// </summary>
    private readonly Thread _watchDogThread;
    
    /// <summary>
    /// Current ping count to detect how many keep-alive pings were sent
    /// </summary>
    private int _pingCount;
    
    /// <summary>
    /// Indicator of whether the system is healthy
    /// </summary>
    private long _healthy; // Using an long for full interlocked support
    
    /// <summary>
    /// Indicator of whether the watch dog/periodic check threads should keep running
    /// If set to a value other than 1, the threads will stop when they next read it
    /// </summary>
    private long _continueChecking = 1; // Using an long for full interlocked support
    
    /// <summary>
    /// List of periodic checks that are managed by this WatchDog
    /// Periodic checks allow automating the running of a pulse
    /// Useful for when there is an event-based system (such as a UI)
    /// </summary>
    private readonly List<Thread> _periodicChecks = new();

    /// <summary>
    /// Creates a new Watchdog
    /// </summary>
    /// <param name="watchInterval">Interval for checking. Defaults to 500ms</param>
    /// <param name="numRetriesBeforeLog">Number of allowed failures before a message is logged</param>
    /// <param name="numRetriesBeforeTerminate">Number of allowed failures before the program is terminated; null = Infinite. Defaults to null</param>
    public WatchDog(
        TimeSpan? watchInterval = null,
        int numRetriesBeforeLog = 3,
        int? numRetriesBeforeTerminate = null
    )
    {
        this._pingCount = 0;
        this._healthy = 1;
        var interval = watchInterval ?? TimeSpan.FromMilliseconds(DefaultWatchDogThreadMs);
        this._watchDogThread = new Thread(Watch(interval, numRetriesBeforeLog, numRetriesBeforeTerminate));
        _watchDogThread.Start();
    }

    /// <summary>
    /// Clean up the watch dog
    /// </summary>
    ~WatchDog()
    {
        Dispose();
    }

    /// <summary>
    /// Clean up the watch dog and it's threads
    /// </summary>
    public void Dispose()
    {
        if (Interlocked.Exchange(ref _continueChecking, 0) == 1)
        {
            _watchDogThread.Join();
            foreach (var periodicCheck in _periodicChecks)
            {
                periodicCheck.Join();
            }
        }
    }

    /// <summary>
    /// Checks if the system is healthy
    /// </summary>
    /// <returns>Whether the system is Healthy</returns>
    public bool IsHealthy()
    {
        return Interlocked.Read(ref _healthy) == 1;
    }

    /// <summary>
    /// Check if the watch dog is running
    /// </summary>
    /// <returns>Whether the watch dog is running</returns>
    public bool Running()
    {
        return Interlocked.Read(ref _continueChecking) == 1;
    }

    /// <summary>
    /// Send the watch dog a keep-alive puls
    /// </summary>
    public void Pulse()
    {
        Interlocked.Increment(ref _pingCount);
    }

    /// <summary>
    /// Starts a new periodic check thread
    /// </summary>
    /// <param name="periodicCheckMethod">
    /// Method that takes an action to run.
    /// The Method is responsible for ensuring the check is ran in the right context and that the application is in a good state.
    /// The action will send a pulse to the watch dog.
    /// </param>
    /// <param name="watchInterval">
    /// Inerval for calling the periodic check method
    /// </param>
    /// <returns>Thread for periodic check (already started)</returns>
    public Thread PeriodicCheckMethod(Action<Action> periodicCheckMethod, TimeSpan? watchInterval = null)
    {
        var thread = new Thread(PeriodicCheck(periodicCheckMethod, watchInterval));
        thread.Start();
        _periodicChecks.Add(thread);
        return thread;
    }

    /// <summary>
    /// Runs a periodic check
    /// </summary>
    /// <param name="periodicCheckMethod">Method to pass "Pulse" to when doing th check</param>
    /// <param name="watchInterval">Interval to run</param>
    /// <returns></returns>
    private ThreadStart PeriodicCheck(Action<Action> periodicCheckMethod, TimeSpan? watchInterval)
    {
        return () =>
        {
            // Default to pulsating twice per check to make sure we get in at least once
            var interval = watchInterval ?? TimeSpan.FromMilliseconds(DefaultWatchDogThreadMs / 2);
            while (Interlocked.Read(ref _continueChecking) == 1)
            {
                Thread.Sleep(interval);
                periodicCheckMethod(Pulse);
            }
        };
    }

    /// <summary>
    /// Starts the watch dog's watch thread
    /// </summary>
    /// <param name="interval">Interval to check</param>
    /// <param name="numRetriesBeforeLog">Number of retries before logging a failure</param>
    /// <param name="numRetriesBeforeTerminate"></param>
    /// <returns></returns>
    private ThreadStart Watch(TimeSpan interval, int numRetriesBeforeLog, int? numRetriesBeforeTerminate)
    {
        return () =>
        {
            int failures = 0;
            Thread.Sleep(interval);
            while (Interlocked.Read(ref _continueChecking) == 1)
            {
                var oldPing = Interlocked.Exchange(ref _pingCount, 0);
                if (oldPing > 0)
                {
                    Interlocked.Exchange(ref _healthy, 1);
                    failures = 0;
                    Log.Logger.Verbose("Program is alive!");
                }
                else
                {
                    ++failures;
                    Log.Logger.Verbose("Program is frozen!");
                }

                if (failures == numRetriesBeforeLog)
                {
                    Interlocked.Exchange(ref _healthy, 0);
                    Log.Logger.Error("Watchdog triggered! Program has frozen!");
                }
                else if (failures == numRetriesBeforeTerminate)
                {
                    Log.Logger.Fatal("Program has been frozen too long! Terminating!");
                    Environment.Exit(1);
                }
                Thread.Sleep(interval);
            }
        };
    }
}
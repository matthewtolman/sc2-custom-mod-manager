namespace SC2_CCM_Common;

public class WatchDog
{
    private readonly Thread _watchDogThread;
    private int _pingCount;

    public WatchDog(
        TimeSpan? watchInterval = null,
        int numRetriesBeforeLog = 3,
        int? numRetriesBeforeTerminate = 1000
    )
    {
        this._pingCount = 0;
        var interval = watchInterval ?? new TimeSpan(0, 0, 0, 0, 500);
        this._watchDogThread = new Thread(Watch(interval, numRetriesBeforeLog, numRetriesBeforeTerminate));
    }

    public void Alive()
    {
        Interlocked.Increment(ref _pingCount);
    }

    public Thread PeriodicCheckMethod(Action<Action> periodicCheckMethod, TimeSpan? watchInterval = null)
    {
        return new Thread(PeriodicCheck(periodicCheckMethod, watchInterval));
    }

    private ThreadStart PeriodicCheck(Action<Action> periodicCheckMethod, TimeSpan? watchInterval)
    {
        return () =>
        {
            var interval = watchInterval ?? new TimeSpan(0, 0, 0, 0, 250);
            while (true)
            {
                Thread.Sleep(interval);
                periodicCheckMethod(Alive);
            }
        };
    }

    private ThreadStart Watch(TimeSpan interval, int numRetriesBeforeLog, int? numRetriesBeforeTerminate)
    {
        return () =>
        {
            int failures = 0;
            while (true)
            {
                Thread.Sleep(interval);
                var oldPing = Interlocked.Exchange(ref _pingCount, 0);
                if (oldPing > 0)
                {
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
                    Log.Logger.Error("Watchdog triggered! Program has frozen!");
                }
                else if (failures == numRetriesBeforeTerminate)
                {
                    Log.Logger.Fatal("Program has been frozen too long! Terminating!");
                    Environment.Exit(1);
                }
            }
        };
    }
}
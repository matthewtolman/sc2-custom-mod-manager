using SC2_CCM_Common;

namespace SC2_CCM_Common_Test;

public class WatchDogTest
{
    [Fact]
    public void WatchDogDetectsBadHealth()
    {
        using (var watchDog = new WatchDog(
                   watchInterval: TimeSpan.FromMilliseconds(20),
                   numRetriesBeforeLog: 3
               ))
        {
            watchDog.Pulse();
            Assert.True(watchDog.IsHealthy());
            Thread.Sleep(100);
            Assert.False(watchDog.IsHealthy());
            watchDog.Pulse();
            Thread.Sleep(30);
            watchDog.Pulse();
            Assert.True(watchDog.IsHealthy());
        }
    }
    
    [Fact]
    public void WatchDogDetectsAliveCheckWorks()
    {
        using (var watchDog = new WatchDog(
                   watchInterval: TimeSpan.FromMilliseconds(20),
                   numRetriesBeforeLog: 1
               ))
        {
            Thread.Sleep(60);
            Assert.False(watchDog.IsHealthy());
            watchDog.PeriodicCheckMethod((action) => { action(); }, TimeSpan.FromMilliseconds(10));
            Thread.Sleep(40);
            Assert.True(watchDog.IsHealthy());
        }
    }
}
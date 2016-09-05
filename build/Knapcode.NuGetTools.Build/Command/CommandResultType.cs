namespace Knapcode.NuGetTools.Build
{
    public enum CommandStatus
    {
        FailedToStartCommand,
        Timeout,
        FailedToKillAfterTimeout,
        Exited
    }
}

namespace uSync.Complete.HangfireJobs.Restore;

public interface ISyncRestorePointScheduler
{
    void CreateRestorePoint(string name);
}
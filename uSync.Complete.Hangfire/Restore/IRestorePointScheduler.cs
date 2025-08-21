
namespace uSync.Complete.Hangfire.Restore;

public interface IRestorePointScheduler
{
    Task<bool> CreateRestorePoint(string name, bool includeMedia);
}
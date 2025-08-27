
namespace uSync.Hangfire.Actions;

public interface ISyncActionScheduler
{
    Task<bool> Export(string group);
    Task<bool> Export(string folder, string set, string group);
    Task<bool> Export(string folder, string[] entityTypes);
    Task<bool> Import(string group, bool force);
    Task<bool> Import(string[] folders, string set, string group, bool force);
    Task<bool> Import(string folder, string[] entityTypes, bool force);
}
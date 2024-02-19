using uSync.Core.Dependency;
using uSync.Expansions.Core;

using static Umbraco.Cms.Core.Constants;

namespace uSync.Complete.HangfireJobs.Publisher;

/// <summary>
///  options to pass to any publisher job that gets scheduled
/// </summary>
public class SyncSchedulerOptions
{
    /// <summary>
    ///  any dependency flags to set on the job 
    /// </summary>
    public DependencyFlags DependencyFlags { get; set; } = DependencyFlags.None;
    
    /// <summary>
    ///  type of item we are syncing
    /// </summary>
    public string EntityType { get; set; } = UdiEntityType.Document;

    /// <summary>
    ///  do we check that the server is up and ready before queueing the job
    /// </summary>
    public bool CheckForServer { get; set; } = true;

    /// <summary>
    ///  options to pass the publish job. 
    /// </summary>
    public SyncPackOptions Options { get; set; } = new()
    {
        CreateRestorePoint = false,
        IncludeMediaFiles = true,
        PrimaryType = UdiEntityType.Document,
        RemoveOrphans = true,
    };
}
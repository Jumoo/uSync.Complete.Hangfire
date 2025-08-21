using Jumoo.Processing.Core.Processing.Interfaces;

namespace uSync.Complete.Hangfire;
public interface IProcessingScheduler
{
    Task<bool> ProcessPipeline(string name, string strategy, IProcessingOptions? options);
}
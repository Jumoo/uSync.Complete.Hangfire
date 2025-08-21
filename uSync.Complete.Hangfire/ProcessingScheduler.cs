using Jumoo.Json;
using Jumoo.Processing.Core.Pipelines;
using Jumoo.Processing.Core.Pipelines.Models;
using Jumoo.Processing.Core.Processing.Interfaces;
using Jumoo.Processing.Core.Queue.Models;

using Microsoft.Extensions.Logging;

using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models.Membership;
using Umbraco.Cms.Core.Services;

namespace uSync.Complete.Hangfire;
internal class ProcessingScheduler : IProcessingScheduler
{
    private readonly IPipelineService _pipelineService;
    private readonly IEventAggregator _eventAggregator;
    private readonly ILogger<ProcessingScheduler> _logger;
    private readonly IUserService _userService;

    public ProcessingScheduler(
        IPipelineService pipelineService,
        IEventAggregator eventAggregator,
        IUserService userService,
        ILogger<ProcessingScheduler> logger)
    {
        _pipelineService = pipelineService;
        _eventAggregator = eventAggregator;
        _userService = userService;
        _logger = logger;
        
        
    }

    public async Task<bool> ProcessPipeline(string name, string strategy, IProcessingOptions? options)
    {
        var user = await GetUser();

        var pipeline = await CreatePipeline(name, strategy, user);
        if (pipeline == null) return false;

        if (options is not null)
            await _pipelineService.UpdateOptions(pipeline.Value, options, user);

        return await ProgressPipeline(pipeline.Value, string.Empty, user);
    }

    private async Task<Guid?> CreatePipeline(string name, string strategy, IUser? user)
    {
        _logger.LogInformation("Creating uSync restore point: {name} {strategy} {user}", name, strategy, user?.Name ?? "(Unknown)");
        var pipeline = await _pipelineService.CreatePipeline(new CreatePipelineOptions
        {
            Alias = name,
            Strategy = strategy,
            IsInBackground = true,
            User = user
        });

        return pipeline?.Id;
    }

    private async Task<bool> ProgressPipeline(Guid id, string clientId, IUser? user)
    {
        IPipeline? pipeline = await _pipelineService.GetPipeline(id, user);

        // protection counter, just stops the process from killing the server
        // should it get stuck in a loop. 
        int protectionCount = 0;
        do
        {
            _logger.LogInformation("Processing: {PipelineId} {status} for user: {UserName}", id,
                pipeline?.State.Status ?? PipelineStatus.NotStarted,
                user?.Name ?? "Unknown");

            protectionCount++;
            pipeline = await _pipelineService.Process(id, user, clientId, true);
        } while (protectionCount < 1000 && pipeline?.State.Status == PipelineStatus.Running);

        return pipeline?.State.Status == PipelineStatus.Completed;
    }

    private async Task<bool> PostToBackground(Guid id, IUser? user)
    {
        var pipeline = await _pipelineService.GetPipeline(id, user);

        if (pipeline == null)
            return false;

        pipeline.State.BackgroundState ??= new PipelineBackgroundState
        {
            IsBackgroundRequest = true,
        };

        pipeline.State.Status = PipelineStatus.Background;

        _eventAggregator.Publish(
            new QueueRequestNotification(
               new QueuedItem
               {
                   Key = pipeline.Id,
                   Processor = pipeline.Alias,
                   Strategy = pipeline.Strategy,
                   Immediate = true,
                   Scheduled = DateTime.MinValue,
                   Data = pipeline.SerializeJsonString(),
                   UserKey = user?.Key ?? Guid.Empty,
                   UserName = user?.Name ?? string.Empty,
                   ClientId = null,
                   Note = "Triggered via hangfire."
               }));

        return true;
    }

    private async Task<IUser?> GetUser()
        => await _userService.GetAsync(Constants.Security.SuperUserKey);
}

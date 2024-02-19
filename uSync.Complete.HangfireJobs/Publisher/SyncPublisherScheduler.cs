using System.Diagnostics.CodeAnalysis;

using Microsoft.Extensions.Logging;

using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Services;
using Umbraco.Extensions;

using uSync.Core.Sync;
using uSync.Expansions.Core.ItemManagers;
using uSync.Publisher.Models;
using uSync.Publisher.Publishers;
using uSync.Publisher.Services;

using static Umbraco.Cms.Core.Constants;

namespace uSync.Complete.HangfireJobs.Publisher;
public class SyncPublisherScheduler : ISyncPublisherScheduler
{
    private readonly ILogger<SyncPublisherScheduler> _logger;

    private readonly IContentService _contentService;
    private readonly IMediaService _mediaService;

    private readonly IUserService _userService;

    private readonly SyncItemManagerCollection _itemManagers;
    private readonly ISyncPublisherActionService _syncActionService;
    private readonly SyncServerService _syncServerService;
    private readonly SyncPublisherFactory _publisherFactory;

    public SyncPublisherScheduler(
        ILogger<SyncPublisherScheduler> logger,
        IContentService contentService,
        IMediaService mediaService,
        IUserService userService,
        SyncItemManagerCollection itemManagers,
        ISyncPublisherActionService syncActionService,
        SyncServerService syncServerService,
        SyncPublisherFactory publisherFactory)
    {
        _logger = logger;
        _contentService = contentService;
        _mediaService = mediaService;
        _userService = userService;

        _itemManagers = itemManagers;
        _syncActionService = syncActionService;
        _syncServerService = syncServerService;
        _publisherFactory = publisherFactory;
    }

    /// <inheritdoc />
    public async Task<bool> PublishContent(
        int contentId,
        string serverAlias,
        PublishMode mode,
        SyncSchedulerOptions options,
        int userId = -1)
    {
        options.EntityType = UdiEntityType.Document;

        var item = new SyncItem
        {
            Flags = options.DependencyFlags
        };

        if (contentId != -1)
        {
            var contentItem = _contentService.GetById(contentId);
            if (contentItem == null)
            {
                _logger.LogWarning("Unable to find content item with id {id} to start publish", contentId);
                return false;
            }

            item.Udi = contentItem.GetUdi();
            item.Name = contentItem.Name;
        }
        else
        {
            item.Udi = Udi.Create(options.EntityType);
            item.Name = "Content root";
        }

        return await PublishItem(item, serverAlias, mode, options, userId);
    }


    public async Task<bool> PublishMedia(
        int mediaId, 
        string serverAlias,
        PublishMode mode,
        SyncSchedulerOptions options,
        int userId = -1)
    {
        options.EntityType = UdiEntityType.Media;

        var item = new SyncItem
        {
            Flags = options.DependencyFlags
        };

        if (mediaId != -1)
        {
            var contentItem = _mediaService.GetById(mediaId);
            if (contentItem == null)
            {
                _logger.LogWarning("Unable to find Media item with id {id} to start publish", mediaId);
                return false;
            }

            item.Udi = contentItem.GetUdi();
            item.Name = contentItem.Name;
        }
        else
        {
            item.Udi = Udi.Create(options.EntityType);
            item.Name = "Media root";
        }

        return await PublishItem(item, serverAlias, mode, options, userId);
    }

    /// <inheritdoc/>
    public async Task<bool> PublishItem(
        SyncItem item,
        string serverAlias,
        PublishMode mode,
        SyncSchedulerOptions options,
        int userId)
    {
        var itemManager = _itemManagers.GetByEntityType(options.EntityType);
        var items = itemManager.GetItems(item);

        return await PublishItems(items, serverAlias, mode, options, userId);
    }

    /// <inheritdoc />
    public async Task<bool> PublishItems(
        IEnumerable<SyncItem> items,
        string serverAlias,
        PublishMode mode,
        SyncSchedulerOptions options,
        int userId)
    {
        if (TryGetBackgroundPublisher(serverAlias, out var publisher) is false)
        {
            _logger.LogWarning("Unable to get publisher for the server {server}", serverAlias);
            return false;
        }

        _logger.LogDebug("Background: Using {publisher}", publisher.Name);

        if (options.CheckForServer)
        {
            await EnsureServer(publisher, serverAlias);
        }

        if (TryGetQueueAction(publisher, mode, out var queueAction) is false)
        {
            _logger.LogWarning("Unable to find the queue action on the publisher {publisher}", publisher.Name);
            return false;
        }

        _logger.LogDebug("Background: Using Action {action}", queueAction.Alias);

        var user = _userService.GetUserById(userId);
        var request = new PublisherActionRequest
        {
            Server = serverAlias,
            ActionAlias = queueAction.Alias,
            Items = items,
            Mode = mode,
            User = user?.Username ?? "(Background)",
            Options = options.Options
        };


        _logger.LogDebug("Background: Perform Action: {server} {action} {items}",
            serverAlias, queueAction.Alias, items.Count());

        // ensure the primary type is the same as what we started with.
        request.Options.PrimaryType = options.EntityType;

        var result = await _syncActionService.PerformAction(request, user);

        return result.Success;
    }

    /// <summary>
    ///  get the background publisher 
    /// </summary>
    /// <param name="serverAlias"></param>
    /// <param name="publisher"></param>
    /// <returns></returns>
    private bool TryGetBackgroundPublisher(string serverAlias, [MaybeNullWhen(false)] out ISyncPublisher publisher)
    {
        publisher = default;

        var server = _syncServerService.GetServer(serverAlias);
        if (server == null)
        {
            _logger.LogWarning("Unable to find server {serverAlias}", serverAlias);
            return false;
        }

        publisher = _publisherFactory.GetPublisher(serverAlias);
        if (publisher == null || publisher.Alias.Equals("background", StringComparison.OrdinalIgnoreCase) is false)
        {
            _logger.LogWarning("Publisher is not the background publisher, you can't schedule the '{name}' publisher",
                publisher?.Alias ?? "Not found");
            publisher = default;
            return false;
        }

        return true;
    }

    /// <summary>
    ///  try to get the first action to queue things.
    /// </summary>
    /// <remarks>
    ///  the queue alias is usually. "queue-add" but by looking for the first
    ///  non-view action in the background queue, we are guarding against us
    ///  renaming the action.
    /// </remarks>
    private bool TryGetQueueAction(ISyncPublisher publisher, PublishMode mode, [MaybeNullWhen(false)] out PublisherAction action)
    {
        action = default;
        if (publisher.Actions.ContainsKey(mode) is false) return false;
        action = publisher.Actions[mode].FirstOrDefault(x => string.IsNullOrWhiteSpace(x.View));
        return action != default;
    }

    /// <summary>
    ///  ensure that the target server is up. 
    /// </summary>
    /// <remarks>
    ///  while technically this doesn't matter for background jobs 
    ///  (the server can be down when the job is scheduled).
    ///  
    ///  Its safer to check, because the job is just going to fail if the 
    ///  server isn't there.
    /// </remarks>
    private async Task EnsureServer(ISyncPublisher publisher, string serverAlias)
    {
        var serverStatus = await publisher.GetStatus(serverAlias);
        if (serverStatus != SyncServerStatus.Success)
        {
            _logger.LogError("Server {server} is not available {status}", serverAlias, serverStatus);
            throw new KeyNotFoundException($"Server {serverAlias} is unavailable {serverStatus}");
        }

        return;

    }
}

using uSync.BackOffice;
using uSync.BackOffice.Configuration;
using uSync.BackOffice.SyncHandlers;
using uSync.BackOffice.SyncHandlers.Models;

namespace uSync.Hangfire.Actions;
internal class SyncActionScheduler : ISyncActionScheduler
{
    private readonly ISyncService _syncService;
    private readonly ISyncConfigService _syncConfigService;
    private readonly ISyncHandlerFactory _syncHandlerFactory;

    public SyncActionScheduler(
        ISyncService syncService,
        ISyncConfigService syncConfigService,
        ISyncHandlerFactory syncHandlerFactory)
    {
        _syncService = syncService;
        _syncConfigService = syncConfigService;
        _syncHandlerFactory = syncHandlerFactory;
    }

    public async Task<bool> Export(string group)
    {
        var folder = _syncConfigService.GetFolders().Last();
        var set = _syncConfigService.Settings.DefaultSet;
        return await Export(folder, set, group);
    }


    public async Task<bool> Export(string folder, string set, string group)
    {
        var options = new SyncHandlerOptions
        {
            Action = HandlerActions.Export,
            Group = group,
            Set = _syncConfigService.Settings.DefaultSet
        };

        var handlers = _syncHandlerFactory.GetValidHandlers(options);

        var result = await _syncService.ExportAsync(folder, handlers, null);
        return result.ContainsErrors() == false;
    }

    public async Task<bool> Export(string folder, string[] entityTypes) 
    {
        var options = new SyncHandlerOptions
        {
            Action = HandlerActions.Export,
            Group = "All",
            Set = "Default",    
        };

        var validHandlers = _syncHandlerFactory.GetValidHandlersByEntityType(entityTypes, options);

        var result = await _syncService.ExportAsync(folder, validHandlers, null);
        return result.ContainsErrors() == false;

    }

    public async Task<bool> Import(string group, bool force)
    {
        var folders = _syncConfigService.GetFolders();
        var set = _syncConfigService.Settings.DefaultSet;

        return await Import(folders, set, group, force);
    }

    public async Task<bool> Import(string[] folders, string set, string group, bool force)
    {
        var options = new SyncHandlerOptions
        {
            Action = HandlerActions.Import,
            Group = group,
            Set = set,
        };

        var handlers = _syncHandlerFactory.GetValidHandlers(options);

        var result = await _syncService.ImportAsync(folders, force, handlers, options, null);
        return result.ContainsErrors() == false;
    }

    public async Task<bool> Import(string folder, string[] entityTypes, bool force)
    {
        var options = new SyncHandlerOptions
        {
            Action = HandlerActions.Import,
            Group = "All",
            Set = "Default",
        };

        var validHandlers = _syncHandlerFactory.GetValidHandlersByEntityType(entityTypes, options);

        var result = await _syncService.ImportAsync(new[] { folder }, force, validHandlers, options, null);
        return result.ContainsErrors() == false;
    }
}

using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.External;
using SPTarkov.Server.Core.Models.Logging;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Servers;

namespace _14AfterDBLoadHook;

public record ModMetadata : AbstractModMetadata
{
    public override string ModGuid { get; init; } = "com.sp-tarkov.examples.afterdbhook";
    public override string Name { get; init; } = "AfterDBLoadHookExample";
    public override string Author { get; init; } = "SPTarkov";
    public override List<string>? Contributors { get; set; }
    public override SemanticVersioning.Version Version { get; } = new("1.0.0");
    public override SemanticVersioning.Version SptVersion { get; } = new("4.0.0");
    public override List<string>? LoadBefore { get; set; }
    public override List<string>? LoadAfter { get; set; }
    public override List<string>? Incompatibilities { get; set; }
    public override Dictionary<string, SemanticVersioning.Version>? ModDependencies { get; set; }
    public override string? Url { get; set; }
    public override bool? IsBundleMod { get; set; }
    public override string? License { get; init; } = "MIT";
}


/// <summary>
/// <b>*** OBSOLETE WARNING! ***</b>
/// <br/>
/// Interfaces <i>IPostDBLoadMod</i> and <i>IPostSptLoadMod</i> used to be used in TS to load mods after the database finished loading.
/// Although still provided as Async variants, these are now deprecated and should not be used.
/// They will be removed in version 4.1.0 - please use <i>IOnLoad</i> instead with the desired Injectable(TypePriority) as below:
/// </summary>
/// <code>
/// [Injectable(TypePriority = OnLoadOrder.Database + 1)]
/// public class MyMod : IOnLoad
/// {
///   // ... implementation
/// }
/// </code>
[Injectable]
public class AfterDBLoadHook(
    DatabaseServer databaseServer,
    ISptLogger<AfterDBLoadHook> logger)
    : IPostDBLoadModAsync, IPostSptLoadModAsync
{
    private Dictionary<MongoId, TemplateItem>? _itemsDb;

    public Task PostDBLoadAsync()
    {
        _itemsDb = databaseServer.GetTables().Templates.Items;

        // Database will be loaded, this is the fresh state of the DB so NOTHING from the SPT
        // logic has modified anything yet. This is the DB loaded straight from the JSON files
        logger.LogWithColor($"Database item size: {_itemsDb.Count}", LogTextColor.Red, LogBackgroundColor.Yellow);

        // lets do a quick modification and see how this reflect later on, on the postSptLoad()
        // find the nvgs item by its Id
        // this also checks if the item exists before giving you the item
        // if it doesn't, this if check will fail
        if (_itemsDb.TryGetValue("5c0558060db834001b735271", out var nvgs))
        {
            // Lets log the state before the modification
            logger.LogWithColor($"NVGs default CanSellOnRagfair: {nvgs.Properties.CanSellOnRagfair}", LogTextColor.Red, LogBackgroundColor.Yellow);
            // Update one of its properties to be true
            nvgs.Properties.CanSellOnRagfair = true;
        }
        
        return Task.CompletedTask;
    }

    public Task PostSptLoadAsync()
    {
        // The modification we made above would have been processed by now by SPT, so any values we changed had
        // already been passed through the initial lifecycles (OnLoad) of SPT.

        if (_itemsDb.TryGetValue("5c0558060db834001b735271", out var nvgs))
        {
            // Lets log the state after the modification
            logger.LogWithColor($"NVGs default CanSellOnRagfair: {nvgs.Properties.CanSellOnRagfair}", LogTextColor.Red, LogBackgroundColor.Yellow);
        }
        
        return Task.CompletedTask;
    }
}

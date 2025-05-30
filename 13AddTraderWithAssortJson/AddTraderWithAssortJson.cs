using System.Reflection;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Spt.Config;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Routers;
using SPTarkov.Server.Core.Servers;
using SPTarkov.Server.Core.Services;
using SPTarkov.Server.Core.Utils.Cloners;
using Path = System.IO.Path;

namespace _13AddTraderWithAssortJson;

public record ModMetadata : AbstractModMetadata
{
    public override string Name { get; set; } = "AddTraderWithAssortJsonExample";
    public override string Author { get; set; } = "SPTarkov";
    public override List<string>? Contributors { get; set; }
    public override string Version { get; set; } = "1.0.0";
    public override string SptVersion { get; set; } = "4.0.0";
    public override List<string>? LoadBefore { get; set; }
    public override List<string>? LoadAfter { get; set; }
    public override List<string>? Incompatibilities { get; set; }
    public override Dictionary<string, string>? ModDependencies { get; set; }
    public override string? Url { get; set; }
    public override bool? IsBundleMod { get; set; }
    public override string? Licence { get; set; } = "MIT";
}

[Injectable(TypePriority = OnLoadOrder.PostDBModLoader + 1)]
public class AddTraderWithAssortJson : IOnLoad
{
    private readonly ISptLogger<AddTraderWithAssortJson> _logger;
    private readonly ModHelper _modHelper;
    private readonly DatabaseService _databaseService;
    private readonly ImageRouter _imageRouter;
    private readonly ConfigServer _configServer;
    private readonly ICloner _cloner;
    private readonly TraderConfig _traderConfig;
    private readonly RagfairConfig _ragfairConfig;

    public AddTraderWithAssortJson(
        ISptLogger<AddTraderWithAssortJson> logger,
        ModHelper modHelper,
        DatabaseService databaseService,
        ImageRouter imageRouter,
        ConfigServer configServer,
        ICloner cloner)
    {
        _logger = logger;
        _modHelper = modHelper;
        _databaseService = databaseService;
        _imageRouter = imageRouter;
        _configServer = configServer;
        _cloner = cloner;

        _traderConfig = _configServer.GetConfig<TraderConfig>();
        _ragfairConfig = _configServer.GetConfig<RagfairConfig>();
    }

    public Task OnLoad()
    {
        var pathToMod = _modHelper.GetAbsolutePathToModFolder(Assembly.GetExecutingAssembly());

        var traderImagePath = Path.Combine(pathToMod, "db/cat.jpg");
        var traderBase = _modHelper.GetJsonDataFromFile<TraderBase>(pathToMod, "db/base.json");

        var assort = _modHelper.GetJsonDataFromFile<TraderAssort>(pathToMod, "db/assort.json");

        // Create helper class and use it to register our traders image/icon + set its stock refresh time
        var addTraderHelper = new AddTraderHelper();
        _imageRouter.AddRoute(traderBase.Avatar.Replace(".jpg", ""), traderImagePath);
        addTraderHelper.SetTraderUpdateTime(_traderConfig, traderBase, 3600, 4000);

        // Add trader to flea market
        _ragfairConfig.Traders[traderBase.Id] = true;

        // Add new trader to the trader dictionary in DatabaseServer - this is where the assort json is loaded
        /*
         * The assortJSON includes the following:
         * Milk available for roubles at LL1
         * Milk available for item barter at LL1
         * Helmet w/ soft armour available for roubles at LL1
         * Helmet w/ soft armour available for item barter at LL1
         *
         * It is *REQUIRED* to use MongoIDs for IDs in the assort
         */
        addTraderHelper.AddTraderToDb(
            traderBase,
            _databaseService.GetTables(),
            _cloner,
            assort);

        addTraderHelper.AddTraderToLocales(
            traderBase,
            _databaseService.GetTables(),
            traderBase.Name,
            "Cat",
            traderBase.Nickname,
            traderBase.Location,
            "This is the cat shop. Meow.");
        
        return Task.CompletedTask;
    }
}

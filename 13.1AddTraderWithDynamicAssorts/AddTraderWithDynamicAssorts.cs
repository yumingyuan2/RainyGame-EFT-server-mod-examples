using System.Reflection;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Enums;
using SPTarkov.Server.Core.Models.Spt.Config;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Routers;
using SPTarkov.Server.Core.Servers;
using SPTarkov.Server.Core.Services;
using SPTarkov.Server.Core.Utils;
using SPTarkov.Server.Core.Utils.Cloners;
using Path = System.IO.Path;

namespace _13._1AddTraderWithDynamicAssorts;

public record ModMetadata : AbstractModMetadata
{
    public override string Name { get; set; } = "AddTraderWithDynamicAssortsExample";
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

// This line tells the class to load right after "PostDBModLoader" occurs
[Injectable(TypePriority = OnLoadOrder.PostDBModLoader + 1)]
public class AddTraderWithDynamicAssorts : IOnLoad
{
    private readonly ISptLogger<AddTraderWithDynamicAssorts> _logger;
    private readonly ModHelper _modHelper;
    private readonly HashUtil _hashUtil;
    private readonly DatabaseService _databaseService;
    private readonly ImageRouter _imageRouter;
    private readonly ConfigServer _configServer;
    private readonly LocaleService _localeService;
    private readonly ICloner _cloner;
    private readonly TraderConfig _traderConfig;
    private readonly RagfairConfig _ragfairConfig;

    // This MUST match the folder name of the mod in the user/mods folder
    private const string _modName = "13.1AddTraderWithDynamicAssorts";

    public AddTraderWithDynamicAssorts(
        ISptLogger<AddTraderWithDynamicAssorts> logger,
        ModHelper modHelper,
        HashUtil hashUtil,
        DatabaseService databaseService,
        ImageRouter imageRouter,
        ConfigServer configServer,
        LocaleService localeService,
        ICloner cloner        )
    {
        _logger = logger;
        _modHelper = modHelper;
        _hashUtil = hashUtil;
        _databaseService = databaseService;
        _imageRouter = imageRouter;
        _configServer = configServer;
        _localeService = localeService;
        _cloner = cloner;

        _traderConfig = _configServer.GetConfig<TraderConfig>();
        _ragfairConfig = _configServer.GetConfig<RagfairConfig>();
    }
    
    public Task OnLoad()
    {
        // A path to the mods files we use below
        var pathToMod = _modHelper.GetAbsolutePathToModFolder(Assembly.GetExecutingAssembly());

        // A relative path to the trader icon to show
        var traderImagePath = Path.Combine(pathToMod, "db/cat.jpg");

        // The base json containing trader settings we will add to the server
        var traderBase = _modHelper.GetJsonDataFromFile<TraderBase>(pathToMod, "db/base.json");

        // Create a helper class and use it to register our traders image/icon + set its stock refresh time
        var addTraderHelper = new AddTraderHelper(_localeService);
        _imageRouter.AddRoute(traderBase.Avatar.Replace(".jpg", ""), traderImagePath);
        addTraderHelper.SetTraderUpdateTime(_traderConfig, traderBase, 3600, 4000);

        // Add our trader to the config list, this lets it be seen by the flea market
        _ragfairConfig.Traders.TryAdd(traderBase.Id, true);

        // Get the database files
        var dbTables = _databaseService.GetTables();

        // Create an empty assort ready for our items
        var emptyTraderItemAssortObject = new TraderAssort
        {
            Items = [],
            BarterScheme = new Dictionary<string, List<List<BarterScheme>>>(),
            LoyalLevelItems = new Dictionary<string, int>()
        };

        // Add our trader (with no items yet) to the server database
        addTraderHelper.AddTraderToDb(
            traderBase,
            dbTables,
            _cloner,
            emptyTraderItemAssortObject
            );

        // Add localisation text for our trader to the database so it shows to people playing in different languages
        addTraderHelper.AddTraderToLocales(
            traderBase,
            _databaseService.GetTables(),
            traderBase.Name,
            "Cat",
            traderBase.Nickname,
            traderBase.Location,
            "This is the cat shop. Meow.");

        //Create a helper class to assist us with making items for our trader
        // It's called 'fluent' as it's a technical term to describe how it can "chain" methods together
        var fluentAssortCreator = new FluentTraderAssortCreator(_logger, _hashUtil);

        // Add a single "milk" for 2000 roubles that player can buy a max of 10 per refresh
        fluentAssortCreator
            .CreateSingleAssortItem(ItemTpl.DRINK_PACK_OF_MILK)
            .AddStackCount(200)
            .AddBuyRestriction(10)
            .AddMoneyCost(Money.ROUBLES, 2000)
            .AddLoyaltyLevel(1)
            .Export(dbTables.Traders.GetValueOrDefault(traderBase.Id));

        // Add a 3x bitcoin + salewa for milk barter
        fluentAssortCreator
            .CreateSingleAssortItem(ItemTpl.DRINK_PACK_OF_MILK)
            .AddStackCount(100)
            .AddBarterCost(ItemTpl.BARTER_PHYSICAL_BITCOIN, 3)
            .AddBarterCost(ItemTpl.MEDKIT_SALEWA_FIRST_AID_KIT, 1)
            .AddLoyaltyLevel(1)
            .Export(dbTables.Traders.GetValueOrDefault(traderBase.Id));


        // Add glock as a rouble purchase
        fluentAssortCreator
            .CreateComplexAssortItem(addTraderHelper.CreateGlock())
            .AddUnlimitedStackCount()
            .AddMoneyCost(Money.ROUBLES, 20000)
            .AddBuyRestriction(3)
            .AddLoyaltyLevel(1)
            .Export(dbTables.Traders.GetValueOrDefault(traderBase.Id));

        // Add mp133 preset as a barter for mayonnaise
        var mp133BarterId = "584148f2245977598f1ad387"; // This preset id comes from globals.json
        fluentAssortCreator
            .CreateComplexAssortItem(dbTables.Globals.ItemPresets.GetValueOrDefault(mp133BarterId).Items) 
            .AddStackCount(200)
            .AddBarterCost(ItemTpl.FOOD_JAR_OF_DEVILDOG_MAYO, 1)
            .AddBuyRestriction(3)
            .AddLoyaltyLevel(1)
            .Export(dbTables.Traders.GetValueOrDefault(traderBase.Id));

        // Happy little log message
        _logger.Success("Added Cat trader to server");

        // Send back a success to the server to say our trader is good to go
        return Task.CompletedTask;
    }
}

public static class NewItemIds
{
    public static string GLOCK_BASE = "66eeef3b2a166b73d2066a74";
    public static string GLOCK_BARREL = "66eeef3b2a166b73d2066a75";
    public static string GLOCK_RECIEVER = "66eeef3b2a166b73d2066a76";
    public static string GLOCK_COMPENSATOR = "66eeef3b2a166b73d2066a77";
    public static string GLOCK_PISTOL_GRIP = "66eeef3b2a166b73d2066a78";
    public static string GLOCK_REAR_SIGHT = "66eeef3b2a166b73d2066a79";
    public static string GLOCK_FRONT_SIGHT = "66eeef3b2a166b73d2066a7a";
    public static string GLOCK_MAGAZINE = "66eeef3b2a166b73d2066a7b";
}

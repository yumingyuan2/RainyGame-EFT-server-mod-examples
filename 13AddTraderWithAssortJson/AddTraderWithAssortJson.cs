﻿using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Spt.Config;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Routers;
using SPTarkov.Server.Core.Servers;
using SPTarkov.Server.Core.Utils;
using System.Reflection;
using Path = System.IO.Path;

namespace _13AddTraderWithAssortJson;

// This record holds the various properties for your mod
public record ModMetadata : AbstractModMetadata
{
    public override string ModGuid { get; set; } = "com.sp-tarkov.examples.addtraderjsonassorts";
    public override string Name { get; set; } = "AddTraderWithAssortJsonExample";
    public override string Author { get; set; } = "SPTarkov";
    public override List<string>? Contributors { get; set; } = ["Clodan", "CWX"];
    public override string Version { get; set; } = "1.0.0";
    public override string SptVersion { get; set; } = "4.0.0";
    public override List<string>? LoadBefore { get; set; } = ["AddTraderWithDynamicAssortsExample"];
    public override List<string>? LoadAfter { get; set; } = ["RegisterClassesInDIExample"];
    public override List<string>? Incompatibilities { get; set; } = ["ReadJsonConfigExample"];
    public override Dictionary<string, string>? ModDependencies { get; set; }
    public override string? Url { get; set; } = "https://github.com/sp-tarkov/server-mod-examples";
    public override bool? IsBundleMod { get; set; } = false;
    public override string? Licence { get; set; } = "MIT";
}

/// <summary>
/// Feel free to use this as a base for your mod
/// </summary>
[Injectable(TypePriority = OnLoadOrder.PostDBModLoader + 1)]
public class AddTraderWithAssortJson(
    ModHelper modHelper,
    ImageRouter imageRouter,
    ConfigServer configServer,
    TimeUtil timeUtil,
    AddCustomTraderHelper addCustomTraderHelper // This is a custom class we add for this mod, we made it injectable so it can be accessed like other classes here
)
    : IOnLoad
{
    private readonly TraderConfig _traderConfig = configServer.GetConfig<TraderConfig>();
    private readonly RagfairConfig _ragfairConfig = configServer.GetConfig<RagfairConfig>();


    public Task OnLoad()
    {
        // A path to the mods files we use below
        var pathToMod = modHelper.GetAbsolutePathToModFolder(Assembly.GetExecutingAssembly());

        // A relative path to the trader icon to show
        var traderImagePath = Path.Combine(pathToMod, "db/cat.jpg");

        // The base json containing trader settings we will add to the server
        var traderBase = modHelper.GetJsonDataFromFile<TraderBase>(pathToMod, "db/base.json");

        // Create a helper class and use it to register our traders image/icon + set its stock refresh time
        imageRouter.AddRoute(traderBase.Avatar.Replace(".jpg", ""), traderImagePath);
        addCustomTraderHelper.SetTraderUpdateTime(_traderConfig, traderBase, timeUtil.GetHoursAsSeconds(1), timeUtil.GetHoursAsSeconds(2));

        // Add our trader to the config file, this lets it be seen by the flea market
        _ragfairConfig.Traders.TryAdd(traderBase.Id, true);

        // Add our trader (with no items yet) to the server database
        // An 'assort' is the term used to describe the offers a trader sells, it has 3 parts to an assort
        // 1: The item
        // 2: The barter scheme, cost of the item (money or barter)
        // 3: The Loyalty level, what rep level is required to buy the item from trader
        addCustomTraderHelper.AddTraderWithEmptyAssortToDb(traderBase);

        // Add localisation text for our trader to the database so it shows to people playing in different languages
        addCustomTraderHelper.AddTraderToLocales(traderBase, "Cat", "This is the cat shop. Meow.");

        // Get the assort data from JSON
        var assort = modHelper.GetJsonDataFromFile<TraderAssort>(pathToMod, "db/assort.json");

        // Save the data we loaded above into the trader we've made
        addCustomTraderHelper.OverwriteTraderAssort(traderBase.Id, assort);

        // Send back a success to the server to say our trader is good to go
        return Task.CompletedTask;
    }
}

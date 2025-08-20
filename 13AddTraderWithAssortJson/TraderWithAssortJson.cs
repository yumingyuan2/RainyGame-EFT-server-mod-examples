using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Spt.Config;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Routers;
using SPTarkov.Server.Core.Servers;
using SPTarkov.Server.Core.Utils;
using System.Reflection;
using Path = System.IO.Path;

namespace _13AddTraderWithAssortJson
{
    public record ModMetadata : AbstractModMetadata
    {
        public override string ModGuid { get; init; } = "com.sp-tarkov.examples.addtraderjsonassorts";
        public override string Name { get; init; } = "AddTraderWithAssortJsonExample";
        public override string Author { get; init; } = "SPTarkov";
        public override List<string>? Contributors { get; set; } = ["Clodan", "CWX"];
        public override SemanticVersioning.Version Version { get; } = new("1.0.0");
        public override SemanticVersioning.Version SptVersion { get; } = new("4.0.0");
        public override List<string>? LoadBefore { get; set; }
        public override List<string>? LoadAfter { get; set; }
        public override List<string>? Incompatibilities { get; set; }
        public override Dictionary<string, SemanticVersioning.Version>? ModDependencies { get; set; }
        public override string? Url { get; set; } = "https://github.com/sp-tarkov/server-mod-examples";
        public override bool? IsBundleMod { get; set; } = false;
        public override string? License { get; init; } = "MIT";
    }

    [Injectable]
    public record TraderWithAssortJson : ICustomTrader
    {
        private readonly TraderAssort _traderAssort;
        private readonly TraderBase _traderBase;

        public TraderWithAssortJson(
            ISptLogger<TraderWithAssortJson> logger,
            ModHelper modHelper,
            ConfigServer configServer,
            ImageRouter imageRouter,
            TimeUtil timeUtil,
            AddCustomTraderHelper addCustomTraderHelper)
        {
            var traderConfig = configServer.GetConfig<TraderConfig>();
            var ragfairConfig = configServer.GetConfig<RagfairConfig>();

            // A path to the mods files we use below
            var pathToMod = modHelper.GetAbsolutePathToModFolder(Assembly.GetExecutingAssembly());

            // A relative path to the trader icon to show
            var traderImagePath = Path.Combine(pathToMod, "db/cat.jpg");

            // The base json containing trader settings we will add to the server
            _traderBase = modHelper.GetJsonDataFromFile<TraderBase>(pathToMod, "db/base.json");

            // Create a helper class and use it to register our traders image/icon + set its stock refresh time
            imageRouter.AddRoute(_traderBase.Avatar.Replace(".jpg", ""), traderImagePath);
            addCustomTraderHelper.SetTraderUpdateTime(traderConfig, _traderBase, timeUtil.GetHoursAsSeconds(1), timeUtil.GetHoursAsSeconds(2));

            // Add our trader to the config list, this lets it be seen by the flea market
            ragfairConfig.Traders.TryAdd(_traderBase.Id, true);

            // Add localisation text for our trader to the database so it shows to people playing in different languages
            addCustomTraderHelper.AddTraderToLocales(_traderBase, "Cat", "This is the cat shop. Meow.");

            // Get the assort data from JSON
            _traderAssort = modHelper.GetJsonDataFromFile<TraderAssort>(pathToMod, "db/assort.json");

        }

        public override TraderAssort? GetAssort()
        {
            return _traderAssort;
        }

        public override Dictionary<string, Dictionary<MongoId, MongoId>>? GetQuestAssort()
        {
            // Return an empty dictionary as this trader has no quest-unlocked assorts
            return new();
        }

        public override TraderBase? GetBase()
        {
            return _traderBase;
        }

        public override string Name => "Cat";
        public override MongoId Id { get; } = new("68a5e666ee7c07d084da744f");
    }
}

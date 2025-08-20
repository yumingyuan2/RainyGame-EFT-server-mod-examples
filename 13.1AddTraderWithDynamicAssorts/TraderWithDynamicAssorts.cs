using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Enums;
using SPTarkov.Server.Core.Models.Spt.Config;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Routers;
using SPTarkov.Server.Core.Servers;
using SPTarkov.Server.Core.Services;
using SPTarkov.Server.Core.Utils;
using System.Reflection;
using Path = System.IO.Path;

namespace _13._1AddTraderWithDynamicAssorts
{
    public record ModMetadata : AbstractModMetadata
    {
        public override string ModGuid { get; init; } = "com.sp-tarkov.examples.addtraderdynamicassorts";
        public override string Name { get; init; } = "AddTraderWithDynamicAssortsExample";
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
    public record TraderWithDynamicAssorts : ICustomTrader
    {
        private readonly TraderAssort _traderAssort;
        private readonly TraderBase _traderBase;
        private readonly FluentTraderAssortCreator _fluentAssortCreator;
        private readonly DatabaseService _databaseService;
        private readonly AddCustomTraderHelper _addCustomTraderHelper;

        public TraderWithDynamicAssorts(
            ISptLogger<TraderWithDynamicAssorts> logger,
            DatabaseService databaseService,
            ModHelper modHelper,
            ConfigServer configServer,
            ImageRouter imageRouter,
            TimeUtil timeUtil,
            FluentTraderAssortCreator fluentAssortCreator,
            AddCustomTraderHelper addCustomTraderHelper)
        {
            var _traderConfig = configServer.GetConfig<TraderConfig>();
            var _ragfairConfig = configServer.GetConfig<RagfairConfig>();

            _fluentAssortCreator = fluentAssortCreator;
            _addCustomTraderHelper = addCustomTraderHelper;
            _databaseService = databaseService;

            // A path to the mods files we use below
            var pathToMod = modHelper.GetAbsolutePathToModFolder(Assembly.GetExecutingAssembly());

            // A relative path to the trader icon to show
            var traderImagePath = Path.Combine(pathToMod, "db/cat.jpg");

            // The base json containing trader settings we will add to the server
            _traderBase = modHelper.GetJsonDataFromFile<TraderBase>(pathToMod, "db/base.json");

            // Create a helper class and use it to register our traders image/icon + set its stock refresh time
            imageRouter.AddRoute(_traderBase.Avatar.Replace(".jpg", ""), traderImagePath);
            addCustomTraderHelper.SetTraderUpdateTime(_traderConfig, _traderBase, timeUtil.GetHoursAsSeconds(1), timeUtil.GetHoursAsSeconds(2));

            // Add our trader to the config list, this lets it be seen by the flea market
            _ragfairConfig.Traders.TryAdd(_traderBase.Id, true);

            // Add localisation text for our trader to the database so it shows to people playing in different languages
            addCustomTraderHelper.AddTraderToLocales(_traderBase, "Cat", "This is the cat shop. Meow.");

            AddItemsToAssort();
        }

        /// <summary>
        /// Add various items as examples to our trader
        /// </summary>
        private void AddItemsToAssort()
        {
            // Add a single "milk" for 2000 roubles that player can buy a max of 10 per refresh
            _fluentAssortCreator
                .CreateSingleAssortItem(ItemTpl.DRINK_PACK_OF_MILK)
                .AddStackCount(200)
                .AddBuyRestriction(10)
                .AddMoneyCost(Money.ROUBLES, 2000)
                .AddLoyaltyLevel(1)
                .Export(_traderAssort);

            // Add a 3x bitcoin + salewa for milk barter
            _fluentAssortCreator
                .CreateSingleAssortItem(ItemTpl.DRINK_PACK_OF_MILK)
                .AddStackCount(100)
                .AddBarterCost(ItemTpl.BARTER_PHYSICAL_BITCOIN, 3)
                .AddBarterCost(ItemTpl.MEDKIT_SALEWA_FIRST_AID_KIT, 1)
                .AddLoyaltyLevel(1)
                .Export(_traderAssort);


            // Add glock as a rouble purchase
            _fluentAssortCreator
                .CreateComplexAssortItem(_addCustomTraderHelper.CreateGlock())
                .AddUnlimitedStackCount()
                .AddMoneyCost(Money.ROUBLES, 20000)
                .AddBuyRestriction(3)
                .AddLoyaltyLevel(1)
                .Export(_traderAssort);

            // Add mp133 preset as a barter for mayonnaise
            // We give it the id of the mp133 weapon preset found in globals.json
            // Most weapons have a 'default' in `Globals.ItemPresets`
            var mp133PresetId = new MongoId("584148f2245977598f1ad387");
            _fluentAssortCreator
                .CreateComplexAssortItem(_databaseService.GetTables().Globals.ItemPresets.GetValueOrDefault(mp133PresetId).Items)
                .AddStackCount(200)
                .AddBarterCost(ItemTpl.FOOD_JAR_OF_DEVILDOG_MAYO, 1)
                .AddBuyRestriction(3)
                .AddLoyaltyLevel(1)
                .Export(_traderAssort);
        }

        public override TraderAssort? GetAssort()
        {
            return _traderAssort;
        }

        public override Dictionary<string, Dictionary<MongoId, MongoId>>? GetQuestAssort()
        {
            return new();
        }

        public override TraderBase? GetBase()
        {
            return _traderBase;
        }

        public override string Name { get; } = "Cat";
        public override MongoId Id { get; } = new MongoId("68a5e1a1bea774b77e7dde63");
    }
}

using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Spt.Config;
using SPTarkov.Server.Core.Models.Spt.Server;
using SPTarkov.Server.Core.Services;
using SPTarkov.Server.Core.Utils.Cloners;

namespace _13._1AddTraderWithDynamicAssorts
{
    public class AddTraderHelper
    {
        private readonly LocaleService _localeService;

        public AddTraderHelper(LocaleService localeService)
        {
            _localeService = localeService;
        }

        /**
     * Add record to trader config to set the refresh time of trader in seconds (default is 60 minutes)
     * @param traderConfig trader config to add our trader to
     * @param baseJson json file for trader (db/base.json)
     * @param refreshTimeSecondsMin How many seconds between trader stock refresh min time
     * @param refreshTimeSecondsMax How many seconds between trader stock refresh max time
     */
    public void SetTraderUpdateTime(TraderConfig traderConfig, TraderBase baseJson, int refreshTimeSecondsMin, int refreshTimeSecondsMax)
    {
        // Add refresh time in seconds to config
        var traderRefreshRecord = new UpdateTime
        {
            TraderId = baseJson.Id,
            Seconds = new MinMax<int>(refreshTimeSecondsMin, refreshTimeSecondsMax)
        };
    

        traderConfig.UpdateTime.Add(traderRefreshRecord);
    }

    /**
     * Add our new trader to the database
     * @param traderDetailsToAdd trader details
     * @param tables database
     * @param jsonUtil json utility class
     */
    public void AddTraderToDb(TraderBase traderDetailsToAdd, DatabaseTables tables, ICloner cloner, TraderAssort assort)
    {
        // Create trader data ready to add to database
        var traderDataToAdd = new Trader
        {
            Assort = cloner.Clone(assort),
            Base = cloner.Clone(traderDetailsToAdd),
            QuestAssort = new Dictionary<string, Dictionary<string, string>> // quest assort is empty as trader has no assorts unlocked by quests
            {
                { "Started", new Dictionary<string, string>() },
                { "Success", new Dictionary<string, string>() },
                { "Fail", new Dictionary<string, string>() }
            }
        };

        // Add trader to trader table, key is the traders id
        if (!tables.Traders.TryAdd(traderDetailsToAdd.Id, traderDataToAdd))
        {
            //Failed to add trader!
        }
    }

        /// <summary>
        /// Add traders name/location/description to all locales (e.g. German/French/English)
        /// </summary>
        /// <param name="baseJson">json file for trader (db/base.json)</param>
        /// <param name="tables">Database tables</param>
        /// <param name="fullName">Complete name of trader</param>
        /// <param name="firstName">First name of trader</param>
        /// <param name="nickName">Nickname of trader</param>
        /// <param name="location">Flavor text of the location of trader (e.g. "Here in the cat shop")</param>
        /// <param name="description">Flavor text of whom the trader is</param>
        public void AddTraderToLocales(TraderBase baseJson, DatabaseTables tables, string fullName, string firstName, string nickName, string location, string description)
    {
        // For each language, add locale for the new trader
        var locales = tables.Locales.Global;
        var newTraderId = baseJson.Id;

        foreach (var (localeKey, localeKvP) in locales)
        {
            _localeService.AddCustomClientLocale(localeKey, $"{newTraderId} FullName", fullName);
            _localeService.AddCustomClientLocale(localeKey, $"{newTraderId} FirstName", firstName);
            _localeService.AddCustomClientLocale(localeKey, $"{newTraderId} Nickname", nickName);
            _localeService.AddCustomClientLocale(localeKey, $"{newTraderId} Location", location);
            _localeService.AddCustomClientLocale(localeKey, $"{newTraderId} Description", description);
        }
    }

    public List<Item> CreateGlock()
    {
            // Create an array ready to hold the glock and all its mods
            var glock = new List<Item>();

            // Add the base (root) first
            glock.Add(new Item { // Add the base weapon first
            Id =
                NewItemIds.GLOCK_BASE, // Ids matter, Ids MUST be unique for every item
            Template =
                "5a7ae0c351dfba0017554310", // This is the weapons tpl, found on: https://db.sp-tarkov.com/search
        });

            // Add barrel
            glock.Add(new Item {
            Id =
                NewItemIds.GLOCK_BARREL,
            Template =
                "5a6b60158dc32e000a31138b",
            ParentId =
                NewItemIds.GLOCK_BASE, // This is a sub item, you need to define its parent its attached to / inserted into
            SlotId =
                "mod_barrel", // Required for mods, you need to define what 'slot' the mod will fill on the weapon
        });

            // Add receiver
            glock.Add( new Item {
            Id =
                NewItemIds.GLOCK_RECIEVER,
            Template =
                "5a9685b1a2750c0032157104",
            ParentId =
                NewItemIds.GLOCK_BASE,
            SlotId =
                "mod_reciever",
        });

            // Add compensator
            glock.Add(new Item {
            Id =
                NewItemIds.GLOCK_COMPENSATOR,
            Template =
                "5a7b32a2e899ef00135e345a",
            ParentId =
                NewItemIds.GLOCK_RECIEVER, // The parent of this mod is the receiver NOT weapon, be careful to get the correct parent
            SlotId =
                "mod_muzzle",
        });

            // Add Pistol grip
            glock.Add(new Item {
            Id =
                NewItemIds.GLOCK_PISTOL_GRIP,
            Template =
                "5a7b4960e899ef197b331a2d",
            ParentId =
                NewItemIds.GLOCK_BASE,
            SlotId =
                "mod_pistol_grip",
        });

            // Add front sight
            glock.Add(new Item {
            Id =
                NewItemIds.GLOCK_FRONT_SIGHT,
            Template =
                "5a6f5d528dc32e00094b97d9",
            ParentId =
                NewItemIds.GLOCK_RECIEVER,
            SlotId =
                "mod_sight_rear",
        });

            // Add rear sight
            glock.Add(new Item {
            Id =
                NewItemIds.GLOCK_REAR_SIGHT,
            Template =
                "5a6f58f68dc32e000a311390",
            ParentId =
                NewItemIds.GLOCK_RECIEVER,
            SlotId =
                "mod_sight_front",
        });

            // Add magazine
            glock.Add(new Item {
            Id =
                NewItemIds.GLOCK_MAGAZINE,
            Template =
                "630769c4962d0247b029dc60",
            ParentId =
                NewItemIds.GLOCK_BASE,
            SlotId =
                "mod_magazine",
        });

            return glock;
        }
    }
}

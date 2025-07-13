using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Enums;
using SPTarkov.Server.Core.Models.Spt.Config;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Servers;

namespace _3EditSptConfig;

/// <summary>
/// This is the replacement for the former package.json data. This is required for all mods.
///
/// This is where we define all the metadata associated with this mod.
/// You don't have to do anything with it, other than fill it out.
/// All properties must be overriden, properties you don't use may be left null.
/// It is read by the mod loader when this mod is loaded.
/// </summary>
public record ModMetadata : AbstractModMetadata
{
    public override string ModGuid { get; set; } = "com.sp-tarkov.examples.editsptconfig";
    public override string Name { get; set; } = "EditConfigsExample";
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

// We want to load after PostDBModLoader is complete, so we set our type priority to that, plus 1.
[Injectable(TypePriority = OnLoadOrder.PostDBModLoader + 1)]
public class EditConfigs(
    ConfigServer configServer,
    ISptLogger<EditConfigs> logger
) : IOnLoad // Implement the IOnLoad interface so that this mod can do something
{
    // We get a config by calling GetConfig<>() and passing the configs 'type' inside the diamond <> brackets
    // These fields start with a _, this is a good convention when you are making private fields in your code
    // They're also readonly as you shouldn't be overwriting configs, only editing values inside them
    private readonly BotConfig _botConfig = configServer.GetConfig<BotConfig>();
    private readonly HideoutConfig _hideoutConfig = configServer.GetConfig<HideoutConfig>();
    private readonly WeatherConfig _weatherConfig = configServer.GetConfig<WeatherConfig>();
    private readonly AirdropConfig _airdropConfig = configServer.GetConfig<AirdropConfig>();
    private readonly PmcChatResponse _pmcChatResponseConfig = configServer.GetConfig<PmcChatResponse>();
    private readonly QuestConfig _questConfig = configServer.GetConfig<QuestConfig>();
    private readonly PmcConfig _pmcConfig = configServer.GetConfig<PmcConfig>();

    /// <summary>
    /// This is called when this class is loaded, the order in which its loaded is set according to the type priority
    /// on the [Injectable] attribute on this class. Each class can then be used as an entry point to do
    /// things at varying times according to type priority
    /// </summary>
    public Task OnLoad()
    {
        // Let's edit the weather config to force the season to winter
        _weatherConfig.OverrideSeason = Season.WINTER;

        // Let's edit the hideout config to Make all crafts take 60 seconds
        _hideoutConfig.OverrideCraftTimeSeconds = 60;

        // Let's edit the hideout config to Make all upgrades take 60 seconds
        _hideoutConfig.OverrideBuildTimeSeconds = 60;

        // Let's edit the airdrop config to Make weapon/armor drops REALLY common
        _airdropConfig.AirdropTypeWeightings[SptAirdropTypeEnum.weaponArmor] = 999;

        // Let's edit the airdrop config to Make weapon/armor drops always have 3 sealed weapon crates
        // When accessing a dictionary, 'TryGetValue' is a safe way to do it, it will return true if it finds the key you want, or false if it doesn't
        // The second parameter 'weaponAndArmorLootSettingsAirdropLoot' is an 'out' parameter, it will be hydrated with the data we want if it's found
        // The examples below that access dictionaries will be the 'unsafe/old' way using square [] brackets. Both approaches will work, you should consider both and consider which suits your needs for your mod
        if (_airdropConfig.Loot.TryGetValue("weaponArmor", out var weaponAndArmorLootSettingsAirdropLoot))
        {
            // We found what we wanted in the dictionary, lets make changes
            // Weapon/armor crates will always have 3 sealed weapon crates inside them
            weaponAndArmorLootSettingsAirdropLoot.WeaponCrateCount.Min = 3;
            weaponAndArmorLootSettingsAirdropLoot.WeaponCrateCount.Max = 3;
        }

        // Let's make PMCs always mail you when they kill you
        _pmcChatResponseConfig.Killer.ResponseChancePercent = 100;

        // Let's make quest rewards sent to you via mail last for over a week for unheard profiles
        _questConfig.MailRedeemTimeHours["unheard_edition"] = 168;

        // Let's make the interchange bot cap huge
        _botConfig.MaxBotCap["interchange"] = 50;

        // Let's disable loot on scavs
        _botConfig.DisableLootOnBotTypes.Add("assault");

        // Lets make PMCs carry absurdly expensive loot in their pockets
        _pmcConfig.MaxPocketLootTotalRub = 99999999;

        logger.Success("Finished Editing Configs");

        // Return a completed task
        return Task.CompletedTask;
    }
}

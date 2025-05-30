using SPTarkov.Server.Core.Models.Enums;
using SPTarkov.Server.Core.Models.Spt.Config;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Servers;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Spt.Mod;

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
public class EditConfigs : IOnLoad // Implement the IOnLoad interface so that this mod can do something
{
    private readonly AirdropConfig _airdropConfig;
    private readonly BotConfig _botConfig;
    private readonly ConfigServer _configServer;
    private readonly HideoutConfig _hideoutConfig;

    private readonly ISptLogger<EditConfigs> _logger;
    private readonly PmcChatResponse _pmcChatResponseConfig;
    private readonly PmcConfig _pmcConfig;
    private readonly QuestConfig _questConfig;
    private readonly WeatherConfig _weatherConfig;

    // We access configs via ConfigServer
    public EditConfigs(
        ConfigServer configServer,
        ISptLogger<EditConfigs> logger
    )
    {
        _configServer = configServer;
        _logger = logger;

        // We get the bot config by calling GetConfig and passing the configs 'type' within the diamond brackets
        _botConfig = _configServer.GetConfig<BotConfig>();
        _hideoutConfig = _configServer.GetConfig<HideoutConfig>();
        _weatherConfig = _configServer.GetConfig<WeatherConfig>();
        _airdropConfig = _configServer.GetConfig<AirdropConfig>();
        _pmcChatResponseConfig = _configServer.GetConfig<PmcChatResponse>();
        _questConfig = _configServer.GetConfig<QuestConfig>();
        _pmcConfig = _configServer.GetConfig<PmcConfig>();
    }
    
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
        var weaponCrateMinMax = _airdropConfig.Loot["weaponArmor"].WeaponCrateCount;
        weaponCrateMinMax.Min = 3;
        weaponCrateMinMax.Max = 3;

        // Let's make PMCs always mail you when they kill you
        _pmcChatResponseConfig.Killer.ResponseChancePercent = 100;

        // Let's make quest rewards sent to you via mail last for over a week for unheard profiles
        _questConfig.MailRedeemTimeHours["unheard_edition"] = 168;

        // Let's make the interchange bot cap huge
        _botConfig.MaxBotCap["interchange"] = 50;

        // Let's disable loot on scavs
        _botConfig.DisableLootOnBotTypes.Add("assault");

        _logger.Success("Finished Editing Configs");
        
        // Return a completed task
        return Task.CompletedTask;
    }
}

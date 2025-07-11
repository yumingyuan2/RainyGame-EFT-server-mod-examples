using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Servers;
using SPTarkov.Server.Core.Services;
using SPTarkov.Server.Core.Utils;

namespace _6OverrideMethod;

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
    public override string ModId { get; set; } = "overridemethod.6870c483bad1c6d60765702b";
    public override string Name { get; set; } = "OverrideMethodExample";
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

[Injectable(TypePriority = OnLoadOrder.Watermark)] // The same load order value needs to be used as the overridden methods containing type
public class OverrideMethod(
    ISptLogger<Watermark> logger, // The logger needs to use the same type as the overridden type (in this case, Watermark)
    ConfigServer configServer,
    ServerLocalisationService localisationService,
    WatermarkLocale watermarkLocale)
    : Watermark(logger, configServer, localisationService, watermarkLocale) // You must provide the parameters the overridden type requires
{

    public override async Task OnLoad()
    {
        // We add a log message to the init method
        _logger.Success("This is a watermark mod override!");
    
        // perform any asynchronous operations here, using await
    
        // This runs the original method (optional)
        await base.OnLoad();
    }
}

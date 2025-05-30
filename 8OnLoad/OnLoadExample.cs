using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Spt.Mod;

namespace _8OnLoad;

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
    public override string Name { get; set; } = "OnLoadExampleExample";
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

// Flag class as being OnLoad and give it a load priority, check `OnLoadOrder` for list of possible choices
[Injectable(InjectableTypeOverride = typeof(IOnLoad), TypePriority = OnLoadOrder.PostSptDatabase)] // Can also give an int value for fine-grained control
[Injectable(InjectableTypeOverride = typeof(OnLoadExample))]
public class OnLoadExample : IOnLoad // Must implement the IOnLoad interface
{
    private readonly ISptLogger<OnLoadExample> _logger;

    public OnLoadExample(
        ISptLogger<OnLoadExample> logger)
    {
        _logger = logger;
    }

    public Task OnLoad()
    {
        // Can do work here
        _logger.Success($"Mod loaded after database!");

        return Task.CompletedTask;
    }

    public string GetRoute()
    {
        return "mod-load-example";
    }
}

using SPTarkov.Server.Core.Helpers;
using System.Reflection;
using SPTarkov.Server.Core.Models.Utils;
using fastJSON5;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Spt.Mod;

namespace _4ReadCustomJson5Config;

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
    public override string Name { get; set; } = "ReadJson5ConfigExample";
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

// We want to load after PreSptModLoader is complete, so we set our type priority to that, plus 1.
[Injectable(TypePriority = OnLoadOrder.PreSptModLoader + 1)]
public class ReadJson5Config : IOnLoad // Implement the IOnLoad interface so that this mod can do something
{
    private readonly ISptLogger<ReadJson5Config> _logger;
    private readonly ModHelper _modHelper;

    public ReadJson5Config(
        ISptLogger<ReadJson5Config> logger,
        ModHelper modHelper)
    {
        _logger = logger;
        _modHelper = modHelper;
    }
    
    public Task OnLoad()
    {
        var pathToMod = _modHelper.GetAbsolutePathToModFolder(Assembly.GetExecutingAssembly());

        // To use JSON5, you will have to find and provide your own JSON5 library to decode it
        var json5Config = JSON5.ToObject<ModConfig>(_modHelper.GetRawFileData(pathToMod, "config.json5"));

        _logger.Success($"Read property: 'ExampleProperty' from config with value: {json5Config.ExampleProperty}");
        
        // Return a completed task
        return Task.CompletedTask;
    }
}

// This class should represent your config structure
public class ModConfig
{
    public string ExampleProperty { get; set; }
}

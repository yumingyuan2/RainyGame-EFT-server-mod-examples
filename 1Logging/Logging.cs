using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Logging;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Models.Utils;

namespace _1Logging;

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
    public override string Name { get; set; } = "LoggingExample";
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
public class Logging : IOnLoad // Implement the IOnLoad interface so that this mod can do something
{
    // Our logger we create in the constructor below
    private readonly ISptLogger<Logging> _logger;

    // Constructor - Inject a 'ISptLogger' with your mods Class inside the diamond brackets
    public Logging(
        ISptLogger<Logging> logger
    )
    {
        // Save the logger we're injecting into a private variable that is scoped to this class (only this class has access to it)
        _logger = logger;
    }

    public Task OnLoad()
    {
        // We can access the logger to assigned in the constructor here
        _logger.Success("This is a success message");
        _logger.Warning("This is a warning message");
        _logger.Error("This is an error message");
        _logger.Info("This is an info message");
        _logger.Critical("this is a critical message");

        // Logging with colors requires you to 'pass' the text color and background color
        _logger.LogWithColor("This is a message with custom colors", LogTextColor.Red, LogBackgroundColor.Black);
        _logger.Debug("This is a debug message that gets written to the log file, not the console");
        
        // Return a completed task so that we know we are done
        return Task.CompletedTask;
    }
}

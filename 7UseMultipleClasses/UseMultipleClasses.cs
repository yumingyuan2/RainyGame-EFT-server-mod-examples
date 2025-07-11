using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Models.Utils;

namespace _7UseMultipleClasses;

public record ModMetadata : AbstractModMetadata
{
    public override string ModId { get; set; } = "multipleclasses.6870c4a9c0d7e7b76f5aeeeb";
    public override string Name { get; set; } = "UseMultipleClassesExample";
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

/// <summary>
/// Having multiple classes can make keeping your code maintainable easier, you can split related code into their own class and inject them
/// </summary>

// We want to load after PostDBModLoader is complete, so we set our type priority to that, plus 1.
[Injectable(TypePriority = OnLoadOrder.PostDBModLoader + 1)]
public class UseMultipleClasses(
    ISptLogger<UseMultipleClasses> logger,
    SecondClass secondClass // We inject our second class just like other classes
    ) : IOnLoad
{
    public Task OnLoad()
    {
        // We call the "GetText" method that exists in the other class
        var text = secondClass.GetText();

        // Log the result to the server console
        logger.Info($"The SecondClass returned the text: {text}");
        
        // Tell server we've finished
        return Task.CompletedTask;
    }
}
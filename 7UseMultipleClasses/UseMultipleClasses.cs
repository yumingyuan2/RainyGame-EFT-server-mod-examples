using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Models.Utils;

namespace _7UseMultipleClasses;

public record ModMetadata : AbstractModMetadata
{
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
/// Having multiple classes can make keeping your code maintainable easier, you can split related code into their own class
/// </summary>

// We want to load after PostDBModLoader is complete, so we set our type priority to that, plus 1.
[Injectable(TypePriority = OnLoadOrder.PostDBModLoader + 1)]
public class UseMultipleClasses(ISptLogger<UseMultipleClasses> logger) : IOnLoad
{
    public Task OnLoad()
    {
        // We create an instance of the other class
        var otherClass = new SecondClass();

        // We call the "GetText" method that exists in the other class
        var text = otherClass.GetText();

        // Log the result to the server console
        logger.Info($"The SecondClass returned the text: {text}");
        
        // Return a completed task
        return Task.CompletedTask;
    }
}
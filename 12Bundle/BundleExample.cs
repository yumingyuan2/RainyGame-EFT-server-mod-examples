using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Models.Utils;

namespace _12Bundle
{
    public record ModMetadata : AbstractModMetadata
    {
        public override string Name { get; init; } = "BundleExample";
        public override string Author { get; init; } = "SPTarkov";
        public override List<string>? Contributors { get; set; }
        public override SemanticVersioning.Version Version { get; } = new("1.0.0");
        public override SemanticVersioning.Version SptVersion { get; } = new("4.0.0");
        public override List<string>? LoadBefore { get; set; }
        public override List<string>? LoadAfter { get; set; }
        public override List<string>? Incompatibilities { get; set; }
        public override Dictionary<string, SemanticVersioning.Version>? ModDependencies { get; set; }
        public override string? Url { get; set; }
        public override bool? IsBundleMod { get; set; } = true;
        public override string? License { get; init; } = "MIT";
        public override string ModGuid { get; init; } = "com.sp-tarkov.examples.bundleexample";
    }

    [Injectable(TypePriority = OnLoadOrder.PostSptModLoader)]
    public class BundleExample(ISptLogger<BundleExample> logger) : IOnLoad
    {
        public Task OnLoad()
        {
            logger.Success("Bundle example loaded!");
            return Task.CompletedTask;
        }
    }
}

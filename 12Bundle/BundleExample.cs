using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Models.Utils;

namespace _12Bundle
{
    public record ModMetadata : AbstractModMetadata
    {
        public override string Name { get; set; } = "BundleExample";
        public override string Author { get; set; } = "SPTarkov";
        public override List<string>? Contributors { get; set; }
        public override string Version { get; set; } = "1.0.0";
        public override string SptVersion { get; set; } = "4.0.0";
        public override List<string>? LoadBefore { get; set; }
        public override List<string>? LoadAfter { get; set; }
        public override List<string>? Incompatibilities { get; set; }
        public override Dictionary<string, string>? ModDependencies { get; set; }
        public override string? Url { get; set; }
        public override bool? IsBundleMod { get; set; } = true;
        public override string? Licence { get; set; } = "MIT";
        public override string ModGuid { get; set; } = "com.sp-tarkov.examples.bundleexample";
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

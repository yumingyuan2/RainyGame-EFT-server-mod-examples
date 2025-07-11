using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Utils;

namespace _10CustomRoute;

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
    public override string ModId { get; set; } = "com.sp-tarkov.examples.customroute";
    public override string Name { get; set; } = "CustomStaticRouterExample";
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

[Injectable]
public class CustomStaticRouter : StaticRouter
{
    private static HttpResponseUtil _httpResponseUtil;

    public CustomStaticRouter(
        JsonUtil jsonUtil,
        HttpResponseUtil httpResponseUtil) : base(
        jsonUtil,
        // Add an array of routes we want to add
        GetCustomRoutes()
    )
    {
        _httpResponseUtil = httpResponseUtil;
    }

    private static List<RouteAction> GetCustomRoutes()
    {
        return
        [
            new RouteAction(
                "/example/route/static",
                async (
                    url,
                    info,
                    sessionId,
                    output
                ) => await HandleRoute(url, info as ExampleStaticRequestData, sessionId)
            )
        ];
    }

    private static ValueTask<string> HandleRoute(string url, ExampleStaticRequestData info, MongoId sessionId)
    {
        // Your mods code goes here

        return new ValueTask<string>(_httpResponseUtil.NullResponse());
    }
}
public class ExampleStaticRequestData : IRequestData
{
}
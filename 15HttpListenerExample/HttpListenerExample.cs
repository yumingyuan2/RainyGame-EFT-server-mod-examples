using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Servers.Http;

namespace _15HttpListenerExample;

public record ModMetadata : AbstractModMetadata
{
    public override string ModGuid { get; set; } = "com.sp-tarkov.examples.httplistener";
    public override string Name { get; set; } = "HttpListenerExample";
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
public class HttpListenerExample : IHttpListener
{
    public bool CanHandle(string sessionId, HttpRequest req)
    {
        return req.Method == "GET" && req.Method.Contains("/type-custom-url");
    }

    public async Task Handle(string sessionId, HttpRequest req, HttpResponse resp)
    {
        resp.StatusCode = 200;
        await resp.Body.WriteAsync("[1] This is the first example of a mod hooking into the HttpServer"u8.ToArray());
        await resp.StartAsync();
        await resp.CompleteAsync();
    }
}

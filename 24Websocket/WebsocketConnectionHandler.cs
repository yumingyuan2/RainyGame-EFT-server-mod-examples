using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Servers.Ws;
using System.Net.WebSockets;
using System.Text;

namespace _24Websocket;

public record ModMetadata : AbstractModMetadata
{
    public override string ModId { get; set; } = "websocket.6870fccfb9b3615d166ebbb8";
    public override string Name { get; set; } = "CustomWebSocketConnectionHandlerExample";
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

[Injectable(InjectionType = InjectionType.Singleton)]
public class CustomWebSocketConnectionHandler(
    ISptLogger<CustomWebSocketConnectionHandler> logger) : IWebSocketConnectionHandler
{
    public string GetHookUrl()
    {
        return "/custom/socket/";
    }

    public string GetSocketId()
    {
        return "My Custom WebSocket";
    }

    public Task OnConnection(WebSocket ws, HttpContext context, string sessionIdContext)
    {
        logger.Info("Custom web socket is now connected!");
        
        return Task.CompletedTask;
    }

    public async Task OnMessage(byte[] rawData, WebSocketMessageType messageType, WebSocket ws, HttpContext context)
    {
        var msg = Encoding.UTF8.GetString(rawData);

        if (msg == "toodaloo")
        {
            await ws.SendAsync(Encoding.UTF8.GetBytes("toodaloo back!"), WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }

    public Task OnClose(WebSocket ws, HttpContext context, string sessionIdContext)
    {
        return Task.CompletedTask;
    }
}

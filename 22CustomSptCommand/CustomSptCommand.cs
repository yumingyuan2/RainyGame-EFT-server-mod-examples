using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Helpers.Dialog.Commando.SptCommands;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Dialog;
using SPTarkov.Server.Core.Models.Eft.Profile;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Services;

namespace _22CustomSptCommand;

public record ModMetadata : AbstractModMetadata
{
    public override string ModGuid { get; init; } = "com.sp-tarkov.examples.customsptcommand";
    public override string Name { get; init; } = "CustomCommandoCommandExample";
    public override string Author { get; init; } = "SPTarkov";
    public override List<string>? Contributors { get; set; }
    public override SemanticVersioning.Version Version { get; } = new("1.0.0");
    public override SemanticVersioning.Version SptVersion { get; } = new("4.0.0");
    public override List<string>? LoadBefore { get; set; }
    public override List<string>? LoadAfter { get; set; }
    public override List<string>? Incompatibilities { get; set; }
    public override Dictionary<string, SemanticVersioning.Version>? ModDependencies { get; set; }
    public override string? Url { get; set; }
    public override bool? IsBundleMod { get; set; }
    public override string? License { get; init; } = "MIT";
}

[Injectable]
public class CustomSptCommand(
    MailSendService mailSendService,
    ItemHelper itemHelper) : ISptCommand
{
    public ValueTask<string> PerformAction(UserDialogInfo commandHandler, MongoId sessionId, SendMessageRequest request)
    {
        var splitCommand  = request.Text.Split(" ");
        mailSendService.SendUserMessageToPlayer(sessionId, commandHandler, $"That templateId belongs to item {itemHelper.GetItem(splitCommand[2]).Value?.Properties?.Name ?? ""}");
        
        return ValueTask.FromResult(request.DialogId);
    }

    public string Command => "getName";

    public string CommandHelp => "Usage: spt getName tplId";
}

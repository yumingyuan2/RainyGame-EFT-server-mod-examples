using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Helpers.Dialog.Commando.SptCommands;
using SPTarkov.Server.Core.Models.Eft.Dialog;
using SPTarkov.Server.Core.Models.Eft.Profile;
using SPTarkov.Server.Core.Services;
using SPTarkov.Server.Core.Models.Spt.Mod;

namespace _22CustomSptCommand;

public record ModMetadata : AbstractModMetadata
{
    public override string Name { get; set; } = "CustomCommandoCommandExample";
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
public class CustomSptCommand : ISptCommand
{
    private readonly MailSendService _mailSendService;
    private readonly ItemHelper _itemHelper;

    public CustomSptCommand(
        MailSendService mailSendService,
        ItemHelper itemHelper
    )
    {
        _mailSendService = mailSendService;
        _itemHelper = itemHelper;
    }

    public string GetCommand()
    {
        return "getName";
    }

    public string GetCommandHelp()
    {
        return "Usage: spt getName tplId";
    }

    public string PerformAction(UserDialogInfo commandHandler, string sessionId, SendMessageRequest request)
    {
        var splitCommand  = request.Text.Split(" ");
        _mailSendService.SendUserMessageToPlayer(sessionId, commandHandler, $"That templateId belongs to item {_itemHelper.GetItem(splitCommand[2]).Value?.Properties?.Name ?? ""}");
        return request.DialogId;
    }
}

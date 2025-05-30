using SPTarkov.Server.Core.Helpers.Dialog.Commando;
using SPTarkov.Server.Core.Models.Eft.Dialog;
using SPTarkov.Server.Core.Models.Eft.Profile;
using SPTarkov.Server.Core.Servers;
using SPTarkov.Server.Core.Services;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Spt.Mod;

namespace _21CustomCommandoCommand;

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
public class CustomCommandoCommand : IChatCommand
{
    private DatabaseServer _databaseServer;
    private MailSendService _mailSendService;

    public CustomCommandoCommand(
        DatabaseServer databaseServer,
        MailSendService mailSendService
    )
    {
        _databaseServer = databaseServer;
        _mailSendService = mailSendService;
    }

    public string GetCommandPrefix()
    {
        return "test";
    }

    public string? GetCommandHelp(string command)
    {
        if (command == "talk")
        {
            return "Usage: test talk";
        }

        return null;
    }

    public List<string> GetCommands()
    {
        return ["talk"];
    }

    // spelling of sessionId is fixed in server
    public string? Handle(string command, UserDialogInfo commandHandler, string sessionId, SendMessageRequest request)
    {
        if (command == "talk")
        {
            _mailSendService.SendUserMessageToPlayer(sessionId, commandHandler, $"IM TALKING! OKAY?!\nHere's the walk speed X config from the DB: {_databaseServer.GetTables().Globals.Configuration.WalkSpeed.X}");
            return request.DialogId;
        }

        return null;
    }
}

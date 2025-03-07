using SPTarkov.Server.Core.Helpers.Dialog.Commando;
using SPTarkov.Server.Core.Models.Eft.Dialog;
using SPTarkov.Server.Core.Models.Eft.Profile;
using SPTarkov.Server.Core.Servers;
using SPTarkov.Server.Core.Services;
using SPTarkov.Common.Annotations;

namespace _21CustomCommandoCommand;

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

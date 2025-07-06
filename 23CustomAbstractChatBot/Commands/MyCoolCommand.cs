using SPTarkov.Server.Core.Helpers.Dialog.Commando;
using SPTarkov.Server.Core.Models.Eft.Dialog;
using SPTarkov.Server.Core.Models.Eft.Profile;
using SPTarkov.Server.Core.Services;

namespace _23CustomAbstractChatBot.Commands;

public class MyCoolCommand : IChatCommand
{
    private readonly MailSendService _mailSendService;

    public MyCoolCommand(
        MailSendService mailSendService
    )
    {
        _mailSendService = mailSendService;
    }

    public string GetCommandPrefix()
    {
        return "example";
    }

    public string GetCommandHelp(string command)
    {
        if (command == "test")
        {
            return "Usage: example test";
        }

        return null;
    }

    public List<string> GetCommands()
    {
        return ["test"];
    }

    public ValueTask<string> Handle(string command, UserDialogInfo commandHandler, string sessionId, SendMessageRequest request)
    {
        if (command == "test")
        {
            _mailSendService.SendUserMessageToPlayer(sessionId, commandHandler, $"This is a test message shown as an example!");
            return ValueTask.FromResult(request.DialogId);
        }

        return new ValueTask<string>(string.Empty);
    }
}

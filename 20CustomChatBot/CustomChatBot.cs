using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Helpers.Dialogue;
using SPTarkov.Server.Core.Models.Eft.Dialog;
using SPTarkov.Server.Core.Models.Eft.Profile;
using SPTarkov.Server.Core.Models.Enums;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Services;

namespace _20CustomChatBot;

public record ModMetadata : AbstractModMetadata
{
    public override string Name { get; set; } = "CustomChatBotExample";
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
public class CustomChatBot(
    MailSendService mailSendService) : IDialogueChatBot
{
    public UserDialogInfo GetChatBot()
    {
        return new UserDialogInfo
        {
            Id = "modderBuddy",
            Aid = 9999999,
            Info = new UserDialogDetails
            {
                Nickname = "Buddy",
                Side = "Usec",
                Level = 69,
                MemberCategory = MemberCategory.Sherpa,
                SelectedMemberCategory = MemberCategory.Sherpa
            }
        };
    }

    public string? HandleMessage(string sessionId, SendMessageRequest request)
    {
        mailSendService.SendUserMessageToPlayer(
            sessionId,
            GetChatBot(),
            $"Im Buddy! I just reply back what you typed to me!\n{request.Text}");

        return request.DialogId;
    }
}

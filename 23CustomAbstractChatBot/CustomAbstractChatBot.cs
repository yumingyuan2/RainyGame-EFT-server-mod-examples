using SPTarkov.Server.Core.Helpers.Dialog.Commando;
using SPTarkov.Server.Core.Helpers.Dialogue;
using SPTarkov.Server.Core.Helpers.Dialogue.SPTFriend.Commands;
using SPTarkov.Server.Core.Models.Eft.Profile;
using SPTarkov.Server.Core.Models.Enums;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Services;

namespace _23CustomAbstractChatBot;

public record ModMetadata : AbstractModMetadata
{
    public override string ModGuid { get; set; } = "com.sp-tarkov.examples.customabstractchatbot";
    public override string Name { get; set; } = "CustomAbstractChatBotExample";
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

public class CustomAbstractChatBot : AbstractDialogChatBot
{
    public CustomAbstractChatBot(
        ISptLogger<AbstractDialogChatBot> logger,
        MailSendService mailSendService,
        ServerLocalisationService localisationService,
        IEnumerable<IChatCommand> chatCommands,
        IEnumerable<IChatMessageHandler> chatMessageHandlers
    ) : base(logger, mailSendService, localisationService, chatCommands)
    {
    }

    public override UserDialogInfo GetChatBot()
    {
        return new UserDialogInfo
        {
            Id = "674db14ed849a3727ef24da0", // REQUIRES a valid mongoid, use online generator to create one
            Aid = 1234566,
            Info = new UserDialogDetails
            {
                Level = 69,
                MemberCategory = MemberCategory.Developer,
                SelectedMemberCategory = MemberCategory.Developer,
                Nickname = "CoolAbstractChatBot",
                Side = "Bear"
            }
        };
    }

    protected override string GetUnrecognizedCommandMessage()
    {
        return "No clue what you are talking about bud!";
    }
}

using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;

namespace LloydBot.Commands.Moderation;

[RequirePermissions(DSharpPlus.Entities.DiscordPermission.ModerateMembers)]
public sealed class ForumManager
{
    [Command("issueify")]
    public async ValueTask TransferForumPostAsync(CommandContext ctx)
    {
    }
}

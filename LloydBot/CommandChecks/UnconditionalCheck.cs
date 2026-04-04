using LloydBot.Context;
using LloydBot.Models.Main;
using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;

namespace LloydBot.CommandChecks;

public class UnconditionalCheck(LloydBotContext dbContext) : IContextCheck<UnconditionalCheckAttribute>
{
    public ValueTask<string?> ExecuteCheckAsync(UnconditionalCheckAttribute attribute, CommandContext context)
    {
        ulong guildid = context.Guild?.Id ?? ulong.MaxValue;
        BlacklistedDbEntity? blacklistedEntity = dbContext.Set<BlacklistedDbEntity>()
            .Where(bl => bl.UserId == context.User.Id || bl.GuildId == guildid)
            .FirstOrDefault();

        if (blacklistedEntity is null)
        {
            // The user or guild is in good standing
            return ValueTask.FromResult<string?>(null);
        }

        return blacklistedEntity.UserId is 0
            ? ValueTask.FromResult<string?>($"This guild has been banned from LloydBot!\n{blacklistedEntity.BanReason()}")
            : ValueTask.FromResult<string?>($"You were banned from using LloydBot!\n{blacklistedEntity.BanReason()}");
    }
}
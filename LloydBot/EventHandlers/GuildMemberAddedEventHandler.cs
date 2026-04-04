using LloydBot.Context;
using LloydBot.Models.Main;
using DSharpPlus;
using DSharpPlus.EventArgs;

namespace LloydBot.EventHandlers;

public sealed class GuildMemberAddedEventHandler(LloydBotContext _dbContext) : IEventHandler<GuildMemberAddedEventArgs>
{
    public async Task HandleEventAsync(DiscordClient sender, GuildMemberAddedEventArgs eventArgs)
    {
        var userSet = _dbContext.Set<UserDbEntity>();
        if (await userSet.FindAsync(eventArgs.Member.Id) is not null)
        {
            return;
        }

        userSet.Add(new(eventArgs.Member, DateTimeOffset.UtcNow));
        await _dbContext.SaveChangesAsync();
    }
}

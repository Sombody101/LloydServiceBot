using LloydBot.Context;
using LloydBot.Models.Main;
using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;
using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore;

namespace LloydBot.CommandChecks;

public class EnsureDBEntitiesCheck : IContextCheck<UnconditionalCheckAttribute>
{
    private readonly IDbContextFactory<LloydBotContext> _contextFactory;

    public EnsureDBEntitiesCheck(IDbContextFactory<LloydBotContext> dbContextFactory)
    {
        _contextFactory = dbContextFactory;
    }

    public async ValueTask<string?> ExecuteCheckAsync(UnconditionalCheckAttribute __, CommandContext context)
    {
        DiscordUser user = context.User;

        await using LloydBotContext dbContext = await _contextFactory.CreateDbContextAsync();

        UserDbEntity userdbEntity = new(user);

        _ = await dbContext.Users.Upsert(userdbEntity)
            .On(x => x.Id)
            .NoUpdate()
            .RunAsync();

        if (context.Guild is null)
        {
            return null;
        }

        GuildDbEntity guildDbEntity = new(context.Guild);

        _ = await dbContext.Guilds.Upsert(guildDbEntity)
            .On(x => x.Id)
            .NoUpdate()
            .RunAsync();

        _ = await dbContext.SaveChangesAsync();
        return null;
    }
}
using LloydBot.CommandChecks.Attributes;
using LloydBot.Context;
using LloydBot.Models.Main;
using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;

namespace LloydBot.CommandChecks;

public class RequireAdminUserCheck : IContextCheck<RequireAdminUserAttribute>
{
    private readonly LloydBotContext _dbContext;

    public RequireAdminUserCheck(LloydBotContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async ValueTask<string?> ExecuteCheckAsync(RequireAdminUserAttribute? _, CommandContext context)
    {
        UserDbEntity? user = await _dbContext.Users.FindAsync(context.User.Id);

        return user is null || (!user.IsBotAdmin && !RequireOwnerCheck.IsOwner(context))
            ? "You need to be a bot administrator!"
            : null;
    }
}
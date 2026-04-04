using LloydBot.CommandChecks.Attributes;
using LloydBot.Context;
using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;

namespace LloydBot.CommandChecks;

public class RequireDebugOnlyCheck : IContextCheck<DebugOnlyAttribute>
{
    private readonly LloydBotContext _dbContext;

    public RequireDebugOnlyCheck(LloydBotContext _db)
    {
        _dbContext = _db;
    }

    public async ValueTask<string?> ExecuteCheckAsync(DebugOnlyAttribute? attribute, CommandContext context)
    {
#if !DEBUG
        return "This command can only be run on the Debug version of LloydBot!";
#else
        return await new RequireAdminUserCheck(_dbContext).ExecuteCheckAsync(null, context) is not null
            ? "You need to be a bot administrator to use this command while it's in the Debug stage!"
            : null;
#endif
    }
}
using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.SlashCommands.Metadata;
using DSharpPlus.Entities;
using DSharpPlus.Exceptions;
using LloydBot.CommandChecks;
using Serilog;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LloydBot.Commands.Moderation;

public static class BrickCommand
{
    private const string DEFAULT_TIMEOUT_MESSAGE = "get bricked, kid";

    private static readonly string[] brickGifs = [
        "https://c.tenor.com/pgc1fF8_SYsAAAAd/tenor.gif", // Spongebob
        "https://i.giphy.com/qhUFBEIApYR0VTfC1v.webp", // Bloody brick
        "https://gifdb.com/images/high/brick-asian-girl-throw-hit-eosuh35q558n3dyf.webp", // Light toss
        "https://media.tenor.com/AhBxuESbEQsAAAAj/jefrooo-brick.gif", // Kat
        "https://media.tenor.com/iUjXDBq2odwAAAAM/ajr-brick-throw.gif", // AJR
        "https://c.tenor.com/ijutQ8PsqEYAAAAd/tenor.gif", // Polly dog
        "https://i.giphy.com/8maQUL5HiZbYLeMqXl.webp", // Steak throw
    ];

    [Command("brick"),
        Description("Bricks the specified user and times them out with an optional message."),
        RequirePermissions(DiscordPermission.ModerateMembers)]
    public static async Task BrickTheLittleShitAsync(
        CommandContext ctx,

        [Description("The @ of the wanted user.")]
        DiscordUser user,

        [Description($"The reason for the timeout. Defaults to '{DEFAULT_TIMEOUT_MESSAGE}'.")]
        string reason = DEFAULT_TIMEOUT_MESSAGE)
    {
        DiscordMember? member = user as DiscordMember ?? await ctx.Guild?.GetMemberAsync(user.Id);

        if (member is null)
        {
            return;
        }

        await ModBrickUserAsync(ctx, member, null, reason);
    }

    public static async Task BrickTheLittleShitAsync(
        CommandContext ctx,

        [Description("The ID of the wanted user.")]
        ulong userId,

        [Description($"The reason for the timeout. Defaults to '{DEFAULT_TIMEOUT_MESSAGE}'.")]
        string reason = DEFAULT_TIMEOUT_MESSAGE)
    {
        DiscordMember member = await ctx.Guild?.GetMemberAsync(userId)!;

        if (member is null)
        {
            await ctx.RespondAsync("Failed to find any user with that ID!");
            return;
        }

        await ModBrickUserAsync(ctx, member, null, reason);
    }

    [Command("ubrick"),
        Description("Bricks the specified user with an optional message."),
        UserGuildInstallable, 
        InteractionAllowedContexts(DiscordInteractionContextType.Guild, DiscordInteractionContextType.BotDM, DiscordInteractionContextType.PrivateChannel)]
    public static async ValueTask BrickTheFriendAsync(
        CommandContext ctx,

        [Description("The @ of the wanted user.")]
        DiscordUser user,

        [Description($"The reason for the bricking. Defaults to '{DEFAULT_TIMEOUT_MESSAGE}'.")]
        string reason = DEFAULT_TIMEOUT_MESSAGE)
    {
        await BrickTheFriendAsync(ctx, user.Id, reason);
    }

    public static async ValueTask BrickTheFriendAsync(
        CommandContext ctx,

        [Description("The ID of the wanted user.")]
        ulong userid,

        [Description($"The reason for the bricking. Defaults to '{DEFAULT_TIMEOUT_MESSAGE}'.")]
        string reason = DEFAULT_TIMEOUT_MESSAGE)
    {
        await BrickMemberAsync(ctx, userid, reason);
    }

    private static async Task ModBrickUserAsync(CommandContext ctx, DiscordMember member, DateTime? duration, string? message)
    {
        duration ??= DateTime.Now.AddHours(1);

        try
        {
            await member.TimeoutAsync(duration, message);
        }
        catch (UnauthorizedException)
        {
            await ctx.RespondAsync("This user cannot be timed out!");
            return;
        }
        catch (Exception ex)
        {
            await ctx.RespondAsync("Failed to time this user out!");
            Log.Logger.Error(ex, "Failed to time user out.");
        }

        await BrickMemberAsync(ctx, member.Id, message);
    }

    private static async Task BrickMemberAsync(CommandContext ctx, ulong userid, string? message)
    {
        message ??= PickGif();

        if (!message.Contains("http"))
        {
            message += PickGif();
        }

        message = $"<@{userid}>\n{message}";

        if (ctx is SlashCommandContext slashContext)
        {
            await slashContext.RespondAsync(message);
            return;
        }

        await ctx.Channel.SendMessageAsync(message);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static string PickGif()
    {
        return $"[.]({brickGifs[Random.Shared.Next(0, brickGifs.Length)]})";
    }
}

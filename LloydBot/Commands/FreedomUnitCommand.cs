using DSharpPlus.Commands;
using DSharpPlus.Commands.Converters;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.TextCommands;
using DSharpPlus.Entities;
using System.ComponentModel;
using UnitsNet;

namespace LloydBot.Commands;

public static class FreedomUnitCommand
{
    private const double FOOTBALL_FIELD_METERS = 109.728d;

    [Command("infreedoms")]
    public static async ValueTask ConvertFreedomsAsync(
        CommandContext ctx,

        [Description("The length to be converted to Freedom Units")]
        Length length,

        [Description("When true, only sends back the value.")]
        bool shortReport = false
    )
    {
        double freedoms = ConvertLengthToFreedoms(length);

        if (shortReport)
        {
            await ctx.RespondAsync(freedoms.ToString());
            return;
        }

        await ctx.RespondAsync(
            new DiscordEmbedBuilder()
            .WithTitle($"{length.Value} {length.ToString("u").ToLower().Pluralize(length.Value)} is {freedoms:N2} football fields.")
            .WithDescription(":flag_us::eagle::flag_us::football:")
            .WithDefaultColor()
        );
    }

    private static double ConvertLengthToFreedoms(Length length)
    {
        return length.Meters / FOOTBALL_FIELD_METERS;
    }
}

public sealed class LengthArgumentConverter : ITextArgumentConverter<Length>, ISlashArgumentConverter<Length>
{
    public ConverterInputType RequiresText => ConverterInputType.Always;

    public string ReadableName => "Length";

    public DiscordApplicationCommandOptionType ParameterType => DiscordApplicationCommandOptionType.String;

    private static Optional<Length> EmptyLength => Optional.FromNoValue<Length>();

    public Task<Optional<Length>> ConvertAsync(ConverterContext context)
    {
        if (context.Argument is not string value)
        {
            return Task.FromResult(Optional.FromNoValue<Length>());
        }

        return Length.TryParse(value, out Length result)
            ? Task.FromResult(Optional.FromValue(result))
            : Task.FromResult(EmptyLength);
    }
}
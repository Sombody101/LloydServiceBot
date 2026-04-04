using LloydBot.Interactivity.Moments.Idle;
using DSharpPlus.Entities;

namespace LloydBot.Interactivity.Moments.Prompt;

public record PromptMoment : IdleMoment<IPromptComponentCreator>
{
    public required string Question { get; init; }
    public required string Placeholder { get; init; }
    public TaskCompletionSource<string?> TaskCompletionSource { get; init; } = new();

    public override async ValueTask HandleAsync(Procrastinator procrastinator, DiscordInteraction interaction)
    {
        // If this is a modal submit
        if (interaction.Type == DiscordInteractionType.ModalSubmit)
        {
            DiscordTextInputComponent? textInputComponent = interaction.Data.TextInputComponents?.FirstOrDefault(component => component.CustomId == Id.ToString());
            if (textInputComponent is null)
            {
                return;
            }

            TaskCompletionSource.SetResult(textInputComponent.Value);
            await interaction.CreateResponseAsync(DiscordInteractionResponseType.DeferredMessageUpdate);
        }

        // Try to find the text button, if applicable, and disable it
        DiscordButtonComponent findButton = ComponentCreator.CreateTextPromptButton(Question, Id);
        if (interaction.Message?.Components?.Count is not (null or 0)
            && interaction.Message.FilterComponents<DiscordButtonComponent>().Any(button => button.CustomId == findButton.CustomId))
        {
            if (interaction.Type is DiscordInteractionType.Component)
            {
                await HandleComponentAsync(procrastinator, interaction);
            }
            else if (interaction.Type is DiscordInteractionType.ModalSubmit)
            {
                await HandleModalSubmitAsync(interaction, findButton);
            }
        }
    }

    private async Task HandleComponentAsync(Procrastinator procrastinator, DiscordInteraction interaction)
    {
        // Read the data
        if (!procrastinator.TryAddData(Id, this))
        {
            // This shouldn't ever happen, but just in case
            throw new InvalidOperationException("The data could not be added to the dictionary.");
        }

        // Send the modal
        await interaction.CreateResponseAsync(DiscordInteractionResponseType.Modal, new DiscordModalBuilder()
            .WithTitle(Question)
            .WithCustomId(Id.ToString())
            .AddTextInput(ComponentCreator.CreateModalPromptButton(Question, Placeholder, Id), string.Empty)
        );
    }

    private static async Task HandleModalSubmitAsync(DiscordInteraction interaction, DiscordButtonComponent findButton)
    {
        // Update the text button to a disabled state
        DiscordWebhookBuilder responseBuilder = new(new DiscordMessageBuilder(interaction.Message));
        responseBuilder.ClearComponents();

        _ = responseBuilder.AddActionRowComponent(interaction.Message.Components.Mutate<DiscordButtonComponent>(
            button => button.CustomId == findButton.CustomId,
            button => button.Disable()
        ).Cast<DiscordButtonComponent>());

        _ = await interaction.EditOriginalResponseAsync(responseBuilder);
    }

    public override ValueTask TimedOutAsync(Procrastinator procrastinator)
    {
        TaskCompletionSource.SetResult(null);
        return base.TimedOutAsync(procrastinator);
    }
}

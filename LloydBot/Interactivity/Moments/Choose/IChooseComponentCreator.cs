using DSharpPlus.Entities;

namespace LloydBot.Interactivity.Moments.Choose;

public interface IChooseComponentCreator : IComponentCreator
{
    public DiscordSelectComponent CreateChooseDropdown(string question, IReadOnlyList<string> options, Ulid id);
}
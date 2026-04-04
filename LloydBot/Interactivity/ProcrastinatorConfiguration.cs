using LloydBot.Interactivity.Moments.Choose;
using LloydBot.Interactivity.Moments.Confirm;
using LloydBot.Interactivity.Moments.Pagination;
using LloydBot.Interactivity.Moments.Pick;
using LloydBot.Interactivity.Moments.Prompt;

namespace LloydBot.Interactivity;

public sealed record ProcrastinatorConfiguration
{
    public TimeSpan DefaultTimeout { get; init; } = TimeSpan.FromSeconds(30);
    public Dictionary<Type, IComponentCreator> ComponentCreators { get; init; } = new()
    {
        { typeof(IChooseComponentCreator), new ChooseDefaultComponentCreator() },
        { typeof(IConfirmComponentCreator), new ConfirmDefaultComponentCreator() },
        { typeof(IPaginationComponentCreator), new PaginationDefaultComponentCreator() },
        { typeof(IPickComponentCreator), new PickDefaultComponentCreator() },
        { typeof(IPromptComponentCreator), new PromptDefaultComponentCreator() }
    };

    public TComponentCreator GetComponentCreatorOrDefault<TComponentCreator, TDefaultComponentCreator>()
        where TComponentCreator : IComponentCreator
        where TDefaultComponentCreator : TComponentCreator, new()
    {
        return ComponentCreators.TryGetValue(typeof(TComponentCreator), out IComponentCreator? creator)
                    ? (TComponentCreator)creator
                    : new TDefaultComponentCreator();
    }
}

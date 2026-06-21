using R3;

namespace ProjectContext.UiRequests
{
    public interface IUiModalReadModel
    {
        ReadOnlyReactiveProperty<UiModalRequest> CurrentModal { get; }
    }
}

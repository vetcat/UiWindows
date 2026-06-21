using R3;

namespace ProjectContext.UiRequests
{
    public interface IUiFeedbackReadModel
    {
        Observable<UiHintRequest> HintRequests { get; }
        Observable<UiFxRequest> FxRequests { get; }
    }
}

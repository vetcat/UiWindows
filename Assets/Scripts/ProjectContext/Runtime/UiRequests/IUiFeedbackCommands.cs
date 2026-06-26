namespace ProjectContext.UiRequests
{
    public interface IUiFeedbackCommands
    {
        void ShowHint(string description, UiHintAnchor anchor = UiHintAnchor.Center, float durationSeconds = 1.5f);
        void RequestCollectFx(int amount, UiFxTarget target = UiFxTarget.Coins);
        void RequestCollectFxFrom(int amount, UiFxTarget source, UiFxTarget target = UiFxTarget.Coins);
        void RequestSpendFx(int amount, UiFxTarget target = UiFxTarget.Coins);
        void RequestSpendFxFrom(int amount, UiFxTarget source, UiFxTarget target = UiFxTarget.Coins);
    }
}

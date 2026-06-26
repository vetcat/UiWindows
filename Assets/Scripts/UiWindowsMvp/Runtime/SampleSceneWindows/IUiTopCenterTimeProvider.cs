using System;

namespace UiWindowsMvp.SampleSceneWindows
{
    public interface IUiTopCenterTimeProvider
    {
        DateTime UtcNow { get; }
        IDisposable SubscribeUtcTimeChanged(Action callback);
    }
}

using System;

namespace UiWindowsMvp.SampleSceneWindows
{
    public interface IUiTopCenterHoldTimer
    {
        IDisposable Start(TimeSpan delay, Action completed);
    }
}

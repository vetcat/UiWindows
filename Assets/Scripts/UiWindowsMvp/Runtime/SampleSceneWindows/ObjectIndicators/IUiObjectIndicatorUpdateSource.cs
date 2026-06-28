using System;

namespace UiWindowsMvp.SampleSceneWindows
{
    public interface IUiObjectIndicatorUpdateSource
    {
        IDisposable Register(Action update);
    }
}

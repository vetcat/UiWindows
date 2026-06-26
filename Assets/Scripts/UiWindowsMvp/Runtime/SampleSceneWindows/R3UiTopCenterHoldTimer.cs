using System;
using R3;

namespace UiWindowsMvp.SampleSceneWindows
{
    public sealed class R3UiTopCenterHoldTimer : IUiTopCenterHoldTimer
    {
        public IDisposable Start(TimeSpan delay, Action completed)
        {
            if (completed == null)
            {
                throw new ArgumentNullException(nameof(completed));
            }

            if (delay <= TimeSpan.Zero)
            {
                completed();
                return new DisposableAction(() => { });
            }

            return Observable.Timer(delay).Subscribe(_ => completed());
        }
    }
}

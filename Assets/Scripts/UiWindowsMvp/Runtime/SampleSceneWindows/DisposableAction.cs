using System;

namespace UiWindowsMvp.SampleSceneWindows
{
    internal sealed class DisposableAction : IDisposable
    {
        private Action action;

        public DisposableAction(Action action)
        {
            this.action = action ?? throw new ArgumentNullException(nameof(action));
        }

        public void Dispose()
        {
            var callback = action;
            action = null;
            callback?.Invoke();
        }
    }
}

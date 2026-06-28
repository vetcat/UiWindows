using System;
using System.Collections.Generic;
using R3;

namespace UiWindowsMvp.SampleSceneWindows
{
    public sealed class R3UiObjectIndicatorUpdateSource : IUiObjectIndicatorUpdateSource, IDisposable
    {
        private readonly List<Action> subscribers = new();
        private IDisposable updateSubscription;
        private bool disposed;

        public int SubscriberCount => subscribers.Count;

        public bool IsRunning => updateSubscription != null;

        public IDisposable Register(Action update)
        {
            if (update == null)
            {
                throw new ArgumentNullException(nameof(update));
            }

            ThrowIfDisposed();

            subscribers.Add(update);
            updateSubscription ??= Observable.EveryUpdate().Subscribe(_ => Tick());

            return new DisposableAction(() => Unregister(update));
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            updateSubscription?.Dispose();
            updateSubscription = null;
            subscribers.Clear();
        }

        private void Tick()
        {
            for (var i = 0; i < subscribers.Count; i++)
            {
                subscribers[i]?.Invoke();
            }
        }

        private void Unregister(Action update)
        {
            if (disposed)
            {
                return;
            }

            subscribers.Remove(update);
            if (subscribers.Count > 0)
            {
                return;
            }

            updateSubscription?.Dispose();
            updateSubscription = null;
        }

        private void ThrowIfDisposed()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(nameof(R3UiObjectIndicatorUpdateSource));
            }
        }
    }
}

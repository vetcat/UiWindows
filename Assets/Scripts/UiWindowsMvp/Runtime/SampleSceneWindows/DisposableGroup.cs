using System;
using System.Collections.Generic;

namespace UiWindowsMvp.SampleSceneWindows
{
    internal sealed class DisposableGroup : IDisposable
    {
        private readonly List<IDisposable> disposables = new();
        private bool disposed;

        public void Add(IDisposable disposable)
        {
            if (disposable == null)
            {
                return;
            }

            if (disposed)
            {
                disposable.Dispose();
                return;
            }

            disposables.Add(disposable);
        }

        public void Clear()
        {
            for (var i = disposables.Count - 1; i >= 0; i--)
            {
                disposables[i]?.Dispose();
            }

            disposables.Clear();
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            Clear();
        }
    }
}

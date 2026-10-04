using System;
using System.Collections.Generic;

namespace UiWindowsMvp.UIAdapter
{
    public sealed class WindowPresenterShowScope : IUiShowScope, IDisposable
    {
        private readonly List<IDisposable> disposables = new List<IDisposable>();

        public bool IsDisposed { get; private set; }

        public void Add(IDisposable disposable)
        {
            if (disposable == null)
            {
                throw new ArgumentNullException(nameof(disposable));
            }

            if (IsDisposed)
            {
                throw new ObjectDisposedException(nameof(WindowPresenterShowScope));
            }

            disposables.Add(disposable);
        }

        public void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }

            IsDisposed = true;

            List<Exception> exceptions = null;
            for (var i = disposables.Count - 1; i >= 0; i--)
            {
                try
                {
                    disposables[i].Dispose();
                }
                catch (Exception exception)
                {
                    exceptions ??= new List<Exception>();
                    exceptions.Add(exception);
                }
            }

            disposables.Clear();

            if (exceptions != null)
            {
                throw new AggregateException(exceptions);
            }
        }
    }
}

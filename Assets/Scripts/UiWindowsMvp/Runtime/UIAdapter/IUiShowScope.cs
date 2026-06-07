using System;

namespace UiWindowsMvp.UIAdapter
{
    public interface IUiShowScope
    {
        bool IsDisposed { get; }

        void Add(IDisposable disposable);
    }
}

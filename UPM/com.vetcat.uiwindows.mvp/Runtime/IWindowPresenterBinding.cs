using System;
using UnityEngine.UI.Windows;

namespace UiWindowsMvp.UIAdapter
{
    public interface IWindowPresenterBinding : IDisposable
    {
        WindowBase Window { get; }

        IUiPresenter Presenter { get; }

        bool IsDisposed { get; }

        void OnWindowInitialized();

        void OnWindowShowBegin();

        void OnWindowShowEnd();

        void OnWindowHideBegin();

        void OnWindowHideEnd();

        void OnWindowDeInitialized();
    }

    public interface IWindowPresenterBinding<out TWindow> : IWindowPresenterBinding
        where TWindow : WindowBase
    {
        new TWindow Window { get; }
    }
}

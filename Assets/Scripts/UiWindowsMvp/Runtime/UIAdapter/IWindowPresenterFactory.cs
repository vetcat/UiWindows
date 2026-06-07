using UnityEngine.UI.Windows;

namespace UiWindowsMvp.UIAdapter
{
    public interface IWindowPresenterFactory<TWindow>
        where TWindow : WindowBase
    {
        IWindowPresenter<TWindow> Create(TWindow window);
    }
}

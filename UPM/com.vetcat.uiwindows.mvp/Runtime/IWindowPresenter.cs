using UnityEngine.UI.Windows;

namespace UiWindowsMvp.UIAdapter
{
    public interface IWindowPresenter<in TWindow> : IUiPresenter
        where TWindow : WindowBase
    {
        void Bind(TWindow window);
    }
}

using UnityEngine.UI.Windows;
using UnityEngine.UI.Windows.WindowTypes;

namespace UiWindowsMvp.SampleSceneWindows
{
    public sealed class UiDownLeftWindow : LayoutWindowType
    {
        public bool TryGetView(out UiDownLeftView view)
        {
            if (GetLayoutComponent(out view, Algorithm.GetFirstTypeAny))
            {
                return true;
            }

            view = FindComponent<UiDownLeftView>();
            return view != null;
        }
    }
}

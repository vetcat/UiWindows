using UnityEngine.UI.Windows;
using UnityEngine.UI.Windows.WindowTypes;

namespace UiWindowsMvp.SampleSceneWindows
{
    public sealed class UiObjectIndicatorWindow : LayoutWindowType
    {
        public bool TryGetView(out UiObjectIndicatorView view)
        {
            if (GetLayoutComponent(out view, Algorithm.GetFirstTypeAny))
            {
                return true;
            }

            view = FindComponent<UiObjectIndicatorView>();
            return view != null;
        }
    }
}

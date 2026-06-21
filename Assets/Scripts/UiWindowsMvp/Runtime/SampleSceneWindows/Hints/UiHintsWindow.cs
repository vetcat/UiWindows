using UnityEngine.UI.Windows;
using UnityEngine.UI.Windows.WindowTypes;

namespace UiWindowsMvp.SampleSceneWindows
{
    public sealed class UiHintsWindow : LayoutWindowType
    {
        public bool TryGetView(out UiHintsView view)
        {
            if (GetLayoutComponent(out view, Algorithm.GetFirstTypeAny))
            {
                return true;
            }

            view = FindComponent<UiHintsView>();
            return view != null;
        }
    }
}

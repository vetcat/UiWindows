using UnityEngine.UI.Windows;
using UnityEngine.UI.Windows.WindowTypes;

namespace UiWindowsMvp.SampleSceneWindows
{
    public sealed class UiTopCenterWindow : LayoutWindowType
    {
        public bool TryGetView(out UiTopCenterView view)
        {
            if (GetLayoutComponent(out view, Algorithm.GetFirstTypeAny))
            {
                return true;
            }

            view = FindComponent<UiTopCenterView>();
            return view != null;
        }
    }
}

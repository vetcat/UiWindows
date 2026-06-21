using UnityEngine.UI.Windows;
using UnityEngine.UI.Windows.WindowTypes;

namespace UiWindowsMvp.SampleSceneWindows
{
    public sealed class UiTopRightWindow : LayoutWindowType
    {
        public bool TryGetView(out UiTopRightView view)
        {
            if (GetLayoutComponent(out view, Algorithm.GetFirstTypeAny))
            {
                return true;
            }

            view = FindComponent<UiTopRightView>();
            return view != null;
        }
    }
}

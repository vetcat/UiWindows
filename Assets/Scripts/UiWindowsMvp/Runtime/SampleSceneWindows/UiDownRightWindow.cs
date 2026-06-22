using UnityEngine.UI.Windows;
using UnityEngine.UI.Windows.WindowTypes;

namespace UiWindowsMvp.SampleSceneWindows
{
    public sealed class UiDownRightWindow : LayoutWindowType
    {
        public bool TryGetView(out UiDownRightView view)
        {
            if (GetLayoutComponent(out view, Algorithm.GetFirstTypeAny))
            {
                return true;
            }

            view = FindComponent<UiDownRightView>();
            return view != null;
        }
    }
}

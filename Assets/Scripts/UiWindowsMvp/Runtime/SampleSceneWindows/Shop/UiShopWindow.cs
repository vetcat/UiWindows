using UnityEngine.UI.Windows;
using UnityEngine.UI.Windows.WindowTypes;

namespace UiWindowsMvp.SampleSceneWindows
{
    public sealed class UiShopWindow : LayoutWindowType
    {
        public bool TryGetView(out UiShopView view)
        {
            if (GetLayoutComponent(out view, Algorithm.GetFirstTypeAny))
            {
                return true;
            }

            view = FindComponent<UiShopView>();
            return view != null;
        }
    }
}

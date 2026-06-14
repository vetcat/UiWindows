using UnityEngine.UI.Windows;
using UnityEngine.UI.Windows.WindowTypes;

namespace UiWindowsMvp.SampleSceneWindows
{
    public sealed class UiTopLeftWindow : LayoutWindowType
    {
        public bool TryGetView(out UiTopLeftView view)
        {
            return GetLayoutComponent(out view, Algorithm.GetFirstTypeAny);
        }
    }
}
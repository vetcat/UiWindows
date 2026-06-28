using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Windows;

namespace UiWindowsMvp.SampleSceneWindows
{
    public sealed class UiDownLeftView : WindowComponent
    {
        public RectTransform Body;
        public Button ButtonItemsShop;
        public Text TextItemsShop;
        public Image ImageItemsShop;

        public void EnsureLayout()
        {
            if (Body != null && ButtonItemsShop != null && TextItemsShop != null && ImageItemsShop != null)
            {
                return;
            }

            throw UiPrefabReferenceGuard.Missing(
                this,
                "Body, ButtonItemsShop, TextItemsShop, ImageItemsShop");
        }

        public void SetItemsShopText(string value)
        {
            EnsureLayout();
            if (TextItemsShop != null)
            {
                TextItemsShop.text = value;
            }
        }
    }
}

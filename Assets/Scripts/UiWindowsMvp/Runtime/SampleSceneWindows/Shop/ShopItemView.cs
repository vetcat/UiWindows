using ProjectContext.Shop;
using UnityEngine;
using UnityEngine.UI;

namespace UiWindowsMvp.SampleSceneWindows
{
    public sealed class ShopItemView : MonoBehaviour
    {
        public Button ButtonItem;
        public Image ImageIcon;
        public Image ImageSelection;
        public Text TextName;
        public Text TextAmount;
        [HideInInspector] public ShopItemType Type;

        public void SetSelected(bool selected)
        {
            if (ImageSelection != null)
            {
                var color = ImageSelection.color;
                color.a = selected ? 0.85f : 0.08f;
                ImageSelection.color = color;
            }
        }
    }
}

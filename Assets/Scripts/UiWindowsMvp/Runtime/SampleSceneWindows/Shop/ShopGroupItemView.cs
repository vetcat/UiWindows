using ProjectContext.Shop;
using UnityEngine;
using UnityEngine.UI;

namespace UiWindowsMvp.SampleSceneWindows
{
    public sealed class ShopGroupItemView : MonoBehaviour
    {
        public Toggle Toggle;
        public Image ImageIcon;
        public Image ImageGlow;
        public Text TextName;
        [HideInInspector] public ShopItemGroup Group;

        public void SetSelected(bool selected)
        {
            if (ImageGlow != null)
            {
                var color = ImageGlow.color;
                color.a = selected ? 0.85f : 0.12f;
                ImageGlow.color = color;
            }

            if (Toggle != null)
            {
                Toggle.SetIsOnWithoutNotify(selected);
            }
        }
    }
}

using System;
using UnityEngine.UI;

namespace UiWindowsMvp.SampleSceneWindows
{
    [Serializable]
    public sealed class ShopDetailsLayout
    {
        public Image ImageIcon;
        public Text TextName;
        public Text TextAmount;
        public Text TextGroup;
        public Text TextDescription;
    }
}

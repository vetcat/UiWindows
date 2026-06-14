using System;
using UnityEngine.UI;

namespace UiWindowsMvp.SampleSceneWindows
{
    [Serializable]
    public sealed class PlayerDataLayout
    {
        public Image ImageIcon;
        public Text TextValue;
        public Slider Slider;
        public Button ButtonAdd;
        public Button ButtonReduce;
    }
}
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Windows;

namespace UiWindowsMvp.SampleSceneWindows
{
    public sealed class UiTopLeftView : WindowComponent
    {
        public RectTransform Body;
        public Text TextPlayerName;
        public Text TextLevel;
        public Text TextLevelValue;

        public PlayerDataLayout HealthData;
        public PlayerDataLayout XpData;
    }
}
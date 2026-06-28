using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Windows;

namespace UiWindowsMvp.SampleSceneWindows
{
    public sealed class UiTopCenterView : WindowComponent
    {
        public RectTransform Body;
        public Image HintArea;
        public Text TextLocalTime;
        public Text TextPressAndHold;
        public UiTopCenterHoldInput HoldInput;

        public void EnsureLayout()
        {
            if (Body != null &&
                HintArea != null &&
                TextLocalTime != null &&
                TextPressAndHold != null &&
                HoldInput != null)
            {
                return;
            }

            throw UiPrefabReferenceGuard.Missing(
                this,
                "Body, HintArea, TextLocalTime, TextPressAndHold, HoldInput");
        }

        public void SetTimeText(string value)
        {
            EnsureLayout();
            if (TextLocalTime != null)
            {
                TextLocalTime.text = value ?? string.Empty;
            }
        }

        public void SetPressAndHoldText(string value)
        {
            EnsureLayout();
            if (TextPressAndHold != null)
            {
                TextPressAndHold.text = value ?? string.Empty;
            }
        }
    }
}

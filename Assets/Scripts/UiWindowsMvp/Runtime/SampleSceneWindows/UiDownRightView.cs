using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Windows;

namespace UiWindowsMvp.SampleSceneWindows
{
    public sealed class UiDownRightView : WindowComponent
    {
        public RectTransform Body;
        public Button ButtonSettings;
        public Text TextSettings;
        public Image ImageSettings;

        public void EnsureLayout()
        {
            if (Body != null && ButtonSettings != null && TextSettings != null && ImageSettings != null)
            {
                return;
            }

            throw UiPrefabReferenceGuard.Missing(
                this,
                "Body, ButtonSettings, TextSettings, ImageSettings");
        }

        public void SetSettingsText(string value)
        {
            EnsureLayout();
            if (TextSettings != null)
            {
                TextSettings.text = value;
            }
        }
    }
}

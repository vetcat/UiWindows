using System;
using UnityEngine;
using UnityEngine.UI;

namespace UiWindowsMvp.SampleSceneWindows
{
    [Serializable]
    public sealed class SettingsLayout
    {
        public CanvasGroup CanvasGroup;
        public LayoutElement LayoutElement;
        public Slider SliderMusicVolume;
        public Slider SliderSoundVolume;
        public Text TextMusic;
        public Text TextSound;
        public Text TextSliderMusicValue;
        public Text TextSliderSoundValue;

        public void SetVisible(bool visible)
        {
            if (CanvasGroup != null)
            {
                CanvasGroup.alpha = visible ? 1f : 0f;
                CanvasGroup.interactable = visible;
                CanvasGroup.blocksRaycasts = visible;
            }

            if (LayoutElement != null)
            {
                LayoutElement.ignoreLayout = !visible;
            }
        }
    }
}
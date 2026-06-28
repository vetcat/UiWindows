using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Windows;

namespace UiWindowsMvp.SampleSceneWindows
{
    public sealed class UiObjectIndicatorView : WindowComponent
    {
        public RectTransform Body;
        public RectTransform ItemRoot;
        public CanvasGroup ItemCanvasGroup;
        public Image ImageIcon;
        public Button ButtonAction;
        public Text TextInfo;
        public Slider Slider;
        public Text TextSliderValue;
        public Text TextClickToReward;

        private Vector2 lastAnchoredPosition;
        private bool lastVisibility;

        public RectTransform RewardAnchor
        {
            get
            {
                if (ButtonAction != null)
                {
                    return ButtonAction.transform as RectTransform;
                }

                return ItemRoot;
            }
        }

        public Vector2 LastAnchoredPosition => lastAnchoredPosition;
        public bool IsIndicatorVisible => lastVisibility;
        public string LastInfoText => TextInfo != null ? TextInfo.text : string.Empty;
        public float HealthSliderValue => Slider != null ? Slider.value : 0f;

        public void EnsureLayout()
        {
            if (Body != null &&
                ItemRoot != null &&
                ItemCanvasGroup != null &&
                ImageIcon != null &&
                ButtonAction != null &&
                TextInfo != null &&
                Slider != null &&
                TextSliderValue != null &&
                TextClickToReward != null)
            {
                return;
            }

            throw UiPrefabReferenceGuard.Missing(
                this,
                "Body, ItemRoot, ItemCanvasGroup, ImageIcon, ButtonAction, TextInfo, Slider, " +
                "TextSliderValue, TextClickToReward");
        }

        public void SetIndicatorVisible(bool visible)
        {
            EnsureLayout();
            lastVisibility = visible;
            ItemCanvasGroup.alpha = visible ? 1f : 0f;
            ItemCanvasGroup.interactable = visible;
            ItemCanvasGroup.blocksRaycasts = visible;
        }

        public void SetIndicatorScreenPosition(Vector2 screenPosition)
        {
            EnsureLayout();
            var camera = GetRectTransformCamera(Body);
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    Body,
                    screenPosition,
                    camera,
                    out var localPosition) == false)
            {
                return;
            }

            ItemRoot.anchoredPosition = localPosition;
            lastAnchoredPosition = localPosition;
        }

        public void SetInfo(string value)
        {
            EnsureLayout();
            TextInfo.text = value ?? string.Empty;
        }

        public void SetRewardAmount(int amount)
        {
            EnsureLayout();
            TextClickToReward.text = "Reward +" + Mathf.Max(0, amount);
        }

        public void SetHealth(int value, int maxValue)
        {
            EnsureLayout();
            var clampedMax = Mathf.Max(1, maxValue);
            var clampedValue = Mathf.Clamp(value, 0, clampedMax);
            Slider.value = (float)clampedValue / clampedMax;
            TextSliderValue.text = clampedValue.ToString();
        }

        private static Camera GetRectTransformCamera(RectTransform rectTransform)
        {
            var canvas = rectTransform.GetComponentInParent<Canvas>();
            if (canvas == null || canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                return null;
            }

            return canvas.worldCamera;
        }
    }
}

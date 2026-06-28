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

            BuildDefaultLayout();
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

        private void BuildDefaultLayout()
        {
            var root = (RectTransform)transform;
            Stretch(root);

            Body = CreateRect("Body", transform);
            Stretch(Body);

            ItemRoot = CreateRect("IndicatorItem", Body);
            ItemRoot.anchorMin = new Vector2(0.5f, 0.5f);
            ItemRoot.anchorMax = new Vector2(0.5f, 0.5f);
            ItemRoot.pivot = new Vector2(0.5f, 0f);
            ItemRoot.sizeDelta = new Vector2(260f, 100f);
            ItemRoot.anchoredPosition = Vector2.zero;

            ItemCanvasGroup = ItemRoot.gameObject.AddComponent<CanvasGroup>();
            var background = ItemRoot.gameObject.AddComponent<Image>();
            background.color = new Color(0.08f, 0.09f, 0.12f, 0.88f);
            background.raycastTarget = false;

            ImageIcon = CreateImage("ImageIcon", ItemRoot, new Color(0.95f, 0.78f, 0.26f, 1f));
            var iconRect = (RectTransform)ImageIcon.transform;
            iconRect.anchorMin = new Vector2(0f, 0.5f);
            iconRect.anchorMax = new Vector2(0f, 0.5f);
            iconRect.pivot = new Vector2(0f, 0.5f);
            iconRect.anchoredPosition = new Vector2(12f, 6f);
            iconRect.sizeDelta = new Vector2(48f, 48f);

            TextInfo = CreateText("Character", ItemRoot, 16, TextAnchor.MiddleLeft);
            var infoRect = (RectTransform)TextInfo.transform;
            infoRect.anchorMin = new Vector2(0f, 1f);
            infoRect.anchorMax = new Vector2(1f, 1f);
            infoRect.pivot = new Vector2(0f, 1f);
            infoRect.offsetMin = new Vector2(70f, -34f);
            infoRect.offsetMax = new Vector2(-12f, -8f);

            Slider = CreateSlider("HealthSlider", ItemRoot);
            var sliderRect = (RectTransform)Slider.transform;
            sliderRect.anchorMin = new Vector2(0f, 0.5f);
            sliderRect.anchorMax = new Vector2(1f, 0.5f);
            sliderRect.offsetMin = new Vector2(70f, -4f);
            sliderRect.offsetMax = new Vector2(-58f, 10f);

            TextSliderValue = CreateText("100", ItemRoot, 13, TextAnchor.MiddleRight);
            var valueRect = (RectTransform)TextSliderValue.transform;
            valueRect.anchorMin = new Vector2(1f, 0.5f);
            valueRect.anchorMax = new Vector2(1f, 0.5f);
            valueRect.pivot = new Vector2(1f, 0.5f);
            valueRect.anchoredPosition = new Vector2(-12f, 3f);
            valueRect.sizeDelta = new Vector2(42f, 24f);

            ButtonAction = CreateButton("ButtonAction", ItemRoot);
            var buttonRect = (RectTransform)ButtonAction.transform;
            buttonRect.anchorMin = new Vector2(0f, 0f);
            buttonRect.anchorMax = new Vector2(1f, 0f);
            buttonRect.pivot = new Vector2(0.5f, 0f);
            buttonRect.offsetMin = new Vector2(70f, 12f);
            buttonRect.offsetMax = new Vector2(-12f, 40f);

            TextClickToReward = CreateText("Reward +25", buttonRect, 14, TextAnchor.MiddleCenter);
            Stretch((RectTransform)TextClickToReward.transform);
        }

        private static RectTransform CreateRect(string name, Transform parent)
        {
            var gameObject = new GameObject(name, typeof(RectTransform));
            gameObject.transform.SetParent(parent, false);
            var rectTransform = (RectTransform)gameObject.transform;
            rectTransform.localScale = Vector3.one;
            return rectTransform;
        }

        private static Image CreateImage(string name, Transform parent, Color color)
        {
            var gameObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            gameObject.transform.SetParent(parent, false);
            var image = gameObject.GetComponent<Image>();
            image.color = color;
            image.raycastTarget = false;
            return image;
        }

        private static Text CreateText(string value, Transform parent, int fontSize, TextAnchor alignment)
        {
            var gameObject = new GameObject(value + " Text", typeof(RectTransform), typeof(CanvasRenderer),
                typeof(Text));
            gameObject.transform.SetParent(parent, false);
            var text = gameObject.GetComponent<Text>();
            text.text = value;
            text.color = Color.white;
            text.fontSize = fontSize;
            text.alignment = alignment;
            text.raycastTarget = false;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            return text;
        }

        private static Button CreateButton(string name, Transform parent)
        {
            var image = CreateImage(name, parent, new Color(0.18f, 0.42f, 0.78f, 0.95f));
            image.raycastTarget = true;
            var button = image.gameObject.AddComponent<Button>();
            button.targetGraphic = image;
            return button;
        }

        private static Slider CreateSlider(string name, Transform parent)
        {
            var sliderRoot = CreateRect(name, parent);
            var background = CreateImage("Background", sliderRoot, new Color(0.18f, 0.19f, 0.23f, 1f));
            Stretch((RectTransform)background.transform);

            var fill = CreateImage("Fill", sliderRoot, new Color(0.34f, 0.78f, 0.45f, 1f));
            var fillRect = (RectTransform)fill.transform;
            Stretch(fillRect);

            var slider = sliderRoot.gameObject.AddComponent<Slider>();
            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.value = 1f;
            slider.fillRect = fillRect;
            slider.targetGraphic = fill;
            slider.interactable = false;
            return slider;
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

        private static void Stretch(RectTransform rectTransform)
        {
            rectTransform.localScale = Vector3.one;
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = Vector2.zero;
        }
    }
}

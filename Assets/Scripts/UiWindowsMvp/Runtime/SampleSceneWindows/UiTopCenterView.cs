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

            BuildDefaultLayout();
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

        private void BuildDefaultLayout()
        {
            var root = (RectTransform)transform;
            Stretch(root);

            var rootContainer = CreateRect("Root", root);
            Stretch(rootContainer);

            Body = CreateRect("Body", rootContainer);
            Body.anchorMin = new Vector2(0.5f, 1f);
            Body.anchorMax = new Vector2(0.5f, 1f);
            Body.pivot = new Vector2(0.5f, 1f);
            Body.anchoredPosition = Vector2.zero;
            Body.sizeDelta = new Vector2(420f, 76f);

            HintArea = Body.gameObject.AddComponent<Image>();
            HintArea.color = Color.white;
            HintArea.raycastTarget = true;
            HintArea.maskable = false;
            HoldInput = Body.gameObject.AddComponent<UiTopCenterHoldInput>();

            var clockIcon = CreateImage("ImageClock", Body, new Color(0.16f, 0.48f, 0.92f, 1f));
            var clockRect = (RectTransform)clockIcon.transform;
            clockRect.anchorMin = new Vector2(0f, 1f);
            clockRect.anchorMax = new Vector2(0f, 1f);
            clockRect.pivot = new Vector2(0f, 1f);
            clockRect.anchoredPosition = new Vector2(15f, -5f);
            clockRect.sizeDelta = new Vector2(50f, 50f);

            TextLocalTime = CreateText("TextLocalTime", Body, "Time", 20, TextAnchor.MiddleLeft);
            var timeRect = (RectTransform)TextLocalTime.transform;
            timeRect.anchorMin = new Vector2(0f, 1f);
            timeRect.anchorMax = new Vector2(1f, 1f);
            timeRect.pivot = new Vector2(0.5f, 1f);
            timeRect.anchoredPosition = new Vector2(37.5f, -5f);
            timeRect.sizeDelta = new Vector2(-95f, 50f);

            TextPressAndHold = CreateText("TextPressAndHold", Body, "press and hold", 18, TextAnchor.MiddleCenter);
            var holdRect = (RectTransform)TextPressAndHold.transform;
            holdRect.anchorMin = new Vector2(0f, 0f);
            holdRect.anchorMax = new Vector2(1f, 0f);
            holdRect.pivot = new Vector2(0.5f, 0.5f);
            holdRect.anchoredPosition = new Vector2(0f, 14f);
            holdRect.sizeDelta = new Vector2(-30f, 20f);
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
            image.maskable = false;
            return image;
        }

        private static Text CreateText(string name, Transform parent, string value, int fontSize, TextAnchor alignment)
        {
            var gameObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            gameObject.transform.SetParent(parent, false);
            var text = gameObject.GetComponent<Text>();
            text.text = value;
            text.color = new Color(0.19607843f, 0.19607843f, 0.19607843f, 1f);
            text.fontSize = fontSize;
            text.fontStyle = FontStyle.Bold;
            text.resizeTextForBestFit = name == "TextPressAndHold";
            text.resizeTextMinSize = 10;
            text.resizeTextMaxSize = fontSize;
            text.alignment = alignment;
            text.raycastTarget = false;
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            return text;
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

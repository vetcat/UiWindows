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

            BuildDefaultLayout();
        }

        public void SetSettingsText(string value)
        {
            EnsureLayout();
            if (TextSettings != null)
            {
                TextSettings.text = value;
            }
        }

        private void BuildDefaultLayout()
        {
            var root = (RectTransform)transform;
            Stretch(root);

            var rootContainer = CreateRect("Root", root);
            Stretch(rootContainer);

            Body = CreateRect("Body", rootContainer);
            Body.anchorMin = new Vector2(1f, 0f);
            Body.anchorMax = new Vector2(1f, 0f);
            Body.pivot = new Vector2(1f, 1f);
            Body.anchoredPosition = new Vector2(0f, 70f);
            Body.sizeDelta = new Vector2(240f, 70f);

            var background = Body.gameObject.AddComponent<Image>();
            background.color = Color.white;
            background.raycastTarget = true;
            background.maskable = false;

            ButtonSettings = Body.gameObject.AddComponent<Button>();
            ButtonSettings.targetGraphic = background;

            TextSettings = CreateText("TextSettings", Body);
            var textRect = (RectTransform)TextSettings.transform;
            textRect.anchorMin = new Vector2(0f, 0.5f);
            textRect.anchorMax = new Vector2(0f, 0.5f);
            textRect.pivot = new Vector2(0f, 0.5f);
            textRect.anchoredPosition = new Vector2(20f, 0f);
            textRect.sizeDelta = new Vector2(150f, 54f);

            ImageSettings = CreateImage("ImageSettings", Body);
            var iconRect = (RectTransform)ImageSettings.transform;
            iconRect.anchorMin = new Vector2(1f, 0.5f);
            iconRect.anchorMax = new Vector2(1f, 0.5f);
            iconRect.pivot = new Vector2(1f, 0.5f);
            iconRect.anchoredPosition = new Vector2(-20f, 0f);
            iconRect.sizeDelta = new Vector2(50f, 50f);
        }

        private static RectTransform CreateRect(string name, Transform parent)
        {
            var gameObject = new GameObject(name, typeof(RectTransform));
            gameObject.transform.SetParent(parent, false);
            var rectTransform = (RectTransform)gameObject.transform;
            rectTransform.localScale = Vector3.one;
            return rectTransform;
        }

        private static Image CreateImage(string name, Transform parent)
        {
            var gameObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            gameObject.transform.SetParent(parent, false);
            var image = gameObject.GetComponent<Image>();
            image.color = Color.white;
            image.raycastTarget = false;
            image.preserveAspect = true;
            return image;
        }

        private static Text CreateText(string name, Transform parent)
        {
            var gameObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            gameObject.transform.SetParent(parent, false);
            var text = gameObject.GetComponent<Text>();
            text.text = "Settings";
            text.color = new Color(0.19607843f, 0.19607843f, 0.19607843f, 1f);
            text.fontSize = 30;
            text.fontStyle = FontStyle.Bold;
            text.alignment = TextAnchor.MiddleLeft;
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            text.verticalOverflow = VerticalWrapMode.Overflow;
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

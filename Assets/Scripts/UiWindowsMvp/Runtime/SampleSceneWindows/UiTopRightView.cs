using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Windows;

namespace UiWindowsMvp.SampleSceneWindows
{
    public sealed class UiTopRightView : WindowComponent
    {
        public RectTransform Body;
        public Image ImageIconCoins;
        public Text TextCoinsAmount;

        public RectTransform CoinIconRectTransform =>
            ImageIconCoins != null ? (RectTransform)ImageIconCoins.transform : null;

        public void EnsureLayout()
        {
            if (Body != null && ImageIconCoins != null && TextCoinsAmount != null)
            {
                return;
            }

            BuildDefaultLayout();
        }

        public void SetCoins(int coins)
        {
            EnsureLayout();
            if (TextCoinsAmount != null)
            {
                TextCoinsAmount.text = coins.ToString();
            }
        }

        private void BuildDefaultLayout()
        {
            var root = (RectTransform)transform;
            Stretch(root);

            var rootContainer = CreateRect("Root", root);
            Stretch(rootContainer);

            Body = CreateRect("Body", rootContainer);
            Body.anchorMin = new Vector2(1f, 1f);
            Body.anchorMax = new Vector2(1f, 1f);
            Body.pivot = new Vector2(1f, 1f);
            Body.anchoredPosition = Vector2.zero;
            Body.sizeDelta = new Vector2(180f, 70f);

            var background = Body.gameObject.AddComponent<Image>();
            background.color = Color.white;
            background.raycastTarget = false;
            background.maskable = false;

            ImageIconCoins = CreateImage("ImageIconCoins", Body, new Color(0.95f, 0.78f, 0.26f, 1f));
            var iconRect = (RectTransform)ImageIconCoins.transform;
            iconRect.anchorMin = new Vector2(0f, 0.5f);
            iconRect.anchorMax = new Vector2(0f, 0.5f);
            iconRect.pivot = new Vector2(0f, 0.5f);
            iconRect.anchoredPosition = new Vector2(20f, 0f);
            iconRect.sizeDelta = new Vector2(50f, 50f);

            TextCoinsAmount = CreateText("TextCoinsAmount", Body);
            var textRect = (RectTransform)TextCoinsAmount.transform;
            textRect.anchorMin = new Vector2(0f, 1f);
            textRect.anchorMax = new Vector2(0f, 1f);
            textRect.pivot = new Vector2(0.5f, 0.5f);
            textRect.anchoredPosition = new Vector2(123.13623f, -35f);
            textRect.sizeDelta = new Vector2(100.918f, 30f);
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

        private static Text CreateText(string name, Transform parent)
        {
            var gameObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            gameObject.transform.SetParent(parent, false);
            var text = gameObject.GetComponent<Text>();
            text.text = "0";
            text.color = new Color(0.19607843f, 0.19607843f, 0.19607843f, 1f);
            text.fontSize = 18;
            text.fontStyle = FontStyle.Bold;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 10;
            text.resizeTextMaxSize = 18;
            text.alignment = TextAnchor.MiddleCenter;
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

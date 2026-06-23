using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Windows;

namespace UiWindowsMvp.SampleSceneWindows
{
    public sealed class UiDownLeftView : WindowComponent
    {
        public RectTransform Body;
        public Button ButtonItemsShop;
        public Text TextItemsShop;
        public Image ImageItemsShop;

        public void EnsureLayout()
        {
            if (Body != null && ButtonItemsShop != null && TextItemsShop != null && ImageItemsShop != null)
            {
                return;
            }

            BuildDefaultLayout();
        }

        public void SetItemsShopText(string value)
        {
            EnsureLayout();
            if (TextItemsShop != null)
            {
                TextItemsShop.text = value;
            }
        }

        private void BuildDefaultLayout()
        {
            var root = (RectTransform)transform;
            Stretch(root);

            var rootContainer = CreateRect("Root", root);
            Stretch(rootContainer);

            Body = CreateRect("Body", rootContainer);
            Body.anchorMin = Vector2.zero;
            Body.anchorMax = Vector2.zero;
            Body.pivot = Vector2.zero;
            Body.anchoredPosition = Vector2.zero;
            Body.sizeDelta = new Vector2(200f, 200f);

            var buttonObject = CreateRect("ButtonItemsShop", Body);
            buttonObject.anchorMin = new Vector2(0.5f, 0.5f);
            buttonObject.anchorMax = new Vector2(0.5f, 0.5f);
            buttonObject.pivot = new Vector2(0.5f, 0.5f);
            buttonObject.anchoredPosition = Vector2.zero;
            buttonObject.sizeDelta = new Vector2(200f, 200f);

            ImageItemsShop = buttonObject.gameObject.AddComponent<Image>();
            ImageItemsShop.color = Color.white;
            ImageItemsShop.raycastTarget = true;
            ImageItemsShop.preserveAspect = false;

            ButtonItemsShop = buttonObject.gameObject.AddComponent<Button>();
            ButtonItemsShop.targetGraphic = ImageItemsShop;

            TextItemsShop = CreateText("TextItemsShop", buttonObject);
            var textRect = (RectTransform)TextItemsShop.transform;
            textRect.anchorMin = new Vector2(0.5f, 0.5f);
            textRect.anchorMax = new Vector2(0.5f, 0.5f);
            textRect.pivot = new Vector2(0.5f, 0.5f);
            textRect.anchoredPosition = new Vector2(0f, 30f);
            textRect.sizeDelta = new Vector2(150f, 60f);
        }

        private static RectTransform CreateRect(string name, Transform parent)
        {
            var gameObject = new GameObject(name, typeof(RectTransform));
            gameObject.transform.SetParent(parent, false);
            var rectTransform = (RectTransform)gameObject.transform;
            rectTransform.localScale = Vector3.one;
            return rectTransform;
        }

        private static Text CreateText(string name, Transform parent)
        {
            var gameObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            gameObject.transform.SetParent(parent, false);
            var text = gameObject.GetComponent<Text>();
            text.text = "Item Shop";
            text.color = new Color(0.19607843f, 0.19607843f, 0.19607843f, 1f);
            text.fontSize = 20;
            text.fontStyle = FontStyle.Bold;
            text.alignment = TextAnchor.MiddleCenter;
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

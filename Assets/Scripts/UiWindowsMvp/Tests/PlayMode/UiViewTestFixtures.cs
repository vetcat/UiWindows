using UiWindowsMvp.SampleSceneWindows;
using UnityEngine;
using UnityEngine.UI;

namespace UiWindowsMvp.Tests.PlayMode
{
    internal static class UiViewTestFixtures
    {
        public static UiDownLeftView Configure(UiDownLeftView view)
        {
            view.Body = CreateRect("Body", view.transform);
            var buttonImage = CreateImage("ButtonItemsShop", view.Body);
            view.ButtonItemsShop = buttonImage.gameObject.AddComponent<Button>();
            view.ButtonItemsShop.targetGraphic = buttonImage;
            view.ImageItemsShop = buttonImage;
            view.TextItemsShop = CreateText("TextItemsShop", view.Body);
            return view;
        }

        public static UiTopCenterView Configure(UiTopCenterView view)
        {
            var hintArea = CreateImage("Body", view.transform);
            view.Body = hintArea.rectTransform;
            view.HintArea = hintArea;
            view.HoldInput = view.Body.gameObject.AddComponent<UiTopCenterHoldInput>();
            view.TextLocalTime = CreateText("TextLocalTime", view.Body);
            view.TextPressAndHold = CreateText("TextPressAndHold", view.Body);
            return view;
        }

        public static UiModalView Configure(UiModalView view)
        {
            view.ButtonCloseBg = CreateButton("Close Background", view.transform);
            view.Body = CreateImage("Body", view.transform).rectTransform;
            view.CanvasGroup = view.Body.gameObject.AddComponent<CanvasGroup>();
            view.TextCaption = CreateText("Caption", view.Body);
            view.ButtonClose = CreateButton("Close Button", view.Body);
            view.TextDescription = CreateText("Description", view.Body);
            view.TextDescription.gameObject.AddComponent<LayoutElement>();

            var footer = CreateRect("Footer", view.Body);
            footer.gameObject.AddComponent<LayoutElement>();
            footer.gameObject.AddComponent<CanvasGroup>();

            view.ButtonCancel = CreateButton("Cancel Button", footer);
            view.ButtonCancel.gameObject.AddComponent<CanvasGroup>();
            view.TextCancel = CreateText("Cancel Text", view.ButtonCancel.transform);
            view.ButtonOk = CreateButton("Ok Button", footer);
            view.TextOk = CreateText("Ok Text", view.ButtonOk.transform);
            return view;
        }

        public static UiHintsView Configure(UiHintsView view)
        {
            view.ImageHintBackground = CreateImage("Hint Body", view.transform);
            view.Body = view.ImageHintBackground.rectTransform;
            view.CanvasGroup = view.Body.gameObject.AddComponent<CanvasGroup>();
            view.TextDescription = CreateText("Hint Text", view.Body);
            view.Pointer = CreateImage("Pointer", view.Body).rectTransform;
            return view;
        }

        public static UiFxView Configure(UiFxView view)
        {
            view.Body = CreateRect("Body", view.transform);
            Stretch(view.Body);
            view.FxRoot = CreateRect("FxRoot", view.Body);
            Stretch(view.FxRoot);
            view.PoolRoot = CreateRect("PoolRoot", view.Body);
            Stretch(view.PoolRoot);

            var poolGroup = view.PoolRoot.gameObject.AddComponent<CanvasGroup>();
            poolGroup.alpha = 0f;
            poolGroup.interactable = false;
            poolGroup.blocksRaycasts = false;

            view.CollectSource = CreateAnchor("CollectSource", view.Body, new Vector2(0.18f, 0.28f), Vector2.zero);
            view.CollectTarget = CreateAnchor(
                "CollectTarget",
                view.Body,
                new Vector2(0f, 1f),
                new Vector2(80f, -64f));
            view.SpendSource = CreateAnchor("SpendSource", view.Body, new Vector2(0f, 1f), new Vector2(80f, -64f));
            return view;
        }

        public static UiObjectIndicatorView Configure(UiObjectIndicatorView view)
        {
            view.Body = CreateRect("Body", view.transform);
            Stretch(view.Body);
            var itemImage = CreateImage("IndicatorItem", view.Body);
            view.ItemRoot = itemImage.rectTransform;
            view.ItemCanvasGroup = view.ItemRoot.gameObject.AddComponent<CanvasGroup>();
            view.ImageIcon = CreateImage("ImageIcon", view.ItemRoot);
            view.TextInfo = CreateText("Info Text", view.ItemRoot);
            view.Slider = CreateSlider("HealthSlider", view.ItemRoot);
            view.TextSliderValue = CreateText("Health Value Text", view.ItemRoot);
            view.ButtonAction = CreateButton("ButtonAction", view.ItemRoot);
            view.TextClickToReward = CreateText("Reward Text", view.ButtonAction.transform);
            return view;
        }

        public static UiShopView Configure(UiShopView view)
        {
            view.Body = CreateImage("Body", view.transform).rectTransform;
            view.ButtonClose = CreateButton("Close Button", view.Body);
            view.TextHeader = CreateText("Header Text", view.Body);
            view.TextItemsType = CreateText("Items Type Text", view.Body);

            var groupsPanel = CreateRect("Groups Panel", view.Body);
            view.ToggleGroup = groupsPanel.gameObject.AddComponent<ToggleGroup>();
            view.ToggleGroup.allowSwitchOff = false;
            view.GroupItemsRoot = CreateRect("Group Items", groupsPanel);
            view.GroupPoolRoot = CreatePoolRoot("Group Pool", groupsPanel);

            var scrollObject = CreateImage("Items Scroll Rect", view.Body).rectTransform;
            view.ScrollRect = scrollObject.gameObject.AddComponent<ScrollRect>();
            view.ScrollRect.horizontal = false;
            view.ScrollRect.vertical = true;
            view.ScrollRect.viewport = CreateRect("Viewport", scrollObject);
            view.ShopItemsRoot = CreateRect("Content", view.ScrollRect.viewport);
            view.ScrollRect.content = view.ShopItemsRoot;
            view.ShopItemPoolRoot = CreatePoolRoot("Shop Item Pool", scrollObject);

            view.DetailsLayout = new ShopDetailsLayout
            {
                ImageIcon = CreateImage("Details Icon", view.Body),
                TextName = CreateText("Details Name Text", view.Body),
                TextAmount = CreateText("Details Amount Text", view.Body),
                TextGroup = CreateText("Details Group Text", view.Body),
                TextDescription = CreateText("Details Description Text", view.Body)
            };

            return view;
        }

        private static RectTransform CreateAnchor(
            string name,
            Transform parent,
            Vector2 anchor,
            Vector2 anchoredPosition)
        {
            var rect = CreateRect(name, parent);
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = Vector2.zero;
            rect.anchoredPosition = anchoredPosition;
            return rect;
        }

        private static Button CreateButton(string name, Transform parent)
        {
            var image = CreateImage(name, parent);
            image.raycastTarget = true;
            var button = image.gameObject.AddComponent<Button>();
            button.targetGraphic = image;
            return button;
        }

        private static Image CreateImage(string name, Transform parent)
        {
            var gameObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            gameObject.transform.SetParent(parent, false);
            var image = gameObject.GetComponent<Image>();
            image.raycastTarget = false;
            return image;
        }

        private static Text CreateText(string name, Transform parent)
        {
            var gameObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            gameObject.transform.SetParent(parent, false);
            var text = gameObject.GetComponent<Text>();
            text.raycastTarget = false;
            return text;
        }

        private static RectTransform CreateRect(string name, Transform parent)
        {
            var gameObject = new GameObject(name, typeof(RectTransform));
            gameObject.transform.SetParent(parent, false);
            var rectTransform = (RectTransform)gameObject.transform;
            rectTransform.localScale = Vector3.one;
            return rectTransform;
        }

        private static RectTransform CreatePoolRoot(string name, Transform parent)
        {
            var root = CreateRect(name, parent);
            var canvasGroup = root.gameObject.AddComponent<CanvasGroup>();
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            return root;
        }

        private static Slider CreateSlider(string name, Transform parent)
        {
            var sliderRoot = CreateRect(name, parent);
            var fill = CreateImage("Fill", sliderRoot);
            Stretch(fill.rectTransform);

            var slider = sliderRoot.gameObject.AddComponent<Slider>();
            slider.fillRect = fill.rectTransform;
            slider.targetGraphic = fill;
            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.value = 1f;
            slider.interactable = false;
            return slider;
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

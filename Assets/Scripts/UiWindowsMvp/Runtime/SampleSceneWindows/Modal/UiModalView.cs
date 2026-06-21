using ProjectContext.UiRequests;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Windows;

namespace UiWindowsMvp.SampleSceneWindows
{
    public sealed class UiModalView : WindowComponent
    {
        public RectTransform Body;
        public CanvasGroup CanvasGroup;
        public Text TextCaption;
        public Text TextDescription;
        public Button ButtonOk;
        public Button ButtonCancel;
        public Button ButtonClose;
        public Button ButtonCloseBg;
        public Text TextOk;
        public Text TextCancel;

        private LayoutElement descriptionLayout;
        private LayoutElement footerLayout;
        private CanvasGroup footerGroup;
        private CanvasGroup cancelGroup;

        public void EnsureLayout()
        {
            if (Body != null)
            {
                EnsureRuntimeGroups();
                return;
            }

            BuildDefaultLayout();
        }

        public void Apply(UiModalRequest request)
        {
            EnsureLayout();
            if (request == null)
            {
                Clear();
                return;
            }

            SetText(TextCaption, request.Caption);
            SetText(TextDescription, request.Description);
            SetText(TextOk, "OK");
            SetText(TextCancel, "Cancel");

            var wait = request.Kind == UiModalKind.Wait;
            var okCancel = request.Kind == UiModalKind.InfoOkCancel;

            SetVisible(CanvasGroup, true);
            SetVisible(descriptionLayout, !wait);
            SetVisible(footerGroup, !wait);
            SetVisible(footerLayout, !wait);
            SetVisible(cancelGroup, okCancel);
        }

        public void Clear()
        {
            EnsureLayout();
            SetVisible(CanvasGroup, false);
        }

        private void EnsureRuntimeGroups()
        {
            CanvasGroup ??= Body.GetComponent<CanvasGroup>() ?? Body.gameObject.AddComponent<CanvasGroup>();
            if (TextDescription != null)
            {
                descriptionLayout ??= TextDescription.GetComponent<LayoutElement>();
            }

            if (ButtonOk != null)
            {
                var footer = ButtonOk.transform.parent;
                footerLayout ??= footer.GetComponent<LayoutElement>();
                footerGroup ??= footer.GetComponent<CanvasGroup>() ?? footer.gameObject.AddComponent<CanvasGroup>();
            }

            if (ButtonCancel != null)
            {
                cancelGroup ??=
                    ButtonCancel.GetComponent<CanvasGroup>() ?? ButtonCancel.gameObject.AddComponent<CanvasGroup>();
            }
        }

        private void BuildDefaultLayout()
        {
            var root = (RectTransform)transform;
            Stretch(root);

            ButtonCloseBg = CreateFullScreenButton("Close Background", transform, new Color(0f, 0f, 0f, 0.55f));

            Body = CreatePanel("Body", transform, new Color(0.08f, 0.09f, 0.11f, 0.98f));
            Body.anchorMin = new Vector2(0.5f, 0.5f);
            Body.anchorMax = new Vector2(0.5f, 0.5f);
            Body.pivot = new Vector2(0.5f, 0.5f);
            Body.anchoredPosition = Vector2.zero;
            Body.sizeDelta = new Vector2(560f, 280f);
            CanvasGroup = Body.gameObject.AddComponent<CanvasGroup>();

            var bodyLayout = Body.gameObject.AddComponent<VerticalLayoutGroup>();
            bodyLayout.padding = new RectOffset(24, 24, 20, 22);
            bodyLayout.spacing = 16f;
            bodyLayout.childControlWidth = true;
            bodyLayout.childControlHeight = true;
            bodyLayout.childForceExpandWidth = true;
            bodyLayout.childForceExpandHeight = false;

            var header = CreateRect("Header", Body);
            AddLayoutElement(header.gameObject, 44f, -1f, -1f, 1f);
            var headerLayout = header.gameObject.AddComponent<HorizontalLayoutGroup>();
            headerLayout.spacing = 12f;
            headerLayout.childAlignment = TextAnchor.MiddleCenter;
            headerLayout.childControlWidth = true;
            headerLayout.childControlHeight = true;
            headerLayout.childForceExpandWidth = false;
            headerLayout.childForceExpandHeight = true;

            TextCaption = CreateText("Modal", header, 24, TextAnchor.MiddleLeft);
            AddLayoutElement(TextCaption.gameObject, -1f, -1f, -1f, 1f);
            ButtonClose = CreateTextButton("Close Button", header, "X", 44f, 44f);

            TextDescription = CreateText("Description", Body, 18, TextAnchor.UpperLeft);
            descriptionLayout = AddLayoutElement(TextDescription.gameObject, -1f, -1f, 1f, 1f);

            var footer = CreateRect("Footer", Body);
            footerLayout = AddLayoutElement(footer.gameObject, 46f, -1f, -1f, 1f);
            footerGroup = footer.gameObject.AddComponent<CanvasGroup>();
            var footerHorizontal = footer.gameObject.AddComponent<HorizontalLayoutGroup>();
            footerHorizontal.spacing = 12f;
            footerHorizontal.childAlignment = TextAnchor.MiddleRight;
            footerHorizontal.childControlWidth = false;
            footerHorizontal.childControlHeight = true;
            footerHorizontal.childForceExpandWidth = false;
            footerHorizontal.childForceExpandHeight = true;

            ButtonCancel = CreateTextButton("Cancel Button", footer, "Cancel", 130f, 44f);
            TextCancel = ButtonCancel.GetComponentInChildren<Text>();
            cancelGroup = ButtonCancel.gameObject.AddComponent<CanvasGroup>();
            ButtonOk = CreateTextButton("Ok Button", footer, "OK", 110f, 44f);
            TextOk = ButtonOk.GetComponentInChildren<Text>();

            Clear();
        }

        private static Button CreateFullScreenButton(string name, Transform parent, Color color)
        {
            var rect = CreatePanel(name, parent, color);
            Stretch(rect);
            var button = rect.gameObject.AddComponent<Button>();
            button.targetGraphic = rect.GetComponent<Image>();
            return button;
        }

        private static Button CreateTextButton(
            string name,
            Transform parent,
            string label,
            float preferredWidth,
            float preferredHeight)
        {
            var buttonRect = CreatePanel(name, parent, new Color(0.23f, 0.28f, 0.34f, 1f));
            var button = buttonRect.gameObject.AddComponent<Button>();
            button.targetGraphic = buttonRect.GetComponent<Image>();
            AddLayoutElement(buttonRect.gameObject, preferredHeight, preferredWidth, -1f, -1f);

            var text = CreateText(label, buttonRect, 18, TextAnchor.MiddleCenter);
            Stretch((RectTransform)text.transform);
            return button;
        }

        private static RectTransform CreateRect(string name, Transform parent)
        {
            var gameObject = new GameObject(name, typeof(RectTransform));
            gameObject.transform.SetParent(parent, false);
            var rectTransform = (RectTransform)gameObject.transform;
            rectTransform.localScale = Vector3.one;
            return rectTransform;
        }

        private static RectTransform CreatePanel(string name, Transform parent, Color color)
        {
            var rect = CreateRect(name, parent);
            var image = rect.gameObject.AddComponent<Image>();
            image.color = color;
            return rect;
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

        private static LayoutElement AddLayoutElement(
            GameObject gameObject,
            float preferredHeight,
            float preferredWidth,
            float flexibleHeight,
            float flexibleWidth)
        {
            var layoutElement = gameObject.GetComponent<LayoutElement>() ?? gameObject.AddComponent<LayoutElement>();
            if (preferredHeight >= 0f)
            {
                layoutElement.preferredHeight = preferredHeight;
            }

            if (preferredWidth >= 0f)
            {
                layoutElement.preferredWidth = preferredWidth;
            }

            if (flexibleHeight >= 0f)
            {
                layoutElement.flexibleHeight = flexibleHeight;
            }

            if (flexibleWidth >= 0f)
            {
                layoutElement.flexibleWidth = flexibleWidth;
            }

            return layoutElement;
        }

        private static void SetText(Text text, string value)
        {
            if (text != null)
            {
                text.text = value ?? string.Empty;
            }
        }

        private static void SetVisible(CanvasGroup canvasGroup, bool visible)
        {
            if (canvasGroup == null)
            {
                return;
            }

            canvasGroup.alpha = visible ? 1f : 0f;
            canvasGroup.interactable = visible;
            canvasGroup.blocksRaycasts = visible;
        }

        private static void SetVisible(LayoutElement layoutElement, bool visible)
        {
            if (layoutElement != null)
            {
                layoutElement.ignoreLayout = !visible;
            }
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
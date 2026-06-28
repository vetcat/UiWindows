using DG.Tweening;
using ProjectContext.UiRequests;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Windows;

namespace UiWindowsMvp.SampleSceneWindows
{
    public sealed class UiHintsView : WindowComponent
    {
        private const float FadeInDuration = 0.15f;
        private const float FadeOutDuration = 0.2f;

        public RectTransform Body;
        public CanvasGroup CanvasGroup;
        public Image ImageHintBackground;
        public Text TextDescription;
        public RectTransform Pointer;

        private Sequence activeSequence;

        public string LastDescription => TextDescription != null ? TextDescription.text : string.Empty;
        public float CurrentAlpha => CanvasGroup != null ? CanvasGroup.alpha : 0f;

        public void EnsureLayout()
        {
            if (Body != null)
            {
                CanvasGroup ??= Body.GetComponent<CanvasGroup>() ?? Body.gameObject.AddComponent<CanvasGroup>();
                return;
            }

            BuildDefaultLayout();
        }

        public void ShowHint(UiHintRequest request)
        {
            EnsureLayout();
            ClearHint();

            SetText(TextDescription, request.Description);
            PlaceBody(request.Anchor);
            CanvasGroup.alpha = 0f;
            CanvasGroup.interactable = false;
            CanvasGroup.blocksRaycasts = false;
            Body.localScale = Vector3.one * 0.96f;

            activeSequence = DOTween.Sequence();
            activeSequence.Append(CanvasGroup.DOFade(1f, FadeInDuration));
            activeSequence.Join(Body.DOScale(1f, FadeInDuration).SetEase(Ease.OutBack));
            activeSequence.AppendInterval(request.DurationSeconds);
            activeSequence.Append(CanvasGroup.DOFade(0f, FadeOutDuration));
            activeSequence.OnKill(() => activeSequence = null);
            activeSequence.OnComplete(() => activeSequence = null);
        }

        public void ClearHint()
        {
            if (activeSequence != null)
            {
                activeSequence.Kill();
                activeSequence = null;
            }

            if (CanvasGroup != null)
            {
                CanvasGroup.alpha = 0f;
                CanvasGroup.interactable = false;
                CanvasGroup.blocksRaycasts = false;
            }
        }

        private void BuildDefaultLayout()
        {
            var root = (RectTransform)transform;
            Stretch(root);

            Body = CreatePanel("Hint Body", transform, new Color(0.06f, 0.07f, 0.09f, 0.94f));
            Body.anchorMin = new Vector2(0.5f, 0.5f);
            Body.anchorMax = new Vector2(0.5f, 0.5f);
            Body.pivot = new Vector2(0.5f, 0.5f);
            Body.sizeDelta = new Vector2(420f, 88f);
            Body.anchoredPosition = Vector2.zero;
            CanvasGroup = Body.gameObject.AddComponent<CanvasGroup>();

            ImageHintBackground = Body.GetComponent<Image>();

            TextDescription = CreateText("Hint", Body, 18, TextAnchor.MiddleCenter);
            Stretch((RectTransform)TextDescription.transform);
            ((RectTransform)TextDescription.transform).offsetMin = new Vector2(22f, 12f);
            ((RectTransform)TextDescription.transform).offsetMax = new Vector2(-22f, -12f);

            Pointer = CreatePanel("Pointer", Body, new Color(0.21f, 0.56f, 0.9f, 1f));
            Pointer.anchorMin = new Vector2(0.5f, 0f);
            Pointer.anchorMax = new Vector2(0.5f, 0f);
            Pointer.pivot = new Vector2(0.5f, 1f);
            Pointer.sizeDelta = new Vector2(18f, 18f);
            Pointer.anchoredPosition = new Vector2(0f, -6f);
            Pointer.localEulerAngles = new Vector3(0f, 0f, 45f);

            ClearHint();
        }

        private void PlaceBody(UiHintAnchor anchor)
        {
            switch (anchor)
            {
                case UiHintAnchor.Top:
                    Body.anchoredPosition = new Vector2(0f, 210f);
                    break;
                case UiHintAnchor.Bottom:
                    Body.anchoredPosition = new Vector2(0f, -210f);
                    break;
                case UiHintAnchor.Left:
                    Body.anchoredPosition = new Vector2(-310f, 0f);
                    break;
                case UiHintAnchor.Right:
                    Body.anchoredPosition = new Vector2(310f, 0f);
                    break;
                default:
                    Body.anchoredPosition = Vector2.zero;
                    break;
            }
        }

        private static RectTransform CreatePanel(string name, Transform parent, Color color)
        {
            var gameObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            gameObject.transform.SetParent(parent, false);
            var image = gameObject.GetComponent<Image>();
            image.color = color;
            image.raycastTarget = false;
            return (RectTransform)gameObject.transform;
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

        private static void SetText(Text text, string value)
        {
            if (text != null)
            {
                text.text = value ?? string.Empty;
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

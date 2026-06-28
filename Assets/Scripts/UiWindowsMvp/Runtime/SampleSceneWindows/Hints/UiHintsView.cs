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
            if (Body != null &&
                CanvasGroup != null &&
                ImageHintBackground != null &&
                TextDescription != null &&
                Pointer != null)
            {
                return;
            }

            throw UiPrefabReferenceGuard.Missing(
                this,
                "Body, CanvasGroup, ImageHintBackground, TextDescription, Pointer");
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

        private static void SetText(Text text, string value)
        {
            if (text != null)
            {
                text.text = value ?? string.Empty;
            }
        }
    }
}

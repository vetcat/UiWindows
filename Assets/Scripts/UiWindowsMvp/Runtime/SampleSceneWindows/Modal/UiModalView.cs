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
            if (Body != null &&
                CanvasGroup != null &&
                TextCaption != null &&
                TextDescription != null &&
                ButtonOk != null &&
                ButtonCancel != null &&
                ButtonClose != null &&
                ButtonCloseBg != null &&
                TextOk != null &&
                TextCancel != null)
            {
                EnsureRuntimeGroups();
                return;
            }

            throw UiPrefabReferenceGuard.Missing(
                this,
                "Body, CanvasGroup, TextCaption, TextDescription, ButtonOk, ButtonCancel, ButtonClose, " +
                "ButtonCloseBg, TextOk, TextCancel");
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
            CanvasGroup ??= Body.GetComponent<CanvasGroup>();
            if (CanvasGroup == null)
            {
                throw UiPrefabReferenceGuard.MissingComponent(this, nameof(CanvasGroup), "Body");
            }

            if (TextDescription != null)
            {
                descriptionLayout ??= TextDescription.GetComponent<LayoutElement>();
                if (descriptionLayout == null)
                {
                    throw UiPrefabReferenceGuard.MissingComponent(
                        this,
                        nameof(LayoutElement),
                        "TextDescription");
                }
            }

            if (ButtonOk != null)
            {
                var footer = ButtonOk.transform.parent;
                if (footer == null)
                {
                    throw UiPrefabReferenceGuard.Missing(this, "ButtonOk parent Footer");
                }

                footerLayout ??= footer.GetComponent<LayoutElement>();
                if (footerLayout == null)
                {
                    throw UiPrefabReferenceGuard.MissingComponent(this, nameof(LayoutElement), "Footer");
                }

                footerGroup ??= footer.GetComponent<CanvasGroup>();
                if (footerGroup == null)
                {
                    throw UiPrefabReferenceGuard.MissingComponent(this, nameof(CanvasGroup), "Footer");
                }
            }

            if (ButtonCancel != null)
            {
                cancelGroup ??= ButtonCancel.GetComponent<CanvasGroup>();
                if (cancelGroup == null)
                {
                    throw UiPrefabReferenceGuard.MissingComponent(this, nameof(CanvasGroup), "ButtonCancel");
                }
            }
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
    }
}

using UnityEngine;
using UnityEngine.UI;

namespace UiWindowsMvp.SampleSceneWindows
{
    public sealed class UiFxItemView : MonoBehaviour
    {
        public RectTransform Body;
        public CanvasGroup CanvasGroup;
        public Image ImageIcon;
        public Text TextAmount;

        public RectTransform RectTransform => (RectTransform)transform;

        public void EnsureLayout()
        {
            Body ??= RectTransform;
            CanvasGroup ??= gameObject.GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
        }

        public void Prepare(string amountText, Color amountColor, Color iconColor, Vector2 anchoredPosition)
        {
            EnsureLayout();
            RectTransform.anchoredPosition = anchoredPosition;
            RectTransform.localScale = Vector3.one;
            Body.localScale = Vector3.one;
            CanvasGroup.alpha = 0f;
            CanvasGroup.interactable = false;
            CanvasGroup.blocksRaycasts = false;

            if (ImageIcon != null)
            {
                ImageIcon.color = iconColor;
            }

            if (TextAmount != null)
            {
                TextAmount.text = amountText;
                TextAmount.color = amountColor;
            }
        }

        public void Release()
        {
            EnsureLayout();
            CanvasGroup.alpha = 0f;
            CanvasGroup.interactable = false;
            CanvasGroup.blocksRaycasts = false;
            RectTransform.localScale = Vector3.one;
            Body.localScale = Vector3.one;
        }
    }
}
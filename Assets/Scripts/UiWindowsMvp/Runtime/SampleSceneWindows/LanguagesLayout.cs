using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UiWindowsMvp.SampleSceneWindows
{
    [Serializable]
    public sealed class LanguagesLayout
    {
        public CanvasGroup CanvasGroup;
        public LayoutElement LayoutElement;
        public RectTransform ItemsRoot;
        public ToggleGroup ToggleGroup;
        public List<LanguageItem> Items = new();

        public void SetVisible(bool visible)
        {
            if (CanvasGroup != null)
            {
                CanvasGroup.alpha = visible ? 1f : 0f;
                CanvasGroup.interactable = visible;
                CanvasGroup.blocksRaycasts = visible;
            }

            if (LayoutElement != null)
            {
                LayoutElement.ignoreLayout = !visible;
            }
        }
    }
}
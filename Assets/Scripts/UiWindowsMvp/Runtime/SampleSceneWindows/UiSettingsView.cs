using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Windows;

namespace UiWindowsMvp.SampleSceneWindows
{
    public sealed class UiSettingsView : WindowComponent
    {
        public RectTransform Body;
        public Button ButtonClose;
        public Text TextHeader;
        public Toggle ToggleSettings;
        public Toggle ToggleLanguage;
        public Text TextToggleSettings;
        public Text TextToggleLanguage;
        public SettingsLayout SettingsLayout;
        public LanguagesLayout LanguagesLayout;

        public void SelectTab(UiSettingsTab tab)
        {
            var settingsSelected = tab == UiSettingsTab.Settings;
            SetToggleWithoutNotify(ToggleSettings, settingsSelected);
            SetToggleWithoutNotify(ToggleLanguage, !settingsSelected);
            SettingsLayout?.SetVisible(settingsSelected);
            LanguagesLayout?.SetVisible(!settingsSelected);
        }

        public IReadOnlyList<LanguageItem> RebuildLanguageItems(IReadOnlyList<SystemLanguage> languages)
        {
            if (LanguagesLayout == null || LanguagesLayout.ItemsRoot == null)
            {
                return System.Array.Empty<LanguageItem>();
            }

            ClearLanguageItems();
            if (languages == null)
            {
                return LanguagesLayout.Items;
            }

            for (var i = 0; i < languages.Count; i++)
            {
                LanguagesLayout.Items.Add(CreateLanguageItem(languages[i], LanguagesLayout.ItemsRoot,
                    LanguagesLayout.ToggleGroup));
            }

            return LanguagesLayout.Items;
        }

        public void ClearLanguageItems()
        {
            if (LanguagesLayout?.Items == null)
            {
                return;
            }

            for (var i = 0; i < LanguagesLayout.Items.Count; i++)
            {
                var item = LanguagesLayout.Items[i];
                if (item == null)
                {
                    continue;
                }

                DestroyObject(item.gameObject);
            }

            LanguagesLayout.Items.Clear();
        }

        private static LanguageItem CreateLanguageItem(
            SystemLanguage language,
            RectTransform parent,
            ToggleGroup toggleGroup)
        {
            var itemObject = new GameObject(
                language + " Language Item",
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(Image),
                typeof(Toggle),
                typeof(LayoutElement),
                typeof(LanguageItem));
            itemObject.transform.SetParent(parent, false);

            var itemRect = (RectTransform)itemObject.transform;
            itemRect.localScale = Vector3.one;
            itemRect.anchorMin = new Vector2(0f, 1f);
            itemRect.anchorMax = new Vector2(1f, 1f);
            itemRect.pivot = new Vector2(0.5f, 1f);
            itemRect.sizeDelta = new Vector2(0f, 42f);

            var background = itemObject.GetComponent<Image>();
            background.color = new Color(0.14f, 0.17f, 0.2f, 0.9f);

            var layoutElement = itemObject.GetComponent<LayoutElement>();
            layoutElement.preferredHeight = 42f;
            layoutElement.flexibleWidth = 1f;

            var checkmark = CreateImage("Checkmark", itemObject.transform, new Color(0.34f, 0.78f, 0.53f, 1f));
            var checkmarkRect = (RectTransform)checkmark.transform;
            checkmarkRect.anchorMin = new Vector2(0f, 0.5f);
            checkmarkRect.anchorMax = new Vector2(0f, 0.5f);
            checkmarkRect.pivot = new Vector2(0.5f, 0.5f);
            checkmarkRect.anchoredPosition = new Vector2(22f, 0f);
            checkmarkRect.sizeDelta = new Vector2(16f, 16f);

            var text = CreateText(language.ToString(), itemObject.transform);
            var textRect = (RectTransform)text.transform;
            textRect.anchorMin = new Vector2(0f, 0f);
            textRect.anchorMax = new Vector2(1f, 1f);
            textRect.offsetMin = new Vector2(52f, 0f);
            textRect.offsetMax = new Vector2(-12f, 0f);
            text.alignment = TextAnchor.MiddleLeft;
            text.fontSize = 18;

            var toggle = itemObject.GetComponent<Toggle>();
            toggle.group = toggleGroup;
            toggle.targetGraphic = background;
            toggle.graphic = checkmark;

            var item = itemObject.GetComponent<LanguageItem>();
            item.Toggle = toggle;
            item.TextLanguage = text;
            item.Language = language;
            return item;
        }

        private static Image CreateImage(string name, Transform parent, Color color)
        {
            var gameObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            gameObject.transform.SetParent(parent, false);
            var image = gameObject.GetComponent<Image>();
            image.color = color;
            return image;
        }

        private static Text CreateText(string value, Transform parent)
        {
            var gameObject = new GameObject(value + " Text", typeof(RectTransform), typeof(CanvasRenderer),
                typeof(Text));
            gameObject.transform.SetParent(parent, false);
            var text = gameObject.GetComponent<Text>();
            text.text = value;
            text.color = Color.white;
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            return text;
        }

        private static void SetToggleWithoutNotify(Toggle toggle, bool value)
        {
            if (toggle != null)
            {
                toggle.SetIsOnWithoutNotify(value);
            }
        }

        private static void DestroyObject(Object target)
        {
            if (target == null)
            {
                return;
            }

            if (Application.isPlaying)
            {
                Destroy(target);
                return;
            }

            DestroyImmediate(target);
        }
    }
}
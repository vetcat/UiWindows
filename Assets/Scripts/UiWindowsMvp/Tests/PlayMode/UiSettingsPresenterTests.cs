using NUnit.Framework;
using ProjectContext.Localization;
using ProjectContext.Settings;
using UiWindowsMvp.SampleSceneWindows;
using UiWindowsMvp.UIAdapter;
using UnityEngine;
using UnityEngine.UI;

namespace UiWindowsMvp.Tests.PlayMode
{
    public sealed class UiSettingsPresenterTests
    {
        [Test]
        public void Presenter_BindsSettingsAndRefreshesLocalizedLabels()
        {
            var viewObject = new GameObject("UiSettings Test View", typeof(RectTransform), typeof(UiSettingsView));
            var windowObject =
                new GameObject("UiSettings Test Window", typeof(RectTransform), typeof(UiSettingsWindow));
            using var settings = new GameSettingsService(0.25f, 0.75f);
            using var localization = new LocalizationService(SystemLanguage.English);
            using var showScope = new WindowPresenterShowScope();

            try
            {
                var view = BuildView(viewObject);
                var window = windowObject.GetComponent<UiSettingsWindow>();
                var presenter = new UiSettingsPresenter(settings, settings, localization, localization, _ => view);

                presenter.Bind(window);
                presenter.Initialize();
                presenter.OnShowBegin(showScope);

                Assert.That(view.TextHeader.text, Is.EqualTo("Settings"));
                Assert.That(view.TextToggleSettings.text, Is.EqualTo("Settings"));
                Assert.That(view.TextToggleLanguage.text, Is.EqualTo("Language"));
                Assert.That(view.SettingsLayout.TextMusic.text, Is.EqualTo("MUSUC"));
                Assert.That(view.SettingsLayout.TextSliderMusicValue.text, Is.EqualTo("25"));
                Assert.That(view.SettingsLayout.TextSliderSoundValue.text, Is.EqualTo("75"));
                Assert.That(view.SettingsLayout.SliderMusicVolume.value, Is.EqualTo(0.25f));

                view.SettingsLayout.SliderMusicVolume.value = 0.4f;

                Assert.That(GetCurrentValue<float>(settings, "MusicVolume"), Is.EqualTo(0.4f).Within(0.0001f));
                Assert.That(view.SettingsLayout.TextSliderMusicValue.text, Is.EqualTo("40"));

                localization.ChangeLanguage(SystemLanguage.German);

                Assert.That(view.TextHeader.text, Is.EqualTo("die Einstellungen"));
                Assert.That(view.TextToggleLanguage.text, Is.EqualTo("Sprache"));
                Assert.That(view.SettingsLayout.TextMusic.text, Is.EqualTo("MUSIK"));

                view.ToggleLanguage.isOn = true;

                Assert.That(view.TextHeader.text, Is.EqualTo("Sprache"));
            }
            finally
            {
                Object.DestroyImmediate(windowObject);
                Object.DestroyImmediate(viewObject);
            }
        }

        [Test]
        public void Presenter_DisposesShowScopeHandlersAndReadModelSubscriptions()
        {
            var viewObject = new GameObject("UiSettings Test View", typeof(RectTransform), typeof(UiSettingsView));
            var windowObject =
                new GameObject("UiSettings Test Window", typeof(RectTransform), typeof(UiSettingsWindow));
            using var settings = new GameSettingsService(0.25f, 0.75f);
            using var localization = new LocalizationService(SystemLanguage.English);
            var showScope = new WindowPresenterShowScope();

            try
            {
                var view = BuildView(viewObject);
                var window = windowObject.GetComponent<UiSettingsWindow>();
                var presenter = new UiSettingsPresenter(settings, settings, localization, localization, _ => view);

                presenter.Bind(window);
                presenter.Initialize();
                presenter.OnShowBegin(showScope);
                showScope.Dispose();

                view.SettingsLayout.SliderMusicVolume.value = 0.5f;
                var germanItem = FindLanguageItem(view, SystemLanguage.German);
                germanItem.Toggle.isOn = true;
                settings.SetMusicVolume(0.9f);
                localization.ChangeLanguage(SystemLanguage.French);

                Assert.That(GetCurrentValue<float>(settings, "MusicVolume"), Is.EqualTo(0.9f));
                Assert.That(GetCurrentValue<SystemLanguage>(localization, "CurrentLanguage"),
                    Is.EqualTo(SystemLanguage.French));
                Assert.That(view.SettingsLayout.TextSliderMusicValue.text, Is.EqualTo("25"));
                Assert.That(view.TextHeader.text, Is.EqualTo("Settings"));
            }
            finally
            {
                showScope.Dispose();
                Object.DestroyImmediate(windowObject);
                Object.DestroyImmediate(viewObject);
            }
        }

        internal static UiSettingsView BuildView(GameObject viewObject)
        {
            var view = viewObject.GetComponent<UiSettingsView>();
            view.Body = (RectTransform)viewObject.transform;
            view.ButtonClose = CreateComponent<Button>("Close Button", viewObject.transform);
            view.TextHeader = CreateText("Header", viewObject.transform);
            view.ToggleSettings = CreateComponent<Toggle>("Settings Toggle", viewObject.transform);
            view.ToggleLanguage = CreateComponent<Toggle>("Language Toggle", viewObject.transform);
            view.TextToggleSettings = CreateText("Settings Toggle Text", viewObject.transform);
            view.TextToggleLanguage = CreateText("Language Toggle Text", viewObject.transform);

            view.SettingsLayout = new SettingsLayout
            {
                CanvasGroup = CreateComponent<CanvasGroup>("Settings Group", viewObject.transform),
                LayoutElement = CreateComponent<LayoutElement>("Settings Layout Element", viewObject.transform),
                SliderMusicVolume = CreateComponent<Slider>("Music Slider", viewObject.transform),
                SliderSoundVolume = CreateComponent<Slider>("Sound Slider", viewObject.transform),
                TextMusic = CreateText("Music Text", viewObject.transform),
                TextSound = CreateText("Sound Text", viewObject.transform),
                TextSliderMusicValue = CreateText("Music Value Text", viewObject.transform),
                TextSliderSoundValue = CreateText("Sound Value Text", viewObject.transform)
            };

            view.LanguagesLayout = new LanguagesLayout
            {
                CanvasGroup = CreateComponent<CanvasGroup>("Language Group", viewObject.transform),
                LayoutElement = CreateComponent<LayoutElement>("Language Layout Element", viewObject.transform),
                ItemsRoot = CreateComponent<RectTransform>("Language Items", viewObject.transform),
                ToggleGroup = CreateComponent<ToggleGroup>("Language Toggle Group", viewObject.transform)
            };

            return view;
        }

        internal static LanguageItem FindLanguageItem(UiSettingsView view, SystemLanguage language)
        {
            for (var i = 0; i < view.LanguagesLayout.Items.Count; i++)
            {
                var item = view.LanguagesLayout.Items[i];
                if (item != null && item.Language == language)
                {
                    return item;
                }
            }

            Assert.Fail($"Language item for {language} was not created.");
            return null;
        }

        internal static T GetCurrentValue<T>(object owner, string propertyName)
        {
            var property = owner.GetType().GetProperty(
                propertyName,
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            Assert.That(property, Is.Not.Null);
            var reactiveSurface = property.GetValue(owner);
            var currentValueProperty =
                reactiveSurface.GetType().GetProperty(
                    "CurrentValue",
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            Assert.That(currentValueProperty, Is.Not.Null);
            return (T)currentValueProperty.GetValue(reactiveSurface);
        }

        private static Text CreateText(string name, Transform parent)
        {
            return CreateComponent<Text>(name, parent);
        }

        private static T CreateComponent<T>(string name, Transform parent)
            where T : Component
        {
            var gameObject = typeof(T) == typeof(RectTransform)
                ? new GameObject(name, typeof(RectTransform))
                : new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(T));
            gameObject.transform.SetParent(parent, false);
            return gameObject.GetComponent<T>();
        }
    }
}
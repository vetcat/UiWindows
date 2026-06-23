using System.Collections;
using CompositionRoot.Runtime;
using NUnit.Framework;
using ProjectContext.Localization;
using ProjectContext.Settings;
using UiWindowsMvp.SampleSceneWindows;
using UiWindowsMvp.UIAdapter;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI.Windows;

namespace UiWindowsMvp.Tests.PlayMode
{
    public sealed class UiSettingsWindowLifecycleTests
    {
        [UnityTest]
        public IEnumerator
            ReopenCyclesThroughWindowSystem_ReusePooledWindowWithoutDuplicateSettingsOrLocalizationHandlers()
        {
            PlayerPrefs.DeleteKey(GameSettingsService.MusicVolumeKey);
            PlayerPrefs.DeleteKey(GameSettingsService.SoundVolumeKey);
            PlayerPrefs.DeleteKey(LocalizationService.LanguageKey);
            DestroyWindowSystemsImmediate();
            yield return null;

            SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
            yield return null;

            var root = Object.FindFirstObjectByType<SceneCompositionRoot>();
            Assert.That(root, Is.Not.Null);
            Assert.That(root.IsBootstrapped, Is.True);

            var launcher = root.Services.Resolve<UiSettingsDemoLauncher>();
            var uiTopLeftLauncher = root.Services.Resolve<UiTopLeftDemoLauncher>();
            var settings = root.Services.Resolve<IGameSettingsService>();
            var localization = root.Services.Resolve<ILocalizationService>();
            var presenterFactory = root.Services.Resolve<UiSettingsPresenterFactory>();
            UiSettingsWindow window = null;
            IWindowPresenterBinding<UiSettingsWindow> binding = null;
            var cleaned = false;

            try
            {
                yield return WaitUntil(
                    () => uiTopLeftLauncher.CurrentWindow != null &&
                          uiTopLeftLauncher.CurrentWindow.GetState() == ObjectState.Shown,
                    "initial UiTopLeft window show before settings isolation");
                var uiTopLeftWindow = uiTopLeftLauncher.CurrentWindow;
                uiTopLeftLauncher.Hide();
                yield return WaitUntil(() => uiTopLeftWindow.GetState() == ObjectState.Hidden,
                    "hide initial UiTopLeft before settings isolation");
                WindowSystem.Clean(uiTopLeftWindow);

                launcher.Show();
                yield return WaitUntil(
                    () => launcher.CurrentWindow != null && launcher.CurrentWindow.GetState() == ObjectState.Shown,
                    "initial UiSettings window show");

                window = launcher.CurrentWindow;
                Assert.That(window.createPool, Is.True);
                Assert.That(WindowPresenterBinder.TryGetBinding(window, out binding), Is.True);
                Assert.That(presenterFactory.CreatedCount, Is.EqualTo(1));
                Assert.That(window.TryGetView(out var view), Is.True);

                var initialInstanceId = window.GetInstanceID();
                var initialView = view;

                for (var cycle = 0; cycle < 3; cycle++)
                {
                    settings.SetMusicVolume(0.2f);
                    localization.ChangeLanguage(SystemLanguage.English);
                    yield return null;

                    Assert.That(window.TryGetView(out view), Is.True);
                    Assert.That(view, Is.SameAs(initialView));
                    Assert.That(view.SettingsLayout.TextSliderMusicValue.text, Is.EqualTo("20"));

                    view.SettingsLayout.SliderMusicVolume.value = 0.33f;
                    Assert.That(
                        UiSettingsPresenterTests.GetCurrentValue<float>(settings, "MusicVolume"),
                        Is.EqualTo(0.33f).Within(0.0001f));
                    Assert.That(view.SettingsLayout.TextSliderMusicValue.text, Is.EqualTo("33"));

                    view.ToggleLanguage.isOn = true;
                    var frenchItem = UiSettingsPresenterTests.FindLanguageItem(view, SystemLanguage.French);
                    frenchItem.Toggle.isOn = true;
                    Assert.That(
                        UiSettingsPresenterTests.GetCurrentValue<SystemLanguage>(localization, "CurrentLanguage"),
                        Is.EqualTo(SystemLanguage.French));
                    Assert.That(view.TextHeader.text, Is.EqualTo("Langue"));

                    launcher.Hide();
                    yield return WaitUntil(() => window.GetState() == ObjectState.Hidden, $"hide cycle {cycle}");

                    settings.SetMusicVolume(0.77f);
                    localization.ChangeLanguage(SystemLanguage.German);
                    Assert.That(view.SettingsLayout.TextSliderMusicValue.text, Is.EqualTo("33"));
                    Assert.That(view.TextHeader.text, Is.EqualTo("Langue"));

                    view.SettingsLayout.SliderMusicVolume.value = 0.12f;
                    Assert.That(
                        UiSettingsPresenterTests.GetCurrentValue<float>(settings, "MusicVolume"),
                        Is.EqualTo(0.77f).Within(0.0001f));

                    var englishItem = UiSettingsPresenterTests.FindLanguageItem(view, SystemLanguage.English);
                    englishItem.Toggle.isOn = true;
                    Assert.That(
                        UiSettingsPresenterTests.GetCurrentValue<SystemLanguage>(localization, "CurrentLanguage"),
                        Is.EqualTo(SystemLanguage.German));

                    launcher.Show();
                    yield return WaitUntil(
                        () => launcher.CurrentWindow != null && launcher.CurrentWindow.GetState() == ObjectState.Shown,
                        $"reopen cycle {cycle}");

                    Assert.That(launcher.CurrentWindow.GetInstanceID(), Is.EqualTo(initialInstanceId));
                    Assert.That(WindowPresenterBinder.TryGetBinding(launcher.CurrentWindow, out var reopenedBinding),
                        Is.True);
                    Assert.That(reopenedBinding, Is.SameAs(binding));
                    Assert.That(presenterFactory.CreatedCount, Is.EqualTo(1));
                    Assert.That(view.SettingsLayout.TextSliderMusicValue.text, Is.EqualTo("77"));
                    Assert.That(view.TextHeader.text, Is.EqualTo("Sprache"));
                }

                launcher.Hide();
                yield return WaitUntil(() => window.GetState() == ObjectState.Hidden, "final hide before clean");

                WindowSystem.Clean(window);
                cleaned = true;

                Assert.That(binding.IsDisposed, Is.True);
                Assert.DoesNotThrow(() => binding.Dispose());
            }
            finally
            {
                launcher?.Dispose();
                uiTopLeftLauncher?.Dispose();

                if (!cleaned && window != null && window.GetState() == ObjectState.Hidden)
                {
                    WindowSystem.Clean(window);
                }

                PlayerPrefs.DeleteKey(GameSettingsService.MusicVolumeKey);
                PlayerPrefs.DeleteKey(GameSettingsService.SoundVolumeKey);
                PlayerPrefs.DeleteKey(LocalizationService.LanguageKey);
                DestroyWindowSystemsImmediate();
            }
        }

        private static IEnumerator WaitUntil(System.Func<bool> predicate, string description)
        {
            const int MaxFrames = 180;
            for (var frame = 0; frame < MaxFrames; frame++)
            {
                if (predicate())
                {
                    yield break;
                }

                yield return null;
            }

            Assert.Fail($"Timed out waiting for {description}.");
        }

        private static void DestroyWindowSystemsImmediate()
        {
            DestroyObjectsImmediate<UiDownLeftWindow>();
            DestroyObjectsImmediate<UiDownRightWindow>();
            DestroyObjectsImmediate<UiSettingsWindow>();
            DestroyObjectsImmediate<UiTopLeftWindow>();
            DestroyObjectsImmediate<WindowLayout>();
            DestroyObjectsImmediate<WindowSystem>();
        }

        private static void DestroyObjectsImmediate<T>()
            where T : Component
        {
            var objects = Object.FindObjectsByType<T>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            for (var i = 0; i < objects.Length; i++)
            {
                if (objects[i] != null)
                {
                    Object.DestroyImmediate(objects[i].gameObject);
                }
            }
        }
    }
}

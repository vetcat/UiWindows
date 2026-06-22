using System.Collections;
using CompositionRoot.Runtime;
using NUnit.Framework;
using ProjectContext.Localization;
using UiWindowsMvp.SampleSceneWindows;
using UiWindowsMvp.UIAdapter;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI.Windows;

namespace UiWindowsMvp.Tests.PlayMode
{
    public sealed class UiDownRightWindowLifecycleTests
    {
        [UnityTest]
        public IEnumerator ReopenCyclesThroughWindowSystem_OpenSettingsWithoutDuplicateHandlers()
        {
            PlayerPrefs.DeleteKey(LocalizationService.LanguageKey);
            DestroyWindowSystemsImmediate();
            yield return null;

            SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
            yield return null;

            var root = Object.FindFirstObjectByType<SceneCompositionRoot>();
            Assert.That(root, Is.Not.Null);
            Assert.That(root.IsBootstrapped, Is.True);

            var launcher = root.Services.Resolve<UiDownRightDemoLauncher>();
            var settingsLauncher = root.Services.Resolve<UiSettingsDemoLauncher>();
            var localization = root.Services.Resolve<ILocalizationService>();
            var presenterFactory = root.Services.Resolve<UiDownRightPresenterFactory>();
            var settingsPresenterFactory = root.Services.Resolve<UiSettingsPresenterFactory>();
            UiDownRightWindow window = null;
            UiSettingsWindow settingsWindow = null;
            IWindowPresenterBinding<UiDownRightWindow> binding = null;
            var cleaned = false;

            try
            {
                yield return WaitUntil(
                    () => launcher.CurrentWindow != null && launcher.CurrentWindow.GetState() == ObjectState.Shown,
                    "initial UiDownRight window show");

                localization.ChangeLanguage(SystemLanguage.English);
                yield return null;

                window = launcher.CurrentWindow;
                Assert.That(window.createPool, Is.True);
                Assert.That(WindowPresenterBinder.TryGetBinding(window, out binding), Is.True);
                Assert.That(presenterFactory.CreatedCount, Is.EqualTo(1));
                Assert.That(window.TryGetView(out var view), Is.True);
                Assert.That(view.TextSettings.text, Is.EqualTo("Settings"));

                var initialInstanceId = window.GetInstanceID();
                var initialView = view;

                view.ButtonSettings.onClick.Invoke();
                yield return WaitUntil(
                    () => settingsLauncher.CurrentWindow != null &&
                          settingsLauncher.CurrentWindow.GetState() == ObjectState.Shown,
                    "settings window opened from UiDownRight");

                settingsWindow = settingsLauncher.CurrentWindow;
                Assert.That(settingsPresenterFactory.CreatedCount, Is.EqualTo(1));
                settingsLauncher.Hide();
                yield return WaitUntil(() => settingsWindow.GetState() == ObjectState.Hidden,
                    "hide settings after initial open");

                for (var cycle = 0; cycle < 3; cycle++)
                {
                    localization.ChangeLanguage(SystemLanguage.English);
                    yield return null;

                    Assert.That(window.TryGetView(out view), Is.True);
                    Assert.That(view, Is.SameAs(initialView));
                    Assert.That(view.TextSettings.text, Is.EqualTo("Settings"));

                    view.ButtonSettings.onClick.Invoke();
                    yield return WaitUntil(() => settingsWindow.GetState() == ObjectState.Shown,
                        $"settings open cycle {cycle}");
                    Assert.That(settingsPresenterFactory.CreatedCount, Is.EqualTo(1));

                    settingsLauncher.Hide();
                    yield return WaitUntil(() => settingsWindow.GetState() == ObjectState.Hidden,
                        $"settings hide cycle {cycle}");

                    launcher.Hide();
                    yield return WaitUntil(() => window.GetState() == ObjectState.Hidden, $"hide cycle {cycle}");

                    localization.ChangeLanguage(SystemLanguage.German);
                    Assert.That(view.TextSettings.text, Is.EqualTo("Settings"));

                    view.ButtonSettings.onClick.Invoke();
                    yield return null;
                    Assert.That(settingsWindow.GetState(), Is.EqualTo(ObjectState.Hidden));

                    launcher.Show();
                    yield return WaitUntil(
                        () => launcher.CurrentWindow != null && launcher.CurrentWindow.GetState() == ObjectState.Shown,
                        $"reopen cycle {cycle}");

                    Assert.That(launcher.CurrentWindow.GetInstanceID(), Is.EqualTo(initialInstanceId));
                    Assert.That(WindowPresenterBinder.TryGetBinding(launcher.CurrentWindow, out var reopenedBinding),
                        Is.True);
                    Assert.That(reopenedBinding, Is.SameAs(binding));
                    Assert.That(presenterFactory.CreatedCount, Is.EqualTo(1));

                    Assert.That(launcher.CurrentWindow.TryGetView(out view), Is.True);
                    Assert.That(view.TextSettings.text, Is.EqualTo("die Einstellungen"));
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
                settingsLauncher?.Dispose();
                launcher?.Dispose();

                if (settingsWindow != null && settingsWindow.GetState() == ObjectState.Hidden)
                {
                    WindowSystem.Clean(settingsWindow);
                }

                if (!cleaned && window != null && window.GetState() == ObjectState.Hidden)
                {
                    WindowSystem.Clean(window);
                }

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
            DestroyObjectsImmediate<UiFxWindow>();
            DestroyObjectsImmediate<UiHintsWindow>();
            DestroyObjectsImmediate<UiDownRightWindow>();
            DestroyObjectsImmediate<UiTopRightWindow>();
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

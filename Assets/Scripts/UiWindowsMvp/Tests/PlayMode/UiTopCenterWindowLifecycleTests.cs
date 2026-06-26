using System.Collections;
using CompositionRoot.Runtime;
using NUnit.Framework;
using ProjectContext.Localization;
using ProjectContext.UiRequests;
using UiWindowsMvp.SampleSceneWindows;
using UiWindowsMvp.UIAdapter;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI.Windows;

namespace UiWindowsMvp.Tests.PlayMode
{
    public sealed class UiTopCenterWindowLifecycleTests
    {
        [UnityTest]
        public IEnumerator TopCenter_RunThroughWindowSystem_RefreshesAndCleansShowScopedHoldHandlers()
        {
            DestroyWindowSystemsImmediate();
            yield return null;

            SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
            yield return null;

            var root = Object.FindFirstObjectByType<SceneCompositionRoot>();
            Assert.That(root, Is.Not.Null);
            Assert.That(root.IsBootstrapped, Is.True);

            var launcher = root.Services.Resolve<UiTopCenterDemoLauncher>();
            var hintsLauncher = root.Services.Resolve<UiHintsDemoLauncher>();
            var localization = root.Services.Resolve<ILocalizationCommands>();
            var presenterFactory = root.Services.Resolve<UiTopCenterPresenterFactory>();
            UiTopCenterWindow window = null;
            IWindowPresenterBinding<UiTopCenterWindow> binding = null;
            var cleaned = false;

            try
            {
                yield return WaitUntil(
                    () => launcher.CurrentWindow != null && launcher.CurrentWindow.GetState() == ObjectState.Shown,
                    "initial UiTopCenter window show");
                yield return WaitUntil(
                    () => hintsLauncher.CurrentWindow != null &&
                          hintsLauncher.CurrentWindow.GetState() == ObjectState.Shown,
                    "hints overlay show");

                window = launcher.CurrentWindow;
                Assert.That(window.createPool, Is.True);
                Assert.That(WindowPresenterBinder.TryGetBinding(window, out binding), Is.True);
                Assert.That(presenterFactory.CreatedCount, Is.EqualTo(1));
                Assert.That(window.TryGetView(out var view), Is.True);
                Assert.That(hintsLauncher.CurrentWindow.TryGetView(out var hintsView), Is.True);

                var initialInstanceId = window.GetInstanceID();
                var initialView = view;

                localization.ChangeLanguage(SystemLanguage.English);
                yield return null;

                Assert.That(view.TextPressAndHold.text, Is.EqualTo("press and hold"));
                Assert.That(view.TextLocalTime.text, Does.StartWith("Time : "));

                view.HoldInput.OnPointerDown(null);
                yield return new WaitForSeconds(0.65f);

                Assert.That(hintsView.LastDescription,
                    Is.EqualTo("Test hint description, hint can be very large"));

                view.HoldInput.OnPointerUp(null);
                launcher.Hide();
                yield return WaitUntil(() => window.GetState() == ObjectState.Hidden, "top-center hide");

                localization.ChangeLanguage(SystemLanguage.German);
                yield return null;

                Assert.That(view.TextPressAndHold.text, Is.EqualTo("press and hold"));

                view.HoldInput.OnPointerDown(null);
                yield return new WaitForSeconds(0.65f);
                Assert.That(hintsView.LastDescription,
                    Is.EqualTo("Test hint description, hint can be very large"));

                launcher.Show();
                yield return WaitUntil(
                    () => launcher.CurrentWindow != null && launcher.CurrentWindow.GetState() == ObjectState.Shown,
                    "top-center reopen");

                Assert.That(launcher.CurrentWindow.GetInstanceID(), Is.EqualTo(initialInstanceId));
                Assert.That(WindowPresenterBinder.TryGetBinding(launcher.CurrentWindow, out var reopenedBinding),
                    Is.True);
                Assert.That(reopenedBinding, Is.SameAs(binding));
                Assert.That(presenterFactory.CreatedCount, Is.EqualTo(1));
                Assert.That(launcher.CurrentWindow.TryGetView(out view), Is.True);
                Assert.That(view, Is.SameAs(initialView));
                Assert.That(view.TextPressAndHold.text, Is.EqualTo("drücken und halten"));
                Assert.That(view.TextLocalTime.text, Does.StartWith("Zeit : "));

                view.HoldInput.OnPointerDown(null);
                yield return new WaitForSeconds(0.65f);

                Assert.That(hintsView.LastDescription,
                    Is.EqualTo("Testhinweisbeschreibung, Hinweis kann sehr groß sein"));

                launcher.Hide();
                yield return WaitUntil(() => window.GetState() == ObjectState.Hidden, "final top-center hide");

                WindowSystem.Clean(window);
                cleaned = true;

                Assert.That(binding.IsDisposed, Is.True);
                Assert.DoesNotThrow(() => binding.Dispose());
            }
            finally
            {
                launcher?.Dispose();
                if (!cleaned && window != null && window.GetState() == ObjectState.Hidden)
                {
                    WindowSystem.Clean(window);
                }

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
            DestroyObjectsImmediate<UiObjectIndicatorWindow>();
            DestroyObjectsImmediate<UiModalWindow>();
            DestroyObjectsImmediate<UiHintsWindow>();
            DestroyObjectsImmediate<UiFxWindow>();
            DestroyObjectsImmediate<UiTopCenterWindow>();
            DestroyObjectsImmediate<UiDownLeftWindow>();
            DestroyObjectsImmediate<UiDownRightWindow>();
            DestroyObjectsImmediate<UiTopRightWindow>();
            DestroyObjectsImmediate<UiShopWindow>();
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

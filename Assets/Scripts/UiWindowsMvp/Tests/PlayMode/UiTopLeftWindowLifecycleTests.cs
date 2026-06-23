using System.Collections;
using System.Reflection;
using CompositionRoot.Runtime;
using NUnit.Framework;
using ProjectContext.Player;
using UiWindowsMvp.SampleSceneWindows;
using UiWindowsMvp.UIAdapter;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI.Windows;

namespace UiWindowsMvp.Tests.PlayMode
{
    public sealed class UiTopLeftWindowLifecycleTests
    {
        [UnityTest]
        public IEnumerator ReopenCyclesThroughWindowSystem_ReusePooledWindowWithoutDuplicateSubscriptions()
        {
            DestroyWindowSystemsImmediate();
            yield return null;

            SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
            yield return null;

            var root = Object.FindFirstObjectByType<SceneCompositionRoot>();
            Assert.That(root, Is.Not.Null);
            Assert.That(root.IsBootstrapped, Is.True);

            var launcher = root.Services.Resolve<UiTopLeftDemoLauncher>();
            var player = root.Services.Resolve<IPlayerService>();
            var presenterFactory = root.Services.Resolve<UiTopLeftPresenterFactory>();
            UiTopLeftWindow window = null;
            IWindowPresenterBinding<UiTopLeftWindow> binding = null;
            var cleaned = false;

            try
            {
                yield return WaitUntil(
                    () => launcher.CurrentWindow != null && launcher.CurrentWindow.GetState() == ObjectState.Shown,
                    "initial UiTopLeft window show");

                window = launcher.CurrentWindow;
                Assert.That(window.createPool, Is.True);
                Assert.That(WindowPresenterBinder.TryGetBinding(window, out binding), Is.True);
                Assert.That(presenterFactory.CreatedCount, Is.EqualTo(1));

                Assert.That(window.TryGetView(out var view), Is.True);
                var initialInstanceId = window.GetInstanceID();
                var initialView = view;

                for (var cycle = 0; cycle < 3; cycle++)
                {
                    player.SetHealth(player.Settings.MaxHealth - 1);
                    yield return null;
                    player.SetHealth(player.Settings.MaxHealth);
                    yield return null;

                    Assert.That(window.TryGetView(out view), Is.True);
                    Assert.That(view, Is.SameAs(initialView));
                    Assert.That(view.HealthData.TextValue.text,
                        Is.EqualTo($"{player.Settings.MaxHealth} / {player.Settings.MaxHealth}"));

                    view.HealthData.ButtonReduce.onClick.Invoke();
                    Assert.That(
                        GetCurrentValue<int>(player, "Health"),
                        Is.EqualTo(player.Settings.MaxHealth - UiTopLeftPresenter.DefaultHealthCommandStep));
                    Assert.That(view.HealthData.TextValue.text, Is.EqualTo("90 / 100"));

                    launcher.Hide();
                    yield return WaitUntil(() => window.GetState() == ObjectState.Hidden, $"hide cycle {cycle}");

                    player.SetHealth(player.Settings.MaxHealth);
                    Assert.That(view.HealthData.TextValue.text, Is.EqualTo("90 / 100"));

                    view.HealthData.ButtonReduce.onClick.Invoke();
                    Assert.That(GetCurrentValue<int>(player, "Health"), Is.EqualTo(player.Settings.MaxHealth));

                    launcher.Show();
                    yield return WaitUntil(
                        () => launcher.CurrentWindow != null && launcher.CurrentWindow.GetState() == ObjectState.Shown,
                        $"reopen cycle {cycle}");

                    Assert.That(launcher.CurrentWindow.GetInstanceID(), Is.EqualTo(initialInstanceId));
                    Assert.That(WindowPresenterBinder.TryGetBinding(launcher.CurrentWindow, out var reopenedBinding),
                        Is.True);
                    Assert.That(reopenedBinding, Is.SameAs(binding));
                    Assert.That(presenterFactory.CreatedCount, Is.EqualTo(1));
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

        private static T GetCurrentValue<T>(object owner, string propertyName)
        {
            var property = owner.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            Assert.That(property, Is.Not.Null);
            var reactiveSurface = property.GetValue(owner);
            var currentValueProperty =
                reactiveSurface.GetType().GetProperty("CurrentValue", BindingFlags.Public | BindingFlags.Instance);
            Assert.That(currentValueProperty, Is.Not.Null);
            return (T)currentValueProperty.GetValue(reactiveSurface);
        }

        private static void DestroyWindowSystemsImmediate()
        {
            DestroyObjectsImmediate<UiTopRightWindow>();
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

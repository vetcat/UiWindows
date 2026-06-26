using System.Collections;
using CompositionRoot.Runtime;
using NUnit.Framework;
using ProjectContext.Player;
using ProjectContext.UiRequests;
using UiWindowsMvp.SampleSceneWindows;
using UiWindowsMvp.UIAdapter;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI.Windows;

namespace UiWindowsMvp.Tests.PlayMode
{
    public sealed class UiFeedbackWindowLifecycleTests
    {
        [UnityTest]
        public IEnumerator ModalHintsAndFx_RunThroughWindowSystemAndCleanShowScopedRequests()
        {
            DestroyWindowSystemsImmediate();
            yield return null;

            SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
            yield return null;

            var root = Object.FindFirstObjectByType<SceneCompositionRoot>();
            Assert.That(root, Is.Not.Null);
            Assert.That(root.IsBootstrapped, Is.True);

            var modal = root.Services.Resolve<IUiModalService>();
            var feedback = root.Services.Resolve<IUiFeedbackService>();
            var player = root.Services.Resolve<IPlayerService>();
            var modalLauncher = root.Services.Resolve<UiModalDemoLauncher>();
            var hintsLauncher = root.Services.Resolve<UiHintsDemoLauncher>();
            var fxLauncher = root.Services.Resolve<UiFxDemoLauncher>();
            var hintsFactory = root.Services.Resolve<UiHintsPresenterFactory>();
            var fxFactory = root.Services.Resolve<UiFxPresenterFactory>();
            UiModalWindow modalWindow = null;
            UiHintsWindow hintsWindow = null;
            UiFxWindow fxWindow = null;
            IWindowPresenterBinding<UiHintsWindow> hintsBinding = null;
            IWindowPresenterBinding<UiFxWindow> fxBinding = null;
            var cleaned = false;

            try
            {
                yield return WaitUntil(
                    () => hintsLauncher.CurrentWindow != null &&
                          hintsLauncher.CurrentWindow.GetState() == ObjectState.Shown &&
                          fxLauncher.CurrentWindow != null &&
                          fxLauncher.CurrentWindow.GetState() == ObjectState.Shown,
                    "hints and FX overlay show");

                hintsWindow = hintsLauncher.CurrentWindow;
                fxWindow = fxLauncher.CurrentWindow;
                Assert.That(WindowPresenterBinder.TryGetBinding(hintsWindow, out hintsBinding), Is.True);
                Assert.That(WindowPresenterBinder.TryGetBinding(fxWindow, out fxBinding), Is.True);
                Assert.That(hintsFactory.CreatedCount, Is.EqualTo(1));
                Assert.That(fxFactory.CreatedCount, Is.EqualTo(1));

                Assert.That(hintsWindow.TryGetView(out var hintsView), Is.True);
                Assert.That(fxWindow.TryGetView(out var fxView), Is.True);
                var hintsInstanceId = hintsWindow.GetInstanceID();
                var fxInstanceId = fxWindow.GetInstanceID();

                feedback.ShowHint("Open the shop", UiHintAnchor.Top, 0.1f);
                player.AddCoinsWithFx(25);

                Assert.That(hintsView.LastDescription, Is.EqualTo("Open the shop"));
                Assert.That(fxView.LastFxText, Is.EqualTo("+25"));
                Assert.That(GetCurrentValue<int>(player, "Coins"), Is.EqualTo(25));

                modal.ShowInfoOk("Reward", "Coins collected");
                yield return WaitUntil(
                    () => modalLauncher.CurrentWindow != null &&
                          modalLauncher.CurrentWindow.GetState() == ObjectState.Shown,
                    "modal show");
                modalWindow = modalLauncher.CurrentWindow;
                Assert.That(WindowPresenterBinder.TryGetBinding(modalWindow, out _), Is.True);
                Assert.That(modalWindow.TryGetView(out var modalView), Is.True);
                Assert.That(modalView.TextCaption.text, Is.EqualTo("Reward"));
                modalView.ButtonOk.onClick.Invoke();
                yield return WaitUntil(() => modalWindow.GetState() == ObjectState.Hidden, "modal hide");

                hintsLauncher.Hide();
                fxLauncher.Hide();
                yield return WaitUntil(
                    () => hintsWindow.GetState() == ObjectState.Hidden && fxWindow.GetState() == ObjectState.Hidden,
                    "feedback overlays hide");

                feedback.ShowHint("Hidden hint", UiHintAnchor.Bottom, 0.1f);
                player.AddCoinsWithFx(10);

                Assert.That(hintsView.LastDescription, Is.EqualTo("Open the shop"));
                Assert.That(fxView.LastFxText, Is.EqualTo("+25"));
                Assert.That(fxView.ActiveFxCount, Is.Zero);
                Assert.That(GetCurrentValue<int>(player, "Coins"), Is.EqualTo(35));

                hintsLauncher.Show();
                fxLauncher.Show();
                yield return WaitUntil(
                    () => hintsLauncher.CurrentWindow.GetState() == ObjectState.Shown &&
                          fxLauncher.CurrentWindow.GetState() == ObjectState.Shown,
                    "feedback overlays reopen");

                Assert.That(hintsLauncher.CurrentWindow.GetInstanceID(), Is.EqualTo(hintsInstanceId));
                Assert.That(fxLauncher.CurrentWindow.GetInstanceID(), Is.EqualTo(fxInstanceId));
                Assert.That(WindowPresenterBinder.TryGetBinding(hintsLauncher.CurrentWindow, out var reopenedHints),
                    Is.True);
                Assert.That(WindowPresenterBinder.TryGetBinding(fxLauncher.CurrentWindow, out var reopenedFx),
                    Is.True);
                Assert.That(reopenedHints, Is.SameAs(hintsBinding));
                Assert.That(reopenedFx, Is.SameAs(fxBinding));
                Assert.That(hintsFactory.CreatedCount, Is.EqualTo(1));
                Assert.That(fxFactory.CreatedCount, Is.EqualTo(1));

                feedback.ShowHint("Visible again", UiHintAnchor.Center, 0.1f);
                player.RemoveCoinsWithFx(5);

                Assert.That(hintsView.LastDescription, Is.EqualTo("Visible again"));
                Assert.That(fxView.LastFxText, Is.EqualTo("-5"));
                Assert.That(GetCurrentValue<int>(player, "Coins"), Is.EqualTo(30));

                hintsLauncher.Hide();
                fxLauncher.Hide();
                yield return WaitUntil(
                    () => hintsWindow.GetState() == ObjectState.Hidden && fxWindow.GetState() == ObjectState.Hidden,
                    "final feedback overlay hide");

                WindowSystem.Clean(hintsWindow);
                WindowSystem.Clean(fxWindow);
                WindowSystem.Clean(modalWindow);
                cleaned = true;

                Assert.That(hintsBinding.IsDisposed, Is.True);
                Assert.That(fxBinding.IsDisposed, Is.True);
            }
            finally
            {
                modalLauncher?.Dispose();
                hintsLauncher?.Dispose();
                fxLauncher?.Dispose();
                if (!cleaned)
                {
                    CleanHidden(modalWindow);
                    CleanHidden(hintsWindow);
                    CleanHidden(fxWindow);
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

        private static void CleanHidden(WindowBase window)
        {
            if (window != null && window.GetState() == ObjectState.Hidden)
            {
                WindowSystem.Clean(window);
            }
        }

        private static void DestroyWindowSystemsImmediate()
        {
            DestroyObjectsImmediate<UiObjectIndicatorWindow>();
            DestroyObjectsImmediate<UiModalWindow>();
            DestroyObjectsImmediate<UiHintsWindow>();
            DestroyObjectsImmediate<UiFxWindow>();
            DestroyObjectsImmediate<UiTopCenterWindow>();
            DestroyObjectsImmediate<UiTopRightWindow>();
            DestroyObjectsImmediate<UiDownLeftWindow>();
            DestroyObjectsImmediate<UiDownRightWindow>();
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

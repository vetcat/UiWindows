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
    public sealed class UiTopRightWindowLifecycleTests
    {
        [UnityTest]
        public IEnumerator ReopenCyclesThroughWindowSystem_ReusePooledWindowAndRoutesFxTarget()
        {
            DestroyWindowSystemsImmediate();
            yield return null;

            SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
            yield return null;

            var root = Object.FindFirstObjectByType<SceneCompositionRoot>();
            Assert.That(root, Is.Not.Null);
            Assert.That(root.IsBootstrapped, Is.True);

            var launcher = root.Services.Resolve<UiTopRightDemoLauncher>();
            var fxLauncher = root.Services.Resolve<UiFxDemoLauncher>();
            var player = root.Services.Resolve<IPlayerService>();
            var targetResolver = root.Services.Resolve<IUiFxTargetResolver>();
            var presenterFactory = root.Services.Resolve<UiTopRightPresenterFactory>();
            UiTopRightWindow window = null;
            UiFxWindow fxWindow = null;
            IWindowPresenterBinding<UiTopRightWindow> binding = null;
            var cleaned = false;

            try
            {
                yield return WaitUntil(
                    () => launcher.CurrentWindow != null && launcher.CurrentWindow.GetState() == ObjectState.Shown,
                    "initial UiTopRight window show");
                yield return WaitUntil(
                    () => fxLauncher.CurrentWindow != null && fxLauncher.CurrentWindow.GetState() == ObjectState.Shown,
                    "FX overlay show");

                window = launcher.CurrentWindow;
                fxWindow = fxLauncher.CurrentWindow;
                Assert.That(window.createPool, Is.True);
                Assert.That(WindowPresenterBinder.TryGetBinding(window, out binding), Is.True);
                Assert.That(presenterFactory.CreatedCount, Is.EqualTo(1));

                Assert.That(window.TryGetView(out var view), Is.True);
                Assert.That(fxWindow.TryGetView(out var fxView), Is.True);
                var initialInstanceId = window.GetInstanceID();
                var initialView = view;
                var fallbackCollectTarget = fxView.CollectTargetAnchoredPosition;
                var fallbackSpendSource = fxView.SpendSourceAnchoredPosition;

                for (var cycle = 0; cycle < 3; cycle++)
                {
                    player.SetCoins(100 + cycle);
                    yield return null;

                    Assert.That(window.TryGetView(out view), Is.True);
                    Assert.That(view, Is.SameAs(initialView));
                    Assert.That(view.TextCoinsAmount.text, Is.EqualTo((100 + cycle).ToString()));
                    Assert.That(targetResolver.TryGetTarget(UiFxTarget.Coins, out var target), Is.True);
                    Assert.That(target, Is.SameAs(view.CoinIconRectTransform));

                    var expectedTarget = ResolveLocalPoint(fxView.Body, view.CoinIconRectTransform);
                    player.AddCoinsWithFx(5);

                    Assert.That(fxView.LastFxText, Is.EqualTo("+5"));
                    Assert.That(fxView.CollectTargetAnchoredPosition.x, Is.EqualTo(expectedTarget.x).Within(0.5f));
                    Assert.That(fxView.CollectTargetAnchoredPosition.y, Is.EqualTo(expectedTarget.y).Within(0.5f));

                    launcher.Hide();
                    yield return WaitUntil(() => window.GetState() == ObjectState.Hidden, $"hide cycle {cycle}");

                    Assert.That(targetResolver.TryGetTarget(UiFxTarget.Coins, out _), Is.False);
                    player.AddCoinsWithFx(7);
                    Assert.That(fxView.LastFxText, Is.EqualTo("+7"));
                    Assert.That(fxView.CollectTargetAnchoredPosition.x,
                        Is.EqualTo(fallbackCollectTarget.x).Within(0.5f));
                    Assert.That(fxView.CollectTargetAnchoredPosition.y,
                        Is.EqualTo(fallbackCollectTarget.y).Within(0.5f));

                    player.RemoveCoinsWithFx(3);
                    Assert.That(fxView.LastFxText, Is.EqualTo("-3"));
                    Assert.That(fxView.SpendSourceAnchoredPosition.x,
                        Is.EqualTo(fallbackSpendSource.x).Within(0.5f));
                    Assert.That(fxView.SpendSourceAnchoredPosition.y,
                        Is.EqualTo(fallbackSpendSource.y).Within(0.5f));

                    player.SetCoins(200 + cycle);
                    Assert.That(view.TextCoinsAmount.text, Is.EqualTo((105 + cycle).ToString()));

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
                    Assert.That(view.TextCoinsAmount.text, Is.EqualTo((200 + cycle).ToString()));
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

        private static Vector2 ResolveLocalPoint(RectTransform owner, RectTransform target)
        {
            var corners = new Vector3[4];
            target.GetWorldCorners(corners);
            var worldCenter = (corners[0] + corners[2]) * 0.5f;
            var sourceCamera = GetRectTransformCamera(target);
            var screenPoint = RectTransformUtility.WorldToScreenPoint(sourceCamera, worldCenter);
            var targetCamera = GetRectTransformCamera(owner);
            Assert.That(
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    owner,
                    screenPoint,
                    targetCamera,
                    out var anchoredPosition),
                Is.True);
            return anchoredPosition;
        }

        private static Camera GetRectTransformCamera(RectTransform rectTransform)
        {
            var canvas = rectTransform.GetComponentInParent<Canvas>();
            if (canvas == null || canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                return null;
            }

            return canvas.worldCamera;
        }

        private static void DestroyWindowSystemsImmediate()
        {
            DestroyObjectsImmediate<UiObjectIndicatorWindow>();
            DestroyObjectsImmediate<UiFxWindow>();
            DestroyObjectsImmediate<UiDownLeftWindow>();
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

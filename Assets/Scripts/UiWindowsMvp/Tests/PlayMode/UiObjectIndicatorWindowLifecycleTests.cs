using System.Collections;
using System.Reflection;
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
    public sealed class UiObjectIndicatorWindowLifecycleTests
    {
        [UnityTest]
        public IEnumerator ObjectIndicator_RunThroughWindowSystem_FollowsTargetAndRoutesRewardFx()
        {
            DestroyWindowSystemsImmediate();
            yield return null;

            SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
            yield return null;

            var root = Object.FindFirstObjectByType<SceneCompositionRoot>();
            Assert.That(root, Is.Not.Null);
            Assert.That(root.IsBootstrapped, Is.True);

            var launcher = root.Services.Resolve<UiObjectIndicatorDemoLauncher>();
            var target = root.Services.Resolve<UiObjectIndicatorDemoTarget>();
            var player = root.Services.Resolve<IPlayerService>();
            var targetResolver = root.Services.Resolve<IUiFxTargetResolver>();
            var presenterFactory = root.Services.Resolve<UiObjectIndicatorPresenterFactory>();
            var topRightLauncher = root.Services.Resolve<UiTopRightDemoLauncher>();
            var fxLauncher = root.Services.Resolve<UiFxDemoLauncher>();
            UiObjectIndicatorWindow window = null;
            IWindowPresenterBinding<UiObjectIndicatorWindow> binding = null;
            var cleaned = false;

            try
            {
                yield return WaitUntil(
                    () => launcher.CurrentWindow != null && launcher.CurrentWindow.GetState() == ObjectState.Shown,
                    "object indicator show");
                yield return WaitUntil(
                    () => topRightLauncher.CurrentWindow != null &&
                          topRightLauncher.CurrentWindow.GetState() == ObjectState.Shown &&
                          fxLauncher.CurrentWindow != null &&
                          fxLauncher.CurrentWindow.GetState() == ObjectState.Shown,
                    "coin HUD and FX overlay show");

                window = launcher.CurrentWindow;
                Assert.That(window.createPool, Is.True);
                Assert.That(WindowPresenterBinder.TryGetBinding(window, out binding), Is.True);
                Assert.That(presenterFactory.CreatedCount, Is.EqualTo(1));
                Assert.That(window.TryGetView(out var view), Is.True);
                Assert.That(fxLauncher.CurrentWindow.TryGetView(out var fxView), Is.True);
                Assert.That(topRightLauncher.CurrentWindow.TryGetView(out var topRightView), Is.True);

                var initialInstanceId = window.GetInstanceID();
                var initialView = view;
                var initialPosition = view.LastAnchoredPosition;

                target.MoveTo(new Vector3(2f, 0f, 4f));
                yield return null;
                yield return null;

                Assert.That(view.IsIndicatorVisible, Is.True);
                Assert.That(view.LastAnchoredPosition, Is.Not.EqualTo(initialPosition));
                Assert.That(targetResolver.TryGetTarget(UiFxTarget.CharacterReward, out var rewardAnchor), Is.True);
                Assert.That(rewardAnchor, Is.SameAs(view.RewardAnchor));

                var expectedSource = ResolveLocalPoint(fxView.Body, view.RewardAnchor);
                var expectedTarget = ResolveLocalPoint(fxView.Body, topRightView.CoinIconRectTransform);
                view.ButtonAction.onClick.Invoke();

                Assert.That(GetCurrentValue<int>(player, "Coins"), Is.EqualTo(target.RewardAmount));
                Assert.That(fxView.LastFxText, Is.EqualTo("+" + target.RewardAmount));
                Assert.That(fxView.CollectSourceAnchoredPosition.x, Is.EqualTo(expectedSource.x).Within(0.5f));
                Assert.That(fxView.CollectSourceAnchoredPosition.y, Is.EqualTo(expectedSource.y).Within(0.5f));
                Assert.That(fxView.CollectTargetAnchoredPosition.x, Is.EqualTo(expectedTarget.x).Within(0.5f));
                Assert.That(fxView.CollectTargetAnchoredPosition.y, Is.EqualTo(expectedTarget.y).Within(0.5f));

                launcher.Hide();
                yield return WaitUntil(() => window.GetState() == ObjectState.Hidden, "object indicator hide");

                Assert.That(view.IsIndicatorVisible, Is.False);
                Assert.That(targetResolver.TryGetTarget(UiFxTarget.CharacterReward, out _), Is.False);
                view.ButtonAction.onClick.Invoke();
                Assert.That(GetCurrentValue<int>(player, "Coins"), Is.EqualTo(target.RewardAmount));

                target.MoveTo(new Vector3(-2f, 0f, 4f));
                yield return null;
                Assert.That(view.LastAnchoredPosition, Is.Not.EqualTo(Vector2.zero));

                launcher.Show();
                yield return WaitUntil(
                    () => launcher.CurrentWindow != null && launcher.CurrentWindow.GetState() == ObjectState.Shown,
                    "object indicator reopen");

                Assert.That(launcher.CurrentWindow.GetInstanceID(), Is.EqualTo(initialInstanceId));
                Assert.That(WindowPresenterBinder.TryGetBinding(launcher.CurrentWindow, out var reopenedBinding),
                    Is.True);
                Assert.That(reopenedBinding, Is.SameAs(binding));
                Assert.That(presenterFactory.CreatedCount, Is.EqualTo(1));
                Assert.That(launcher.CurrentWindow.TryGetView(out view), Is.True);
                Assert.That(view, Is.SameAs(initialView));

                launcher.Hide();
                yield return WaitUntil(() => window.GetState() == ObjectState.Hidden, "final object indicator hide");

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

        private static T GetCurrentValue<T>(object owner, string propertyName)
        {
            var property = owner.GetType().GetProperty(
                propertyName,
                BindingFlags.Public | BindingFlags.Instance);
            Assert.That(property, Is.Not.Null);
            var reactiveSurface = property.GetValue(owner);
            var currentValueProperty =
                reactiveSurface.GetType().GetProperty(
                    "CurrentValue",
                    BindingFlags.Public | BindingFlags.Instance);
            Assert.That(currentValueProperty, Is.Not.Null);
            return (T)currentValueProperty.GetValue(reactiveSurface);
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

using NUnit.Framework;
using ProjectContext.UiRequests;
using UiWindowsMvp.SampleSceneWindows;
using UiWindowsMvp.UIAdapter;
using UnityEngine;

namespace UiWindowsMvp.Tests.PlayMode
{
    public sealed class UiFeedbackPresenterTests
    {
        [Test]
        public void HintsPresenter_ConsumesRequestsOnlyWhileShown()
        {
            var viewObject = new GameObject("UiHints Test View", typeof(RectTransform), typeof(UiHintsView));
            var windowObject = new GameObject("UiHints Test Window", typeof(RectTransform), typeof(UiHintsWindow));
            using var feedback = new UiFeedbackService();
            var firstScope = new WindowPresenterShowScope();
            var secondScope = new WindowPresenterShowScope();

            try
            {
                var view = UiViewTestFixtures.Configure(viewObject.GetComponent<UiHintsView>());
                var window = windowObject.GetComponent<UiHintsWindow>();
                var presenter = new UiHintsPresenter(feedback, _ => view);

                presenter.Bind(window);
                presenter.Initialize();
                presenter.OnShowBegin(firstScope);

                feedback.ShowHint("Tap the item", UiHintAnchor.Bottom, 0.1f);
                Assert.That(view.LastDescription, Is.EqualTo("Tap the item"));

                presenter.OnHideBegin();
                firstScope.Dispose();
                feedback.ShowHint("Hidden hint", UiHintAnchor.Top, 0.1f);

                Assert.That(view.LastDescription, Is.EqualTo("Tap the item"));
                Assert.That(view.CurrentAlpha, Is.Zero);

                presenter.OnShowBegin(secondScope);
                feedback.ShowHint("Shown again", UiHintAnchor.Top, 0.1f);

                Assert.That(view.LastDescription, Is.EqualTo("Shown again"));
            }
            finally
            {
                firstScope.Dispose();
                secondScope.Dispose();
                Object.DestroyImmediate(windowObject);
                Object.DestroyImmediate(viewObject);
            }
        }

        [Test]
        public void FxPresenter_KillsTweensOnHideAndConsumesRequestsOnlyWhileShown()
        {
            var viewObject = new GameObject("UiFx Test View", typeof(RectTransform), typeof(UiFxView));
            var windowObject = new GameObject("UiFx Test Window", typeof(RectTransform), typeof(UiFxWindow));
            using var feedback = new UiFeedbackService();
            var firstScope = new WindowPresenterShowScope();
            var secondScope = new WindowPresenterShowScope();

            try
            {
                var view = UiViewTestFixtures.Configure(viewObject.GetComponent<UiFxView>());
                var window = windowObject.GetComponent<UiFxWindow>();
                var presenter = new UiFxPresenter(feedback, _ => view);

                presenter.Bind(window);
                presenter.Initialize();
                presenter.OnShowBegin(firstScope);

                feedback.RequestCollectFx(15, UiFxTarget.Coins);
                Assert.That(view.LastFxText, Is.EqualTo("+15"));
                Assert.That(view.ActiveFxCount, Is.EqualTo(1));

                presenter.OnHideBegin();
                firstScope.Dispose();
                Assert.That(view.ActiveFxCount, Is.Zero);
                Assert.That(view.PooledFxCount, Is.GreaterThanOrEqualTo(1));

                feedback.RequestSpendFx(5, UiFxTarget.Coins);
                Assert.That(view.LastFxText, Is.EqualTo("+15"));

                presenter.OnShowBegin(secondScope);
                feedback.RequestSpendFx(5, UiFxTarget.Coins);

                Assert.That(view.LastFxText, Is.EqualTo("-5"));
                Assert.That(view.ActiveFxCount, Is.EqualTo(1));
            }
            finally
            {
                firstScope.Dispose();
                secondScope.Dispose();
                Object.DestroyImmediate(windowObject);
                Object.DestroyImmediate(viewObject);
            }
        }

        [Test]
        public void FxPresenter_RoutesSemanticSourceAndTarget()
        {
            var viewObject = new GameObject("UiFx Routed Test View", typeof(RectTransform), typeof(UiFxView));
            var windowObject = new GameObject("UiFx Routed Test Window", typeof(RectTransform), typeof(UiFxWindow));
            using var feedback = new UiFeedbackService();
            var registry = new UiFxTargetRegistry();
            var scope = new WindowPresenterShowScope();

            try
            {
                var view = UiViewTestFixtures.Configure(viewObject.GetComponent<UiFxView>());
                var window = windowObject.GetComponent<UiFxWindow>();
                var presenter = new UiFxPresenter(feedback, registry, _ => view);

                presenter.Bind(window);
                presenter.Initialize();
                presenter.OnShowBegin(scope);

                var source = CreateRect("CharacterRewardSource", view.Body, new Vector2(190f, -72f));
                var target = CreateRect("CoinsTarget", view.Body, new Vector2(72f, -48f));
                using var sourceRegistration = registry.RegisterTarget(UiFxTarget.CharacterReward, source);
                using var targetRegistration = registry.RegisterTarget(UiFxTarget.Coins, target);

                feedback.RequestCollectFxFrom(12, UiFxTarget.CharacterReward, UiFxTarget.Coins);

                Assert.That(view.LastFxText, Is.EqualTo("+12"));
                Assert.That(view.CollectSourceAnchoredPosition.x,
                    Is.EqualTo(ResolveLocalPoint(view.Body, source).x).Within(0.5f));
                Assert.That(view.CollectSourceAnchoredPosition.y,
                    Is.EqualTo(ResolveLocalPoint(view.Body, source).y).Within(0.5f));
                Assert.That(view.CollectTargetAnchoredPosition.x,
                    Is.EqualTo(ResolveLocalPoint(view.Body, target).x).Within(0.5f));
                Assert.That(view.CollectTargetAnchoredPosition.y,
                    Is.EqualTo(ResolveLocalPoint(view.Body, target).y).Within(0.5f));
            }
            finally
            {
                scope.Dispose();
                Object.DestroyImmediate(windowObject);
                Object.DestroyImmediate(viewObject);
            }
        }

        private static RectTransform CreateRect(string name, Transform parent, Vector2 anchoredPosition)
        {
            var gameObject = new GameObject(name, typeof(RectTransform));
            gameObject.transform.SetParent(parent, false);
            var rectTransform = (RectTransform)gameObject.transform;
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.anchoredPosition = anchoredPosition;
            rectTransform.sizeDelta = new Vector2(32f, 32f);
            return rectTransform;
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
    }
}

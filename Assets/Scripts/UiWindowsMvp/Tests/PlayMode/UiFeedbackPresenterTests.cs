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
                var view = viewObject.GetComponent<UiHintsView>();
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
                var view = viewObject.GetComponent<UiFxView>();
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
    }
}
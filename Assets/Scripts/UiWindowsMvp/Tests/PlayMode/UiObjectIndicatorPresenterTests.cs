using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using ProjectContext.Player;
using ProjectContext.UiRequests;
using UiWindowsMvp.SampleSceneWindows;
using UiWindowsMvp.UIAdapter;
using UnityEngine;

namespace UiWindowsMvp.Tests.PlayMode
{
    public sealed class UiObjectIndicatorPresenterTests
    {
        [Test]
        public void Presenter_BindsTargetFollowsAnchorAndRewardsOnlyWhileShown()
        {
            var viewObject = new GameObject(
                "UiObjectIndicator Test View",
                typeof(RectTransform),
                typeof(UiObjectIndicatorView));
            var windowObject = new GameObject(
                "UiObjectIndicator Test Window",
                typeof(RectTransform),
                typeof(UiObjectIndicatorWindow));
            var feedback = new TrackingFeedbackCommands();
            using var service = new PlayerService(PlayerSettings.Default, feedback);
            var registry = new UiFxTargetRegistry();
            var anchor = new FakeIndicatorAnchor();
            var screenAdapter = new FakeWorldToScreenAdapter();
            var firstScope = new WindowPresenterShowScope();
            var secondScope = new WindowPresenterShowScope();

            try
            {
                var view = viewObject.GetComponent<UiObjectIndicatorView>();
                var window = windowObject.GetComponent<UiObjectIndicatorWindow>();
                var presenter = new UiObjectIndicatorPresenter(
                    service,
                    service,
                    PlayerSettings.Default,
                    anchor,
                    screenAdapter,
                    registry,
                    _ => view);

                presenter.Bind(window);
                presenter.Initialize();
                presenter.OnShowBegin(firstScope);

                Assert.That(view.IsIndicatorVisible, Is.True);
                Assert.That(screenAdapter.LastWorldPosition, Is.EqualTo(anchor.WorldPosition));
                Assert.That(registry.TryGetTarget(UiFxTarget.CharacterReward, out var target), Is.True);
                Assert.That(target, Is.SameAs(view.RewardAnchor));

                service.SetHealth(50);
                Assert.That(view.HealthSliderValue, Is.EqualTo(0.5f).Within(0.001f));

                view.ButtonAction.onClick.Invoke();
                Assert.That(GetCurrentValue<int>(service, "Coins"), Is.EqualTo(anchor.RewardAmount));
                Assert.That(feedback.Requests, Has.Count.EqualTo(1));
                Assert.That(feedback.Requests[0].Kind, Is.EqualTo(UiFxKind.Collect));
                Assert.That(feedback.Requests[0].Source, Is.EqualTo(UiFxTarget.CharacterReward));
                Assert.That(feedback.Requests[0].Target, Is.EqualTo(UiFxTarget.Coins));

                presenter.OnHideBegin();
                Assert.That(registry.TryGetTarget(UiFxTarget.CharacterReward, out _), Is.True);
                Assert.That(presenter.UpdateIndicatorPosition(), Is.False);
                Assert.That(view.IsIndicatorVisible, Is.False);

                view.ButtonAction.onClick.Invoke();
                Assert.That(GetCurrentValue<int>(service, "Coins"), Is.EqualTo(anchor.RewardAmount));

                firstScope.Dispose();
                Assert.That(view.IsIndicatorVisible, Is.False);
                Assert.That(registry.TryGetTarget(UiFxTarget.CharacterReward, out _), Is.False);

                service.SetHealth(10);
                Assert.That(view.HealthSliderValue, Is.EqualTo(0.5f).Within(0.001f));
                view.ButtonAction.onClick.Invoke();
                Assert.That(GetCurrentValue<int>(service, "Coins"), Is.EqualTo(anchor.RewardAmount));

                anchor.WorldPosition = new Vector3(3f, 2f, 1f);
                screenAdapter.ScreenPosition = new Vector2(320f, 240f);
                presenter.OnShowBegin(secondScope);

                Assert.That(view.HealthSliderValue, Is.EqualTo(0.1f).Within(0.001f));
                Assert.That(presenter.UpdateIndicatorPosition(), Is.True);
                Assert.That(screenAdapter.LastWorldPosition, Is.EqualTo(anchor.WorldPosition));
                Assert.That(view.LastAnchoredPosition, Is.Not.EqualTo(Vector2.zero));

                view.ButtonAction.onClick.Invoke();
                Assert.That(GetCurrentValue<int>(service, "Coins"), Is.EqualTo(anchor.RewardAmount * 2));
            }
            finally
            {
                firstScope.Dispose();
                secondScope.Dispose();
                Object.DestroyImmediate(windowObject);
                Object.DestroyImmediate(viewObject);
            }
        }

        private sealed class FakeIndicatorAnchor : IUiObjectIndicatorAnchor
        {
            public string DisplayName => "Test Character";
            public int RewardAmount => 25;
            public Vector3 WorldPosition { get; set; } = new(1f, 2f, 3f);

            public bool TryGetWorldPosition(out Vector3 worldPosition)
            {
                worldPosition = WorldPosition;
                return true;
            }
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

        private sealed class FakeWorldToScreenAdapter : IUiWorldToScreenAdapter
        {
            public Vector2 ScreenPosition { get; set; } = new(120f, 160f);
            public Vector3 LastWorldPosition { get; private set; }

            public bool TryGetScreenPosition(Vector3 worldPosition, out Vector2 screenPosition)
            {
                LastWorldPosition = worldPosition;
                screenPosition = ScreenPosition;
                return true;
            }
        }

        private sealed class TrackingFeedbackCommands : IUiFeedbackCommands
        {
            public List<UiFxRequest> Requests { get; } = new();

            public void ShowHint(
                string description,
                UiHintAnchor anchor = UiHintAnchor.Center,
                float durationSeconds = 1.5f)
            {
            }

            public void RequestCollectFx(int amount, UiFxTarget target = UiFxTarget.Coins)
            {
                Requests.Add(new UiFxRequest(UiFxKind.Collect, amount, target));
            }

            public void RequestCollectFxFrom(int amount, UiFxTarget source, UiFxTarget target = UiFxTarget.Coins)
            {
                Requests.Add(new UiFxRequest(UiFxKind.Collect, amount, source, target));
            }

            public void RequestSpendFx(int amount, UiFxTarget target = UiFxTarget.Coins)
            {
                Requests.Add(new UiFxRequest(UiFxKind.Spend, amount, target));
            }

            public void RequestSpendFxFrom(int amount, UiFxTarget source, UiFxTarget target = UiFxTarget.Coins)
            {
                Requests.Add(new UiFxRequest(UiFxKind.Spend, amount, source, target));
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using ProjectContext.Player;
using ProjectContext.UiRequests;
using UiWindowsMvp.SampleSceneWindows;
using UiWindowsMvp.UIAdapter;
using UnityEngine;
using Object = UnityEngine.Object;

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
            var updateSource = new TrackingObjectIndicatorUpdateSource();
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
                    updateSource,
                    _ => view);

                presenter.Bind(window);
                presenter.Initialize();
                presenter.OnShowBegin(firstScope);

                Assert.That(updateSource.SubscriberCount, Is.EqualTo(1));
                Assert.That(updateSource.IsRunning, Is.True);
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
                Assert.That(updateSource.SubscriberCount, Is.Zero);
                Assert.That(updateSource.IsRunning, Is.False);
                Assert.That(view.IsIndicatorVisible, Is.False);
                Assert.That(registry.TryGetTarget(UiFxTarget.CharacterReward, out _), Is.False);

                service.SetHealth(10);
                Assert.That(view.HealthSliderValue, Is.EqualTo(0.5f).Within(0.001f));
                view.ButtonAction.onClick.Invoke();
                Assert.That(GetCurrentValue<int>(service, "Coins"), Is.EqualTo(anchor.RewardAmount));

                anchor.WorldPosition = new Vector3(3f, 2f, 1f);
                screenAdapter.ScreenPosition = new Vector2(320f, 240f);
                presenter.OnShowBegin(secondScope);

                Assert.That(updateSource.SubscriberCount, Is.EqualTo(1));
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

        [Test]
        public void Presenters_ShareUpdateSourceAndUnregisterHiddenIndicators()
        {
            var firstViewObject = new GameObject(
                "First UiObjectIndicator Test View",
                typeof(RectTransform),
                typeof(UiObjectIndicatorView));
            var firstWindowObject = new GameObject(
                "First UiObjectIndicator Test Window",
                typeof(RectTransform),
                typeof(UiObjectIndicatorWindow));
            var secondViewObject = new GameObject(
                "Second UiObjectIndicator Test View",
                typeof(RectTransform),
                typeof(UiObjectIndicatorView));
            var secondWindowObject = new GameObject(
                "Second UiObjectIndicator Test Window",
                typeof(RectTransform),
                typeof(UiObjectIndicatorWindow));
            var feedback = new TrackingFeedbackCommands();
            using var service = new PlayerService(PlayerSettings.Default, feedback);
            var registry = new UiFxTargetRegistry();
            var firstAnchor = new FakeIndicatorAnchor { DisplayNameValue = "First Character" };
            var secondAnchor = new FakeIndicatorAnchor { DisplayNameValue = "Second Character" };
            var firstScreenAdapter = new FakeWorldToScreenAdapter { ScreenPosition = new Vector2(120f, 160f) };
            var secondScreenAdapter = new FakeWorldToScreenAdapter { ScreenPosition = new Vector2(220f, 260f) };
            var updateSource = new TrackingObjectIndicatorUpdateSource();
            var firstScope = new WindowPresenterShowScope();
            var secondScope = new WindowPresenterShowScope();

            try
            {
                var firstView = firstViewObject.GetComponent<UiObjectIndicatorView>();
                var firstWindow = firstWindowObject.GetComponent<UiObjectIndicatorWindow>();
                var firstPresenter = new UiObjectIndicatorPresenter(
                    service,
                    service,
                    PlayerSettings.Default,
                    firstAnchor,
                    firstScreenAdapter,
                    registry,
                    updateSource,
                    _ => firstView);
                var secondView = secondViewObject.GetComponent<UiObjectIndicatorView>();
                var secondWindow = secondWindowObject.GetComponent<UiObjectIndicatorWindow>();
                var secondPresenter = new UiObjectIndicatorPresenter(
                    service,
                    service,
                    PlayerSettings.Default,
                    secondAnchor,
                    secondScreenAdapter,
                    registry,
                    updateSource,
                    _ => secondView);

                firstPresenter.Bind(firstWindow);
                firstPresenter.Initialize();
                firstPresenter.OnShowBegin(firstScope);
                secondPresenter.Bind(secondWindow);
                secondPresenter.Initialize();
                secondPresenter.OnShowBegin(secondScope);

                Assert.That(updateSource.SubscriberCount, Is.EqualTo(2));
                Assert.That(updateSource.IsRunning, Is.True);

                var firstInitialPosition = firstView.LastAnchoredPosition;
                var secondInitialPosition = secondView.LastAnchoredPosition;
                firstAnchor.WorldPosition = new Vector3(2f, 3f, 4f);
                secondAnchor.WorldPosition = new Vector3(5f, 6f, 7f);
                firstScreenAdapter.ScreenPosition = new Vector2(320f, 360f);
                secondScreenAdapter.ScreenPosition = new Vector2(420f, 460f);
                updateSource.Tick();

                Assert.That(firstScreenAdapter.LastWorldPosition, Is.EqualTo(firstAnchor.WorldPosition));
                Assert.That(secondScreenAdapter.LastWorldPosition, Is.EqualTo(secondAnchor.WorldPosition));
                Assert.That(firstView.LastAnchoredPosition, Is.Not.EqualTo(firstInitialPosition));
                Assert.That(secondView.LastAnchoredPosition, Is.Not.EqualTo(secondInitialPosition));

                firstPresenter.OnHideBegin();
                firstScope.Dispose();
                Assert.That(updateSource.SubscriberCount, Is.EqualTo(1));
                Assert.That(updateSource.IsRunning, Is.True);

                var hiddenPosition = firstView.LastAnchoredPosition;
                var secondVisiblePosition = secondView.LastAnchoredPosition;
                firstAnchor.WorldPosition = new Vector3(8f, 8f, 8f);
                secondAnchor.WorldPosition = new Vector3(9f, 9f, 9f);
                firstScreenAdapter.ScreenPosition = new Vector2(520f, 560f);
                secondScreenAdapter.ScreenPosition = new Vector2(620f, 660f);
                updateSource.Tick();

                Assert.That(firstView.LastAnchoredPosition, Is.EqualTo(hiddenPosition));
                Assert.That(secondView.LastAnchoredPosition, Is.Not.EqualTo(secondVisiblePosition));

                secondPresenter.OnHideBegin();
                secondScope.Dispose();
                Assert.That(updateSource.SubscriberCount, Is.Zero);
                Assert.That(updateSource.IsRunning, Is.False);
            }
            finally
            {
                firstScope.Dispose();
                secondScope.Dispose();
                Object.DestroyImmediate(secondWindowObject);
                Object.DestroyImmediate(secondViewObject);
                Object.DestroyImmediate(firstWindowObject);
                Object.DestroyImmediate(firstViewObject);
            }
        }

        private sealed class FakeIndicatorAnchor : IUiObjectIndicatorAnchor
        {
            public string DisplayName => DisplayNameValue;
            public int RewardAmount => 25;
            public string DisplayNameValue { get; set; } = "Test Character";
            public Vector3 WorldPosition { get; set; } = new(1f, 2f, 3f);

            public bool TryGetWorldPosition(out Vector3 worldPosition)
            {
                worldPosition = WorldPosition;
                return true;
            }
        }

        private sealed class TrackingObjectIndicatorUpdateSource : IUiObjectIndicatorUpdateSource
        {
            private readonly List<Action> subscribers = new();

            public int SubscriberCount => subscribers.Count;

            public bool IsRunning => subscribers.Count > 0;

            public IDisposable Register(Action update)
            {
                subscribers.Add(update);
                return new TrackingDisposable(() => subscribers.Remove(update));
            }

            public void Tick()
            {
                for (var i = 0; i < subscribers.Count; i++)
                {
                    subscribers[i]?.Invoke();
                }
            }
        }

        private sealed class TrackingDisposable : IDisposable
        {
            private Action action;

            public TrackingDisposable(Action action)
            {
                this.action = action;
            }

            public void Dispose()
            {
                var callback = action;
                action = null;
                callback?.Invoke();
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

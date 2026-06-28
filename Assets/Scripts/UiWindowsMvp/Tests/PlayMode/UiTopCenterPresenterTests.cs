using System;
using System.Collections.Generic;
using NUnit.Framework;
using ProjectContext.Localization;
using ProjectContext.UiRequests;
using UiWindowsMvp.SampleSceneWindows;
using UiWindowsMvp.UIAdapter;
using UnityEngine;

namespace UiWindowsMvp.Tests.PlayMode
{
    public sealed class UiTopCenterPresenterTests
    {
        [Test]
        public void Presenter_RefreshesLocalizedTimeAndStopsUpdatesWhenShowScopeDisposed()
        {
            var viewObject = new GameObject("UiTopCenter Test View", typeof(RectTransform), typeof(UiTopCenterView));
            var windowObject =
                new GameObject("UiTopCenter Test Window", typeof(RectTransform), typeof(UiTopCenterWindow));
            using var localization = new LocalizationService(SystemLanguage.English);
            using var timeProvider = new FakeTimeProvider(new DateTime(2026, 6, 26, 12, 34, 56, DateTimeKind.Utc));
            var holdTimer = new FakeHoldTimer();
            var feedback = new RecordingFeedbackCommands();
            var firstScope = new WindowPresenterShowScope();
            var secondScope = new WindowPresenterShowScope();

            try
            {
                var view = BuildView(viewObject);
                var window = windowObject.GetComponent<UiTopCenterWindow>();
                var presenter = new UiTopCenterPresenter(
                    timeProvider,
                    localization,
                    feedback,
                    holdTimer,
                    _ => view);

                presenter.Bind(window);
                presenter.Initialize();
                presenter.OnShowBegin(firstScope);

                Assert.That(view.TextLocalTime.text, Is.EqualTo("Time : 12:34:56"));
                Assert.That(view.TextPressAndHold.text, Is.EqualTo("press and hold"));

                timeProvider.SetUtcNow(new DateTime(2026, 6, 26, 23, 59, 58, DateTimeKind.Utc));
                Assert.That(view.TextLocalTime.text, Is.EqualTo("Time : 23:59:58"));

                localization.ChangeLanguage(SystemLanguage.German);
                Assert.That(view.TextLocalTime.text, Is.EqualTo("Zeit : 23:59:58"));
                Assert.That(view.TextPressAndHold.text, Is.EqualTo("drücken und halten"));

                firstScope.Dispose();
                localization.ChangeLanguage(SystemLanguage.French);
                timeProvider.SetUtcNow(new DateTime(2026, 6, 27, 1, 2, 3, DateTimeKind.Utc));

                Assert.That(view.TextLocalTime.text, Is.EqualTo("Zeit : 23:59:58"));
                Assert.That(view.TextPressAndHold.text, Is.EqualTo("drücken und halten"));

                presenter.OnShowBegin(secondScope);

                Assert.That(view.TextLocalTime.text, Is.EqualTo("Temps : 01:02:03"));
                Assert.That(view.TextPressAndHold.text, Is.EqualTo("appuyez et maintenez"));
            }
            finally
            {
                firstScope.Dispose();
                secondScope.Dispose();
                UnityEngine.Object.DestroyImmediate(windowObject);
                UnityEngine.Object.DestroyImmediate(viewObject);
            }
        }

        [Test]
        public void Presenter_HoldInputEmitsOneLocalizedHintAndCancelsPendingHold()
        {
            var viewObject = new GameObject("UiTopCenter Test View", typeof(RectTransform), typeof(UiTopCenterView));
            var windowObject =
                new GameObject("UiTopCenter Test Window", typeof(RectTransform), typeof(UiTopCenterWindow));
            using var localization = new LocalizationService(SystemLanguage.English);
            using var timeProvider = new FakeTimeProvider(new DateTime(2026, 6, 26, 12, 34, 56, DateTimeKind.Utc));
            var holdTimer = new FakeHoldTimer();
            var feedback = new RecordingFeedbackCommands();
            var showScope = new WindowPresenterShowScope();

            try
            {
                var view = BuildView(viewObject);
                var window = windowObject.GetComponent<UiTopCenterWindow>();
                var presenter = new UiTopCenterPresenter(
                    timeProvider,
                    localization,
                    feedback,
                    holdTimer,
                    _ => view);

                presenter.Bind(window);
                presenter.Initialize();
                presenter.OnShowBegin(showScope);

                view.HoldInput.OnPointerDown(null);
                Assert.That(holdTimer.ActiveCount, Is.EqualTo(1));
                holdTimer.CompleteLatest();

                Assert.That(feedback.Hints, Has.Count.EqualTo(1));
                Assert.That(feedback.Hints[0].Description,
                    Is.EqualTo("Test hint description, hint can be very large"));
                Assert.That(feedback.Hints[0].Anchor, Is.EqualTo(UiHintAnchor.Top));

                holdTimer.CompleteLatest();
                Assert.That(feedback.Hints, Has.Count.EqualTo(1));

                view.HoldInput.OnPointerUp(null);
                view.HoldInput.OnPointerDown(null);
                view.HoldInput.OnPointerUp(null);
                holdTimer.CompleteLatest();
                Assert.That(feedback.Hints, Has.Count.EqualTo(1));

                view.HoldInput.OnPointerDown(null);
                view.HoldInput.OnPointerExit(null);
                holdTimer.CompleteLatest();
                Assert.That(feedback.Hints, Has.Count.EqualTo(1));

                localization.ChangeLanguage(SystemLanguage.Russian);
                view.HoldInput.OnPointerDown(null);
                holdTimer.CompleteLatest();

                Assert.That(feedback.Hints, Has.Count.EqualTo(2));
                Assert.That(feedback.Hints[1].Description,
                    Is.EqualTo("Тестовое описание хинта, хинт может быть очень большим"));

                view.HoldInput.OnPointerUp(null);
                showScope.Dispose();
                view.HoldInput.OnPointerDown(null);
                holdTimer.CompleteLatest();

                Assert.That(feedback.Hints, Has.Count.EqualTo(2));
            }
            finally
            {
                showScope.Dispose();
                UnityEngine.Object.DestroyImmediate(windowObject);
                UnityEngine.Object.DestroyImmediate(viewObject);
            }
        }

        internal static UiTopCenterView BuildView(GameObject viewObject)
        {
            var view = viewObject.GetComponent<UiTopCenterView>();
            return UiViewTestFixtures.Configure(view);
        }

        private sealed class FakeTimeProvider : IUiTopCenterTimeProvider, IDisposable
        {
            private readonly List<Action> callbacks = new();
            private bool disposed;

            public FakeTimeProvider(DateTime utcNow)
            {
                UtcNow = utcNow;
            }

            public DateTime UtcNow { get; private set; }

            public void SetUtcNow(DateTime utcNow)
            {
                UtcNow = utcNow;
                for (var i = callbacks.Count - 1; i >= 0; i--)
                {
                    callbacks[i]?.Invoke();
                }
            }

            public IDisposable SubscribeUtcTimeChanged(Action callback)
            {
                if (disposed)
                {
                    throw new ObjectDisposedException(nameof(FakeTimeProvider));
                }

                callbacks.Add(callback);
                return new DisposableAction(() => callbacks.Remove(callback));
            }

            public void Dispose()
            {
                disposed = true;
                callbacks.Clear();
            }
        }

        private sealed class FakeHoldTimer : IUiTopCenterHoldTimer
        {
            private readonly List<FakeHold> holds = new();

            public int ActiveCount
            {
                get
                {
                    var count = 0;
                    for (var i = 0; i < holds.Count; i++)
                    {
                        if (holds[i].IsActive)
                        {
                            count++;
                        }
                    }

                    return count;
                }
            }

            public IDisposable Start(TimeSpan delay, Action completed)
            {
                var hold = new FakeHold(completed);
                holds.Add(hold);
                return hold;
            }

            public void CompleteLatest()
            {
                if (holds.Count == 0)
                {
                    return;
                }

                holds[^1].Complete();
            }
        }

        private sealed class FakeHold : IDisposable
        {
            private readonly Action completed;
            private bool disposed;
            private bool completedOnce;

            public FakeHold(Action completed)
            {
                this.completed = completed ?? throw new ArgumentNullException(nameof(completed));
            }

            public bool IsActive => !disposed && !completedOnce;

            public void Complete()
            {
                if (!IsActive)
                {
                    return;
                }

                completedOnce = true;
                completed();
            }

            public void Dispose()
            {
                disposed = true;
            }
        }

        private sealed class RecordingFeedbackCommands : IUiFeedbackCommands
        {
            public List<UiHintRequest> Hints { get; } = new();

            public void ShowHint(
                string description,
                UiHintAnchor anchor = UiHintAnchor.Center,
                float durationSeconds = 1.5f)
            {
                Hints.Add(new UiHintRequest(description, anchor, durationSeconds));
            }

            public void RequestCollectFx(int amount, UiFxTarget target = UiFxTarget.Coins)
            {
            }

            public void RequestCollectFxFrom(int amount, UiFxTarget source, UiFxTarget target = UiFxTarget.Coins)
            {
            }

            public void RequestSpendFx(int amount, UiFxTarget target = UiFxTarget.Coins)
            {
            }

            public void RequestSpendFxFrom(int amount, UiFxTarget source, UiFxTarget target = UiFxTarget.Coins)
            {
            }
        }
    }
}

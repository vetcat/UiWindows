using System;
using NUnit.Framework;
using UiWindowsMvp.UIAdapter;
using UnityEngine;
using UnityEngine.UI.Windows;

namespace UiWindowsMvp.Tests.PlayMode
{
    public sealed class WindowPresenterBindingTests
    {
        [Test]
        public void Binder_CreatesPresenterThroughFactoryAndStoresBindingOnWindow()
        {
            var gameObject = new GameObject("Presenter Binding Test Window");
            var window = gameObject.AddComponent<TestWindow>();
            var presenter = new TrackingPresenter();
            var factory = new TrackingPresenterFactory(presenter);

            try
            {
                var binding = WindowPresenterBinder.Bind(window, factory, subscribeToWindowSystemEvents: false);

                Assert.That(factory.CreatedForWindow, Is.SameAs(window));
                Assert.That(presenter.BoundWindow, Is.SameAs(window));
                Assert.That(WindowPresenterBinder.TryGetBinding(window, out var storedBinding), Is.True);
                Assert.That(storedBinding, Is.SameAs(binding));
                Assert.Throws<InvalidOperationException>(() => WindowPresenterBinder.Bind(window, new TrackingPresenter(), subscribeToWindowSystemEvents: false));

                binding.Dispose();

                Assert.That(WindowPresenterBinder.TryGetBinding(window, out _), Is.False);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(gameObject);
            }
        }

        [Test]
        public void Binding_MapsLifecycleAndDisposesShowSubscriptionOnHideEnd()
        {
            var gameObject = new GameObject("Presenter Lifecycle Test Window");
            var window = gameObject.AddComponent<TestWindow>();
            var presenter = new TrackingPresenter();

            try
            {
                using var binding = new WindowPresenterBinding<TestWindow>(window, presenter);

                binding.OnWindowInitialized();
                binding.OnWindowInitialized();
                binding.OnWindowShowBegin();
                var firstShowDisposable = presenter.LastShowDisposable;
                binding.OnWindowShowEnd();

                binding.OnWindowHideBegin();
                binding.OnWindowHideEnd();

                binding.OnWindowShowBegin();
                var secondShowDisposable = presenter.LastShowDisposable;
                binding.OnWindowHideEnd();

                Assert.That(presenter.InitializeCount, Is.EqualTo(1));
                Assert.That(presenter.ShowBeginCount, Is.EqualTo(2));
                Assert.That(presenter.ShowEndCount, Is.EqualTo(1));
                Assert.That(presenter.HideBeginCount, Is.EqualTo(1));
                Assert.That(presenter.HideEndCount, Is.EqualTo(2));
                Assert.That(firstShowDisposable, Is.Not.SameAs(secondShowDisposable));
                Assert.That(firstShowDisposable.DisposeCount, Is.EqualTo(1));
                Assert.That(secondShowDisposable.DisposeCount, Is.EqualTo(1));
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(gameObject);
            }
        }

        [Test]
        public void Binding_FinalCleanupDisposesActiveShowScopeAndPresenterOnce()
        {
            var gameObject = new GameObject("Presenter Cleanup Test Window");
            var window = gameObject.AddComponent<TestWindow>();
            var presenter = new TrackingPresenter();

            try
            {
                var binding = new WindowPresenterBinding<TestWindow>(window, presenter);

                binding.OnWindowShowBegin();
                Assert.That(presenter.LastShowDisposable.DisposeCount, Is.EqualTo(0));

                binding.Dispose();
                binding.Dispose();

                Assert.That(presenter.LastShowDisposable.DisposeCount, Is.EqualTo(1));
                Assert.That(presenter.DisposeCount, Is.EqualTo(1));
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(gameObject);
            }
        }

        private sealed class TestWindow : WindowBase
        {
        }

        private sealed class TrackingPresenterFactory : IWindowPresenterFactory<TestWindow>
        {
            private readonly TrackingPresenter presenter;

            public TrackingPresenterFactory(TrackingPresenter presenter)
            {
                this.presenter = presenter;
            }

            public TestWindow CreatedForWindow { get; private set; }

            public IWindowPresenter<TestWindow> Create(TestWindow window)
            {
                CreatedForWindow = window;
                return presenter;
            }
        }

        private sealed class TrackingPresenter : IWindowPresenter<TestWindow>
        {
            public TestWindow BoundWindow { get; private set; }

            public int InitializeCount { get; private set; }

            public int ShowBeginCount { get; private set; }

            public int ShowEndCount { get; private set; }

            public int HideBeginCount { get; private set; }

            public int HideEndCount { get; private set; }

            public int DisposeCount { get; private set; }

            public TrackingDisposable LastShowDisposable { get; private set; }

            public void Bind(TestWindow window)
            {
                BoundWindow = window;
            }

            public void Initialize()
            {
                InitializeCount++;
            }

            public void OnShowBegin(IUiShowScope showScope)
            {
                ShowBeginCount++;
                LastShowDisposable = new TrackingDisposable();
                showScope.Add(LastShowDisposable);
            }

            public void OnShowEnd()
            {
                ShowEndCount++;
            }

            public void OnHideBegin()
            {
                HideBeginCount++;
            }

            public void OnHideEnd()
            {
                HideEndCount++;
            }

            public void Dispose()
            {
                DisposeCount++;
            }
        }

        private sealed class TrackingDisposable : IDisposable
        {
            public int DisposeCount { get; private set; }

            public void Dispose()
            {
                DisposeCount++;
            }
        }
    }
}

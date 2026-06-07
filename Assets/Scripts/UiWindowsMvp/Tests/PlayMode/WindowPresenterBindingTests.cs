using System;
using System.Reflection;
using NUnit.Framework;
using UiWindowsMvp.UIAdapter;
using UnityEngine;
using UnityEngine.UI.Windows;
using UnityEngine.UI.Windows.Modules;

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
        public void BindFactory_ThrowsArgumentNullExceptionForNullWindow_BeforeFactoryCreate()
        {
            var factory = new TrackingPresenterFactory(new TrackingPresenter());

            var exception = Assert.Throws<ArgumentNullException>(
                () => WindowPresenterBinder.Bind<TestWindow>(null, factory, subscribeToWindowSystemEvents: false));

            Assert.That(exception.ParamName, Is.EqualTo("window"));
            Assert.That(factory.CreateCount, Is.EqualTo(0));
        }

        [Test]
        public void Dispose_DoesNotBreakLaterWindowEventRaise()
        {
            var windowSystemObject = CreateWindowSystemObject();
            var gameObject = new GameObject("Presenter Event Regression Test Window");
            var window = gameObject.AddComponent<TestWindow>();
            var presenter = new TrackingPresenter();

            try
            {
                var binding = WindowPresenterBinder.Bind(window, presenter);
                var lifecycleSubscription = GetLifecycleSubscription(binding);

                binding.Dispose();

                AssertSubscriptionReleasedStrongReferences(lifecycleSubscription);
                Assert.DoesNotThrow(() => WindowSystem.RaiseEvent(window, WindowEvent.OnHideEnd));
                Assert.That(presenter.HideEndCount, Is.EqualTo(0));

                WindowSystem.ClearEvents(window);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(gameObject);
                UnityEngine.Object.DestroyImmediate(windowSystemObject);
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
        public void Dispose_DisposesPresenterAndClearsAnchor_WhenShowScopeDisposableThrows()
        {
            var gameObject = new GameObject("Presenter Throwing Cleanup Test Window");
            var window = gameObject.AddComponent<TestWindow>();
            var throwingDisposable = new ThrowingTrackingDisposable();
            var presenter = new TrackingPresenter(() => throwingDisposable);

            try
            {
                var binding = WindowPresenterBinder.Bind(window, presenter, subscribeToWindowSystemEvents: false);

                binding.OnWindowShowBegin();

                var exception = Assert.Throws<AggregateException>(() => binding.Dispose());

                Assert.That(exception.InnerExceptions, Has.Count.EqualTo(1));
                Assert.That(exception.InnerExceptions[0], Is.TypeOf<InvalidOperationException>());
                Assert.That(throwingDisposable.DisposeCount, Is.EqualTo(1));
                Assert.That(presenter.DisposeCount, Is.EqualTo(1));
                Assert.That(binding.IsDisposed, Is.True);
                Assert.That(WindowPresenterBinder.TryGetBinding(window, out _), Is.False);

                Assert.DoesNotThrow(() => binding.Dispose());
                Assert.That(throwingDisposable.DisposeCount, Is.EqualTo(1));
                Assert.That(presenter.DisposeCount, Is.EqualTo(1));
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

            public int CreateCount { get; private set; }

            public IWindowPresenter<TestWindow> Create(TestWindow window)
            {
                CreateCount++;
                CreatedForWindow = window;
                return presenter;
            }
        }

        private sealed class TrackingPresenter : IWindowPresenter<TestWindow>
        {
            private readonly Func<TrackingDisposable> showDisposableFactory;

            public TrackingPresenter(Func<TrackingDisposable> showDisposableFactory = null)
            {
                this.showDisposableFactory = showDisposableFactory ?? (() => new TrackingDisposable());
            }

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
                LastShowDisposable = showDisposableFactory();
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

        private class TrackingDisposable : IDisposable
        {
            public int DisposeCount { get; private set; }

            public virtual void Dispose()
            {
                DisposeCount++;
            }
        }

        private sealed class ThrowingTrackingDisposable : TrackingDisposable
        {
            public override void Dispose()
            {
                base.Dispose();
                throw new InvalidOperationException("Intentional show-scope dispose failure.");
            }
        }

        private static GameObject CreateWindowSystemObject()
        {
            var gameObject = new GameObject("Presenter Event Regression WindowSystem");
            gameObject.SetActive(false);

            var windowSystem = gameObject.AddComponent<WindowSystem>();
            windowSystem.events = gameObject.AddComponent<WindowSystemEvents>();
            windowSystem.breadcrumbs = gameObject.AddComponent<WindowSystemBreadcrumbs>();

            gameObject.SetActive(true);
            return gameObject;
        }

        private static object GetLifecycleSubscription<TWindow>(WindowPresenterBinding<TWindow> binding)
            where TWindow : WindowBase
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            var field = typeof(WindowPresenterBinding<TWindow>).GetField("lifecycleSubscription", flags);
            Assert.That(field, Is.Not.Null);
            return field.GetValue(binding);
        }

        private static void AssertSubscriptionReleasedStrongReferences(object lifecycleSubscription)
        {
            Assert.That(lifecycleSubscription, Is.Not.Null);

            const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            var type = lifecycleSubscription.GetType();

            Assert.That(type.GetField("binding", flags)?.GetValue(lifecycleSubscription), Is.Null);
            Assert.That(type.GetField("window", flags)?.GetValue(lifecycleSubscription), Is.Null);
            Assert.That(type.GetField("events", flags)?.GetValue(lifecycleSubscription), Is.Null);
        }
    }
}

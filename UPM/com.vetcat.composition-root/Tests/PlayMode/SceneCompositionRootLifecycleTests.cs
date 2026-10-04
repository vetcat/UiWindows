using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using CompositionRoot.Runtime;
using UnityEngine;
using UnityEngine.TestTools;

namespace CompositionRoot.Tests.PlayMode
{
    public sealed class SceneCompositionRootLifecycleTests
    {
        [UnityTest]
        public IEnumerator SceneCompositionRoot_InitializesStartsAndDisposesRegisteredServicesExactlyOnce()
        {
            var gameObject = new GameObject("Test Scene Composition Root");
            gameObject.SetActive(false);
            var root = gameObject.AddComponent<SceneCompositionRoot>();
            var installer = gameObject.AddComponent<TestCompositionInstaller>();
            gameObject.SetActive(true);

            Assert.That(root.IsBootstrapped, Is.True);
            Assert.That(root.IsStarted, Is.False);
            Assert.That(installer.FirstService.StartCount, Is.Zero);
            Assert.That(installer.SecondService.StartCount, Is.Zero);

            yield return null;

            root.Bootstrap();
            root.Bootstrap();
            root.Startup();
            root.Startup();

            Assert.That(root.IsBootstrapped, Is.True);
            Assert.That(root.IsStarted, Is.True);
            Assert.That(root.Services.Resolve<IFirstTrackingService>(), Is.SameAs(installer.FirstService));
            Assert.That(root.Services.Resolve<ISecondTrackingService>(), Is.SameAs(installer.SecondService));
            Assert.That(installer.FirstService.InitializeCount, Is.EqualTo(1));
            Assert.That(installer.SecondService.InitializeCount, Is.EqualTo(1));
            Assert.That(installer.FirstService.StartCount, Is.EqualTo(1));
            Assert.That(installer.SecondService.StartCount, Is.EqualTo(1));
            Assert.That(installer.Events, Is.EqualTo(new[] { "first:init", "second:init", "first:start", "second:start" }));

            var firstService = installer.FirstService;
            var secondService = installer.SecondService;
            root.Shutdown();
            root.Shutdown();

            Assert.That(firstService.DisposeCount, Is.EqualTo(1));
            Assert.That(secondService.DisposeCount, Is.EqualTo(1));
            Assert.That(root.IsStarted, Is.False);
            Assert.That(installer.Events, Is.EqualTo(new[]
                { "first:init", "second:init", "first:start", "second:start", "second:dispose", "first:dispose" }));

            UnityEngine.Object.Destroy(gameObject);
            yield return null;

            Assert.That(firstService.DisposeCount, Is.EqualTo(1));
            Assert.That(secondService.DisposeCount, Is.EqualTo(1));
        }

        [UnityTest]
        public IEnumerator SceneCompositionRoot_FailedInitializationDisposesTemporaryRegistryAndDoesNotBootstrap()
        {
            var gameObject = new GameObject("Failing Scene Composition Root");
            gameObject.SetActive(false);
            var installer = gameObject.AddComponent<FailingCompositionInstaller>();
            var root = gameObject.AddComponent<SceneCompositionRoot>();

            var exception = Assert.Throws<InvalidOperationException>(() => root.Bootstrap());
            Assert.That(exception.Message, Is.EqualTo(FailingInitializableService.FailureMessage));
            Assert.That(root.IsBootstrapped, Is.False);
            Assert.Throws<InvalidOperationException>(() => _ = root.Services);
            Assert.That(installer.DisposableService.DisposeCount, Is.EqualTo(1));

            root.Shutdown();
            Assert.That(installer.DisposableService.DisposeCount, Is.EqualTo(1));

            UnityEngine.Object.Destroy(gameObject);
            yield return null;

            Assert.That(installer.DisposableService.DisposeCount, Is.EqualTo(1));
        }

        [UnityTest]
        public IEnumerator SceneCompositionRoot_ShutdownBeforeStartDoesNotRestartServices()
        {
            var gameObject = new GameObject("Stopped Scene Composition Root");
            gameObject.SetActive(false);
            var root = gameObject.AddComponent<SceneCompositionRoot>();
            var installer = gameObject.AddComponent<TestCompositionInstaller>();
            gameObject.SetActive(true);
            var service = installer.FirstService;

            root.Shutdown();
            yield return null;

            Assert.That(root.IsBootstrapped, Is.False);
            Assert.That(root.IsStarted, Is.False);
            Assert.That(service.StartCount, Is.Zero);
            Assert.That(service.DisposeCount, Is.EqualTo(1));
            UnityEngine.Object.Destroy(gameObject);
            yield return null;
        }

        [UnityTest]
        public IEnumerator SceneCompositionRoot_FailedStartupDisposesServicesAndClearsRoot()
        {
            var gameObject = new GameObject("Failing Startup Composition Root");
            gameObject.SetActive(false);
            var installer = gameObject.AddComponent<FailingStartupInstaller>();
            var root = gameObject.AddComponent<SceneCompositionRoot>();

            Assert.Throws<InvalidOperationException>(root.Startup);
            root.Bootstrap();
            var exception = Assert.Throws<InvalidOperationException>(root.Startup);
            Assert.That(exception.Message, Is.EqualTo(FailingStartableService.FailureMessage));
            Assert.That(root.IsBootstrapped, Is.False);
            Assert.That(root.IsStarted, Is.False);
            Assert.Throws<InvalidOperationException>(() => _ = root.Services);
            Assert.That(installer.Service.StartCount, Is.EqualTo(1));
            Assert.That(installer.Service.DisposeCount, Is.EqualTo(1));

            root.Shutdown();
            UnityEngine.Object.Destroy(gameObject);
            yield return null;
            Assert.That(installer.Service.DisposeCount, Is.EqualTo(1));
        }

        private interface IFirstTrackingService
        {
        }

        private interface ISecondTrackingService
        {
        }

        private sealed class TestCompositionInstaller : MonoBehaviour, ICompositionInstaller
        {
            public readonly List<string> Events = new List<string>();

            public FirstTrackingService FirstService { get; private set; }

            public SecondTrackingService SecondService { get; private set; }

            private bool awakeCompleted;

            private void Awake()
            {
                awakeCompleted = true;
            }

            public void Install(IServiceRegistry registry)
            {
                FirstService = new FirstTrackingService(Events, () => awakeCompleted);
                SecondService = new SecondTrackingService(Events, () => awakeCompleted);
                registry.Register<IFirstTrackingService>(FirstService);
                registry.Register(FirstService);
                registry.Register<ISecondTrackingService>(SecondService);
            }
        }

        private sealed class FailingCompositionInstaller : MonoBehaviour, ICompositionInstaller
        {
            public DisposableOnlyService DisposableService { get; private set; }

            public void Install(IServiceRegistry registry)
            {
                DisposableService = new DisposableOnlyService();
                registry.Register(DisposableService);
                registry.Register(new FailingInitializableService());
            }
        }

        private sealed class FailingStartupInstaller : MonoBehaviour, ICompositionInstaller
        {
            public FailingStartableService Service { get; private set; }

            public void Install(IServiceRegistry registry)
            {
                Service = new FailingStartableService();
                registry.Register(Service);
            }
        }

        private abstract class TrackingService : IInitializable, IStartable, IDisposable
        {
            private readonly List<string> events;
            private readonly string serviceName;
            private readonly Func<bool> isSceneAwake;

            protected TrackingService(List<string> events, string serviceName, Func<bool> isSceneAwake)
            {
                this.events = events;
                this.serviceName = serviceName;
                this.isSceneAwake = isSceneAwake;
            }

            public int InitializeCount { get; private set; }

            public int DisposeCount { get; private set; }

            public int StartCount { get; private set; }

            public void Initialize()
            {
                InitializeCount++;
                events.Add(serviceName + ":init");
            }

            public void Start()
            {
                Assert.That(isSceneAwake(), Is.True, "Service startup ran before its scene dependency's Awake.");
                StartCount++;
                events.Add(serviceName + ":start");
            }

            public void Dispose()
            {
                DisposeCount++;
                events.Add(serviceName + ":dispose");
            }
        }

        private sealed class FirstTrackingService : TrackingService, IFirstTrackingService
        {
            public FirstTrackingService(List<string> events, Func<bool> isSceneAwake)
                : base(events, "first", isSceneAwake)
            {
            }
        }

        private sealed class SecondTrackingService : TrackingService, ISecondTrackingService
        {
            public SecondTrackingService(List<string> events, Func<bool> isSceneAwake)
                : base(events, "second", isSceneAwake)
            {
            }
        }

        private sealed class DisposableOnlyService : IDisposable
        {
            public int DisposeCount { get; private set; }

            public void Dispose()
            {
                DisposeCount++;
            }
        }

        private sealed class FailingStartableService : IStartable, IDisposable
        {
            public const string FailureMessage = "Intentional composition startup failure.";
            public int StartCount { get; private set; }
            public int DisposeCount { get; private set; }

            public void Start()
            {
                StartCount++;
                throw new InvalidOperationException(FailureMessage);
            }

            public void Dispose()
            {
                DisposeCount++;
            }
        }

        private sealed class FailingInitializableService : IInitializable
        {
            public const string FailureMessage = "Intentional composition initialization failure.";

            public void Initialize()
            {
                throw new InvalidOperationException(FailureMessage);
            }
        }
    }
}

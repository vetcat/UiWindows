using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UiWindowsMvp.Runtime.Composition;
using UnityEngine;
using UnityEngine.TestTools;

namespace UiWindowsMvp.Tests.PlayMode
{
    public sealed class SceneCompositionRootLifecycleTests
    {
        [UnityTest]
        public IEnumerator SceneCompositionRoot_InitializesAndDisposesRegisteredServicesExactlyOnce()
        {
            var gameObject = new GameObject("Test Scene Composition Root");
            var installer = gameObject.AddComponent<TestCompositionInstaller>();
            var root = gameObject.AddComponent<SceneCompositionRoot>();

            yield return null;

            root.Bootstrap();
            root.Bootstrap();

            Assert.That(root.IsBootstrapped, Is.True);
            Assert.That(root.Services.Resolve<IFirstTrackingService>(), Is.SameAs(installer.FirstService));
            Assert.That(root.Services.Resolve<ISecondTrackingService>(), Is.SameAs(installer.SecondService));
            Assert.That(installer.FirstService.InitializeCount, Is.EqualTo(1));
            Assert.That(installer.SecondService.InitializeCount, Is.EqualTo(1));
            Assert.That(installer.Events, Is.EqualTo(new[] { "first:init", "second:init" }));

            var firstService = installer.FirstService;
            var secondService = installer.SecondService;
            root.Shutdown();
            root.Shutdown();

            Assert.That(firstService.DisposeCount, Is.EqualTo(1));
            Assert.That(secondService.DisposeCount, Is.EqualTo(1));
            Assert.That(installer.Events, Is.EqualTo(new[] { "first:init", "second:init", "second:dispose", "first:dispose" }));

            UnityEngine.Object.Destroy(gameObject);
            yield return null;

            Assert.That(firstService.DisposeCount, Is.EqualTo(1));
            Assert.That(secondService.DisposeCount, Is.EqualTo(1));
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

            public void Install(IServiceRegistry registry)
            {
                FirstService = new FirstTrackingService(Events);
                SecondService = new SecondTrackingService(Events);
                registry.Register<IFirstTrackingService>(FirstService);
                registry.Register<ISecondTrackingService>(SecondService);
            }
        }

        private abstract class TrackingService : IInitializable, IDisposable
        {
            private readonly List<string> events;
            private readonly string serviceName;

            protected TrackingService(List<string> events, string serviceName)
            {
                this.events = events;
                this.serviceName = serviceName;
            }

            public int InitializeCount { get; private set; }

            public int DisposeCount { get; private set; }

            public void Initialize()
            {
                InitializeCount++;
                events.Add(serviceName + ":init");
            }

            public void Dispose()
            {
                DisposeCount++;
                events.Add(serviceName + ":dispose");
            }
        }

        private sealed class FirstTrackingService : TrackingService, IFirstTrackingService
        {
            public FirstTrackingService(List<string> events)
                : base(events, "first")
            {
            }
        }

        private sealed class SecondTrackingService : TrackingService, ISecondTrackingService
        {
            public SecondTrackingService(List<string> events)
                : base(events, "second")
            {
            }
        }
    }
}

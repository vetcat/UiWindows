#if UNITY_EDITOR
using System.Collections;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI.Windows;
using Object = UnityEngine.Object;

namespace UiWindowsMvp.Tests.PlayMode
{
    public sealed class WindowSystemOwnershipTests
    {
        [UnityTest]
        public IEnumerator Shutdown_ReleasesOwnershipOnceBeforeDeferredDestroyAndPreservesReplacement()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(
                "Packages/com.me.ui.windows/Runtime/WindowSystem.prefab");
            var module = ScriptableObject.CreateInstance<TrackingShutdownModule>();
            WindowSystem first = null;
            WindowSystem second = null;
            try
            {
                first = Object.Instantiate(prefab).GetComponent<WindowSystem>();
                first.modules.Clear();
                first.modules.Add(module);
                yield return null;
                Assert.That(module.StartCount, Is.EqualTo(1));
                Assert.That(WindowSystem.GetPools(), Is.SameAs(first.pools));

                first.Shutdown();
                first.Shutdown();
                Assert.That(WindowSystem.HasInstance(), Is.False);
                Assert.That(first.enabled, Is.False);
                Assert.That(module.DestroyCount, Is.EqualTo(1));

                second = Object.Instantiate(prefab).GetComponent<WindowSystem>();
                Object.Destroy(first.gameObject);
                yield return null;

                Assert.That(first == null, Is.True);
                Assert.That(module.DestroyCount, Is.EqualTo(1));
                Assert.That(WindowSystem.HasInstance(), Is.True);
                Assert.That(WindowSystem.GetPools(), Is.SameAs(second.pools),
                    "Deferred destruction of the previous owner cleared the replacement singleton.");
            }
            finally
            {
                if (first != null) Object.DestroyImmediate(first.gameObject);
                if (second != null) Object.DestroyImmediate(second.gameObject);
                Object.DestroyImmediate(module);
            }
        }

        private sealed class TrackingShutdownModule : WindowSystemModule
        {
            public int StartCount { get; private set; }
            public int DestroyCount { get; private set; }

            public override void OnStart() => StartCount++;
            public override void OnDestroy() => DestroyCount++;
        }
    }
}
#endif

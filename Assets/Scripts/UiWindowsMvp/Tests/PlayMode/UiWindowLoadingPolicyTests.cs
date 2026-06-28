using System;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using CompositionRoot.Runtime;
using NUnit.Framework;
using ProjectContext.Localization;
using ProjectContext.Settings;
using ProjectContext.UiRequests;
using UiWindowsMvp.SampleSceneWindows;
using UiWindowsMvp.UIAdapter;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI.Windows;
using UnityEngine.UI.Windows.WindowTypes;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace UiWindowsMvp.Tests.PlayMode
{
    public sealed class UiWindowLoadingPolicyTests
    {
        [UnityTest]
        public IEnumerator SampleScene_RecordsEditorFirstShowAndWarmReopenPolicyEvidence()
        {
            ClearPersistentSampleState();
            DestroyWindowSystemsImmediate();
            yield return null;

            var startupStopwatch = Stopwatch.StartNew();
            SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
            yield return null;

            var root = Object.FindFirstObjectByType<SceneCompositionRoot>();
            Assert.That(root, Is.Not.Null);
            Assert.That(root.IsBootstrapped, Is.True);

            var topLeftLauncher = root.Services.Resolve<UiTopLeftDemoLauncher>();
            var topRightLauncher = root.Services.Resolve<UiTopRightDemoLauncher>();
            var topCenterLauncher = root.Services.Resolve<UiTopCenterDemoLauncher>();
            var downRightLauncher = root.Services.Resolve<UiDownRightDemoLauncher>();
            var downLeftLauncher = root.Services.Resolve<UiDownLeftDemoLauncher>();
            var hintsLauncher = root.Services.Resolve<UiHintsDemoLauncher>();
            var fxLauncher = root.Services.Resolve<UiFxDemoLauncher>();
            var objectIndicatorLauncher = root.Services.Resolve<UiObjectIndicatorDemoLauncher>();
            var settingsLauncher = root.Services.Resolve<UiSettingsDemoLauncher>();
            var shopLauncher = root.Services.Resolve<UiShopDemoLauncher>();
            var modalLauncher = root.Services.Resolve<UiModalDemoLauncher>();
            var modal = root.Services.Resolve<IUiModalService>();

            var topLeftFactory = root.Services.Resolve<UiTopLeftPresenterFactory>();
            var topRightFactory = root.Services.Resolve<UiTopRightPresenterFactory>();
            var topCenterFactory = root.Services.Resolve<UiTopCenterPresenterFactory>();
            var downRightFactory = root.Services.Resolve<UiDownRightPresenterFactory>();
            var downLeftFactory = root.Services.Resolve<UiDownLeftPresenterFactory>();
            var hintsFactory = root.Services.Resolve<UiHintsPresenterFactory>();
            var fxFactory = root.Services.Resolve<UiFxPresenterFactory>();
            var objectIndicatorFactory = root.Services.Resolve<UiObjectIndicatorPresenterFactory>();
            var settingsFactory = root.Services.Resolve<UiSettingsPresenterFactory>();
            var shopFactory = root.Services.Resolve<UiShopPresenterFactory>();
            var modalFactory = root.Services.Resolve<UiModalPresenterFactory>();

            try
            {
                yield return WaitUntil(
                    () =>
                        IsShown(topLeftLauncher.CurrentWindow) &&
                        IsShown(topRightLauncher.CurrentWindow) &&
                        IsShown(topCenterLauncher.CurrentWindow) &&
                        IsShown(downRightLauncher.CurrentWindow) &&
                        IsShown(downLeftLauncher.CurrentWindow) &&
                        IsShown(hintsLauncher.CurrentWindow) &&
                        IsShown(fxLauncher.CurrentWindow) &&
                        IsShown(objectIndicatorLauncher.CurrentWindow),
                    "default SampleScene auto-start windows");
                startupStopwatch.Stop();

                Debug.Log(
                    "UIW-29 Editor loading metric | family=SampleSceneAutoStartBatch | phase=scene_load_to_shown | " +
                    $"elapsed_ms={FormatMilliseconds(startupStopwatch)} | " +
                    "includes=TopLeftHud,TopRightHud,TopCenterHud,DownRightLauncher,DownLeftLauncher,HintsOverlay," +
                    "FxOverlay,ObjectIndicator | evidence=EditorPlayModeStopwatch");

                Assert.That(topLeftFactory.CreatedCount, Is.EqualTo(1));
                Assert.That(topRightFactory.CreatedCount, Is.EqualTo(1));
                Assert.That(topCenterFactory.CreatedCount, Is.EqualTo(1));
                Assert.That(downRightFactory.CreatedCount, Is.EqualTo(1));
                Assert.That(downLeftFactory.CreatedCount, Is.EqualTo(1));
                Assert.That(hintsFactory.CreatedCount, Is.EqualTo(1));
                Assert.That(fxFactory.CreatedCount, Is.EqualTo(1));
                Assert.That(objectIndicatorFactory.CreatedCount, Is.EqualTo(1));
                Assert.That(settingsFactory.CreatedCount, Is.Zero);
                Assert.That(shopFactory.CreatedCount, Is.Zero);
                Assert.That(modalFactory.CreatedCount, Is.Zero);

                yield return MeasureWarmReopen(
                    "TopLeftHud",
                    () => topLeftLauncher.CurrentWindow,
                    topLeftLauncher.Show,
                    topLeftLauncher.Hide);
                yield return MeasureWarmReopen(
                    "TopRightHud",
                    () => topRightLauncher.CurrentWindow,
                    topRightLauncher.Show,
                    topRightLauncher.Hide);
                yield return MeasureWarmReopen(
                    "TopCenterHud",
                    () => topCenterLauncher.CurrentWindow,
                    topCenterLauncher.Show,
                    topCenterLauncher.Hide);
                yield return MeasureWarmReopen(
                    "DownRightLauncher",
                    () => downRightLauncher.CurrentWindow,
                    downRightLauncher.Show,
                    downRightLauncher.Hide);
                yield return MeasureWarmReopen(
                    "DownLeftLauncher",
                    () => downLeftLauncher.CurrentWindow,
                    downLeftLauncher.Show,
                    downLeftLauncher.Hide);
                yield return MeasureWarmReopen(
                    "HintsOverlay",
                    () => hintsLauncher.CurrentWindow,
                    hintsLauncher.Show,
                    hintsLauncher.Hide);
                yield return MeasureWarmReopen(
                    "FxOverlay",
                    () => fxLauncher.CurrentWindow,
                    fxLauncher.Show,
                    fxLauncher.Hide);
                yield return MeasureWarmReopen(
                    "ObjectIndicator",
                    () => objectIndicatorLauncher.CurrentWindow,
                    objectIndicatorLauncher.Show,
                    objectIndicatorLauncher.Hide);

                yield return MeasureColdFirstShowAndWarmReopen(
                    "Settings",
                    () => settingsLauncher.CurrentWindow,
                    settingsLauncher.Show,
                    settingsLauncher.Hide);
                yield return MeasureColdFirstShowAndWarmReopen(
                    "Modal",
                    () => modalLauncher.CurrentWindow,
                    () => modal.ShowInfoOk("Loading policy", "Editor-only first-show measurement"),
                    modal.Clear);
                yield return MeasureColdFirstShowAndWarmReopen(
                    "Shop",
                    () => shopLauncher.CurrentWindow,
                    shopLauncher.Show,
                    shopLauncher.Hide);

                Assert.That(settingsFactory.CreatedCount, Is.EqualTo(1));
                Assert.That(shopFactory.CreatedCount, Is.EqualTo(1));
                Assert.That(modalFactory.CreatedCount, Is.EqualTo(1));
            }
            finally
            {
                modal.Clear();
                topLeftLauncher?.Dispose();
                topRightLauncher?.Dispose();
                topCenterLauncher?.Dispose();
                downRightLauncher?.Dispose();
                downLeftLauncher?.Dispose();
                hintsLauncher?.Dispose();
                fxLauncher?.Dispose();
                objectIndicatorLauncher?.Dispose();
                settingsLauncher?.Dispose();
                shopLauncher?.Dispose();
                modalLauncher?.Dispose();

                ClearPersistentSampleState();
                DestroyWindowSystemsImmediate();
            }
        }

        private static IEnumerator MeasureColdFirstShowAndWarmReopen<TWindow>(
            string family,
            Func<TWindow> getWindow,
            Action show,
            Action hide)
            where TWindow : LayoutWindowType
        {
            Assert.That(getWindow(), Is.Null, $"{family} should not have a loaded window before cold first show.");

            var managedMemoryBefore = GC.GetTotalMemory(false);
            var stopwatch = Stopwatch.StartNew();
            show();
            yield return WaitUntil(() => IsShown(getWindow()), $"{family} cold first show");
            stopwatch.Stop();

            var window = getWindow();
            Assert.That(window, Is.Not.Null);
            Assert.That(window.createPool, Is.True);
            Assert.That(WindowPresenterBinder.TryGetBinding(window, out IWindowPresenterBinding<TWindow> binding),
                Is.True);

            LogMetric(
                family,
                "cold_first_show",
                stopwatch,
                GC.GetTotalMemory(false) - managedMemoryBefore,
                sameInstance: true,
                sameBinding: true);

            yield return MeasureWarmReopen(family, getWindow, show, hide, window, binding);
        }

        private static IEnumerator MeasureWarmReopen<TWindow>(
            string family,
            Func<TWindow> getWindow,
            Action show,
            Action hide)
            where TWindow : LayoutWindowType
        {
            var window = getWindow();
            Assert.That(window, Is.Not.Null);
            Assert.That(window.createPool, Is.True);
            Assert.That(WindowPresenterBinder.TryGetBinding(window, out IWindowPresenterBinding<TWindow> binding),
                Is.True);

            yield return MeasureWarmReopen(family, getWindow, show, hide, window, binding);
        }

        private static IEnumerator MeasureWarmReopen<TWindow>(
            string family,
            Func<TWindow> getWindow,
            Action show,
            Action hide,
            TWindow initialWindow,
            IWindowPresenterBinding<TWindow> initialBinding)
            where TWindow : LayoutWindowType
        {
            hide();
            yield return WaitUntil(() => IsHidden(initialWindow), $"{family} hidden before warm reopen");

            var managedMemoryBefore = GC.GetTotalMemory(false);
            var stopwatch = Stopwatch.StartNew();
            show();
            yield return WaitUntil(() => IsShown(getWindow()), $"{family} warm reopen");
            stopwatch.Stop();

            var reopenedWindow = getWindow();
            Assert.That(reopenedWindow, Is.SameAs(initialWindow));
            Assert.That(WindowPresenterBinder.TryGetBinding(reopenedWindow, out var reopenedBinding), Is.True);
            Assert.That(reopenedBinding, Is.SameAs(initialBinding));

            LogMetric(
                family,
                "warm_reopen",
                stopwatch,
                GC.GetTotalMemory(false) - managedMemoryBefore,
                sameInstance: true,
                sameBinding: true);
        }

        private static void LogMetric(
            string family,
            string phase,
            Stopwatch stopwatch,
            long managedMemoryDeltaBytes,
            bool sameInstance,
            bool sameBinding)
        {
            Debug.Log(
                $"UIW-29 Editor loading metric | family={family} | phase={phase} | " +
                $"elapsed_ms={FormatMilliseconds(stopwatch)} | " +
                $"managed_memory_delta_bytes={managedMemoryDeltaBytes} | " +
                $"same_instance={sameInstance} | same_binding={sameBinding} | " +
                "evidence=EditorPlayModeStopwatchAndManagedMemoryDelta");
        }

        private static string FormatMilliseconds(Stopwatch stopwatch)
        {
            return stopwatch.Elapsed.TotalMilliseconds.ToString("0.###", CultureInfo.InvariantCulture);
        }

        private static IEnumerator WaitUntil(Func<bool> predicate, string description)
        {
            const int MaxFrames = 240;
            for (var frame = 0; frame < MaxFrames; frame++)
            {
                if (predicate())
                {
                    yield break;
                }

                yield return null;
            }

            Assert.Fail($"Timed out waiting for {description}.");
        }

        private static bool IsShown(WindowBase window)
        {
            return window != null && window.GetState() == ObjectState.Shown;
        }

        private static bool IsHidden(WindowBase window)
        {
            return window == null || window.GetState() == ObjectState.Hidden;
        }

        private static void ClearPersistentSampleState()
        {
            PlayerPrefs.DeleteKey(GameSettingsService.MusicVolumeKey);
            PlayerPrefs.DeleteKey(GameSettingsService.SoundVolumeKey);
            PlayerPrefs.DeleteKey(LocalizationService.LanguageKey);
        }

        private static void DestroyWindowSystemsImmediate()
        {
            DestroyObjectsImmediate<UiObjectIndicatorWindow>();
            DestroyObjectsImmediate<UiModalWindow>();
            DestroyObjectsImmediate<UiHintsWindow>();
            DestroyObjectsImmediate<UiFxWindow>();
            DestroyObjectsImmediate<UiTopCenterWindow>();
            DestroyObjectsImmediate<UiDownLeftWindow>();
            DestroyObjectsImmediate<UiDownRightWindow>();
            DestroyObjectsImmediate<UiTopRightWindow>();
            DestroyObjectsImmediate<UiShopWindow>();
            DestroyObjectsImmediate<UiSettingsWindow>();
            DestroyObjectsImmediate<UiTopLeftWindow>();
            DestroyObjectsImmediate<WindowLayout>();
            DestroyObjectsImmediate<WindowSystem>();
        }

        private static void DestroyObjectsImmediate<T>()
            where T : Component
        {
            var objects = Object.FindObjectsByType<T>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            for (var i = 0; i < objects.Length; i++)
            {
                if (objects[i] != null)
                {
                    Object.DestroyImmediate(objects[i].gameObject);
                }
            }
        }
    }
}

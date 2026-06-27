using System.Collections;
using System.Reflection;
using CompositionRoot.Runtime;
using NUnit.Framework;
using ProjectContext.Localization;
using ProjectContext.Player;
using ProjectContext.Settings;
using ProjectContext.UiRequests;
using UiWindowsMvp.SampleSceneWindows;
using UiWindowsMvp.UIAdapter;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI.Windows;

namespace UiWindowsMvp.Tests.PlayMode
{
    public sealed class SampleSceneIntegratedAcceptanceTests
    {
        [UnityTest]
        public IEnumerator SampleScene_RunIntegratedMigratedWorkflowAndRepresentativeShowScopeCleanup()
        {
            ClearPersistentSampleState();
            DestroyWindowSystemsImmediate();
            yield return null;

            SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
            yield return null;

            var root = Object.FindFirstObjectByType<SceneCompositionRoot>();
            Assert.That(root, Is.Not.Null);
            Assert.That(root.IsBootstrapped, Is.True);

            var player = root.Services.Resolve<IPlayerService>();
            var settings = root.Services.Resolve<IGameSettingsService>();
            var localization = root.Services.Resolve<ILocalizationService>();
            var feedback = root.Services.Resolve<IUiFeedbackService>();
            var fxTargetResolver = root.Services.Resolve<IUiFxTargetResolver>();
            var objectTarget = root.Services.Resolve<UiObjectIndicatorDemoTarget>();

            var topLeftLauncher = root.Services.Resolve<UiTopLeftDemoLauncher>();
            var topRightLauncher = root.Services.Resolve<UiTopRightDemoLauncher>();
            var topCenterLauncher = root.Services.Resolve<UiTopCenterDemoLauncher>();
            var downRightLauncher = root.Services.Resolve<UiDownRightDemoLauncher>();
            var downLeftLauncher = root.Services.Resolve<UiDownLeftDemoLauncher>();
            var settingsLauncher = root.Services.Resolve<UiSettingsDemoLauncher>();
            var shopLauncher = root.Services.Resolve<UiShopDemoLauncher>();
            var modalLauncher = root.Services.Resolve<UiModalDemoLauncher>();
            var hintsLauncher = root.Services.Resolve<UiHintsDemoLauncher>();
            var fxLauncher = root.Services.Resolve<UiFxDemoLauncher>();
            var objectIndicatorLauncher = root.Services.Resolve<UiObjectIndicatorDemoLauncher>();

            var topLeftFactory = root.Services.Resolve<UiTopLeftPresenterFactory>();
            var topRightFactory = root.Services.Resolve<UiTopRightPresenterFactory>();
            var topCenterFactory = root.Services.Resolve<UiTopCenterPresenterFactory>();
            var downRightFactory = root.Services.Resolve<UiDownRightPresenterFactory>();
            var downLeftFactory = root.Services.Resolve<UiDownLeftPresenterFactory>();
            var settingsFactory = root.Services.Resolve<UiSettingsPresenterFactory>();
            var shopFactory = root.Services.Resolve<UiShopPresenterFactory>();
            var modalFactory = root.Services.Resolve<UiModalPresenterFactory>();
            var hintsFactory = root.Services.Resolve<UiHintsPresenterFactory>();
            var fxFactory = root.Services.Resolve<UiFxPresenterFactory>();
            var objectIndicatorFactory = root.Services.Resolve<UiObjectIndicatorPresenterFactory>();

            UiTopLeftWindow topLeftWindow = null;
            UiTopRightWindow topRightWindow = null;
            UiTopCenterWindow topCenterWindow = null;
            UiDownRightWindow downRightWindow = null;
            UiDownLeftWindow downLeftWindow = null;
            UiSettingsWindow settingsWindow = null;
            UiShopWindow shopWindow = null;
            UiModalWindow modalWindow = null;
            UiHintsWindow hintsWindow = null;
            UiFxWindow fxWindow = null;
            UiObjectIndicatorWindow objectIndicatorWindow = null;

            IWindowPresenterBinding<UiTopLeftWindow> topLeftBinding = null;
            IWindowPresenterBinding<UiTopRightWindow> topRightBinding = null;
            IWindowPresenterBinding<UiTopCenterWindow> topCenterBinding = null;
            IWindowPresenterBinding<UiDownRightWindow> downRightBinding = null;
            IWindowPresenterBinding<UiDownLeftWindow> downLeftBinding = null;
            IWindowPresenterBinding<UiSettingsWindow> settingsBinding = null;
            IWindowPresenterBinding<UiShopWindow> shopBinding = null;
            IWindowPresenterBinding<UiModalWindow> modalBinding = null;
            IWindowPresenterBinding<UiHintsWindow> hintsBinding = null;
            IWindowPresenterBinding<UiFxWindow> fxBinding = null;
            IWindowPresenterBinding<UiObjectIndicatorWindow> objectIndicatorBinding = null;

            var cleaned = false;

            try
            {
                localization.ChangeLanguage(SystemLanguage.English);
                yield return null;

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
                    "auto-started SampleScene windows");

                topLeftWindow = topLeftLauncher.CurrentWindow;
                topRightWindow = topRightLauncher.CurrentWindow;
                topCenterWindow = topCenterLauncher.CurrentWindow;
                downRightWindow = downRightLauncher.CurrentWindow;
                downLeftWindow = downLeftLauncher.CurrentWindow;
                hintsWindow = hintsLauncher.CurrentWindow;
                fxWindow = fxLauncher.CurrentWindow;
                objectIndicatorWindow = objectIndicatorLauncher.CurrentWindow;

                Assert.That(topLeftWindow.createPool, Is.True);
                Assert.That(topRightWindow.createPool, Is.True);
                Assert.That(topCenterWindow.createPool, Is.True);
                Assert.That(downRightWindow.createPool, Is.True);
                Assert.That(downLeftWindow.createPool, Is.True);
                Assert.That(hintsWindow.createPool, Is.True);
                Assert.That(fxWindow.createPool, Is.True);
                Assert.That(objectIndicatorWindow.createPool, Is.True);

                Assert.That(WindowPresenterBinder.TryGetBinding(topLeftWindow, out topLeftBinding), Is.True);
                Assert.That(WindowPresenterBinder.TryGetBinding(topRightWindow, out topRightBinding), Is.True);
                Assert.That(WindowPresenterBinder.TryGetBinding(topCenterWindow, out topCenterBinding), Is.True);
                Assert.That(WindowPresenterBinder.TryGetBinding(downRightWindow, out downRightBinding), Is.True);
                Assert.That(WindowPresenterBinder.TryGetBinding(downLeftWindow, out downLeftBinding), Is.True);
                Assert.That(WindowPresenterBinder.TryGetBinding(hintsWindow, out hintsBinding), Is.True);
                Assert.That(WindowPresenterBinder.TryGetBinding(fxWindow, out fxBinding), Is.True);
                Assert.That(
                    WindowPresenterBinder.TryGetBinding(objectIndicatorWindow, out objectIndicatorBinding),
                    Is.True);

                Assert.That(topLeftFactory.CreatedCount, Is.EqualTo(1));
                Assert.That(topRightFactory.CreatedCount, Is.EqualTo(1));
                Assert.That(topCenterFactory.CreatedCount, Is.EqualTo(1));
                Assert.That(downRightFactory.CreatedCount, Is.EqualTo(1));
                Assert.That(downLeftFactory.CreatedCount, Is.EqualTo(1));
                Assert.That(hintsFactory.CreatedCount, Is.EqualTo(1));
                Assert.That(fxFactory.CreatedCount, Is.EqualTo(1));
                Assert.That(objectIndicatorFactory.CreatedCount, Is.EqualTo(1));

                Assert.That(topLeftWindow.TryGetView(out var topLeftView), Is.True);
                Assert.That(topRightWindow.TryGetView(out var topRightView), Is.True);
                Assert.That(topCenterWindow.TryGetView(out var topCenterView), Is.True);
                Assert.That(downRightWindow.TryGetView(out var downRightView), Is.True);
                Assert.That(downLeftWindow.TryGetView(out var downLeftView), Is.True);
                Assert.That(hintsWindow.TryGetView(out var hintsView), Is.True);
                Assert.That(fxWindow.TryGetView(out var fxView), Is.True);
                Assert.That(objectIndicatorWindow.TryGetView(out var objectIndicatorView), Is.True);

                var topRightInstanceId = topRightWindow.GetInstanceID();
                var topCenterInstanceId = topCenterWindow.GetInstanceID();
                var downRightInstanceId = downRightWindow.GetInstanceID();
                var downLeftInstanceId = downLeftWindow.GetInstanceID();
                var hintsInstanceId = hintsWindow.GetInstanceID();
                var fxInstanceId = fxWindow.GetInstanceID();
                var objectIndicatorInstanceId = objectIndicatorWindow.GetInstanceID();

                player.SetHealth(82);
                player.SetCoins(123);
                yield return null;

                Assert.That(topLeftView.HealthData.TextValue.text, Is.EqualTo("82 / 100"));
                Assert.That(topRightView.TextCoinsAmount.text, Is.EqualTo("123"));
                Assert.That(topCenterView.TextLocalTime.text, Does.StartWith("Time : "));
                Assert.That(downRightView.TextSettings.text, Is.EqualTo("Settings"));
                Assert.That(downLeftView.TextItemsShop.text, Is.EqualTo("Items Shop"));

                topCenterView.HoldInput.OnPointerDown(null);
                yield return new WaitForSeconds(0.65f);
                Assert.That(hintsView.LastDescription,
                    Is.EqualTo("Test hint description, hint can be very large"));
                topCenterView.HoldInput.OnPointerUp(null);

                downRightView.ButtonSettings.onClick.Invoke();
                yield return WaitUntil(
                    () => IsShown(settingsLauncher.CurrentWindow),
                    "settings opened through down-right launcher");

                settingsWindow = settingsLauncher.CurrentWindow;
                Assert.That(settingsWindow.createPool, Is.True);
                Assert.That(WindowPresenterBinder.TryGetBinding(settingsWindow, out settingsBinding), Is.True);
                Assert.That(settingsFactory.CreatedCount, Is.EqualTo(1));
                Assert.That(settingsWindow.TryGetView(out var settingsView), Is.True);
                Assert.That(settingsView.TextHeader.text, Is.EqualTo("Settings"));

                settingsView.SettingsLayout.SliderMusicVolume.value = 0.42f;
                Assert.That(GetCurrentValue<float>(settings, "MusicVolume"), Is.EqualTo(0.42f).Within(0.0001f));
                settingsView.ButtonClose.onClick.Invoke();
                yield return WaitUntil(() => settingsWindow.GetState() == ObjectState.Hidden, "settings hidden");

                downLeftView.ButtonItemsShop.onClick.Invoke();
                yield return WaitUntil(
                    () => IsShown(shopLauncher.CurrentWindow) && downLeftWindow.GetState() == ObjectState.Hidden,
                    "shop opened through down-left launcher and launcher hidden");

                shopWindow = shopLauncher.CurrentWindow;
                Assert.That(shopWindow.createPool, Is.True);
                Assert.That(WindowPresenterBinder.TryGetBinding(shopWindow, out shopBinding), Is.True);
                Assert.That(shopFactory.CreatedCount, Is.EqualTo(1));
                Assert.That(downLeftFactory.CreatedCount, Is.EqualTo(1));
                Assert.That(shopWindow.TryGetView(out var shopView), Is.True);
                Assert.That(shopView.ShopItems, Has.Count.GreaterThan(1));

                shopView.ShopItems[0].ButtonItem.onClick.Invoke();
                yield return WaitUntil(
                    () => IsShown(modalLauncher.CurrentWindow),
                    "modal opened from shop item");

                modalWindow = modalLauncher.CurrentWindow;
                Assert.That(WindowPresenterBinder.TryGetBinding(modalWindow, out modalBinding), Is.True);
                Assert.That(modalFactory.CreatedCount, Is.EqualTo(1));
                Assert.That(modalWindow.TryGetView(out var modalView), Is.True);
                Assert.That(modalView.TextCaption.text, Is.EqualTo("Choice of item"));

                modalView.ButtonOk.onClick.Invoke();
                yield return WaitUntil(() => modalWindow.GetState() == ObjectState.Hidden, "modal hidden");

                shopView.ButtonClose.onClick.Invoke();
                yield return WaitUntil(() => shopWindow.GetState() == ObjectState.Hidden, "shop hidden");
                yield return WaitUntil(
                    () => IsShown(downLeftLauncher.CurrentWindow),
                    "down-left launcher restored after shop hide");

                Assert.That(downLeftLauncher.CurrentWindow.GetInstanceID(), Is.EqualTo(downLeftInstanceId));
                Assert.That(WindowPresenterBinder.TryGetBinding(downLeftLauncher.CurrentWindow, out var downLeftAgain),
                    Is.True);
                Assert.That(downLeftAgain, Is.SameAs(downLeftBinding));
                Assert.That(downLeftFactory.CreatedCount, Is.EqualTo(1));

                objectTarget.MoveTo(new Vector3(2f, 0f, 4f));
                yield return null;
                yield return null;

                Assert.That(objectIndicatorView.IsIndicatorVisible, Is.True);
                Assert.That(fxTargetResolver.TryGetTarget(UiFxTarget.Coins, out var coinTarget), Is.True);
                Assert.That(coinTarget, Is.SameAs(topRightView.CoinIconRectTransform));
                Assert.That(fxTargetResolver.TryGetTarget(UiFxTarget.CharacterReward, out var rewardTarget), Is.True);
                Assert.That(rewardTarget, Is.SameAs(objectIndicatorView.RewardAnchor));

                var coinsBeforeReward = GetCurrentValue<int>(player, "Coins");
                objectIndicatorView.ButtonAction.onClick.Invoke();
                Assert.That(
                    GetCurrentValue<int>(player, "Coins"),
                    Is.EqualTo(coinsBeforeReward + objectTarget.RewardAmount));
                Assert.That(fxView.LastFxText, Is.EqualTo("+" + objectTarget.RewardAmount));
                Assert.That(topRightView.TextCoinsAmount.text,
                    Is.EqualTo((coinsBeforeReward + objectTarget.RewardAmount).ToString()));

                topRightLauncher.Hide();
                yield return WaitUntil(() => topRightWindow.GetState() == ObjectState.Hidden, "top-right hidden");
                Assert.That(fxTargetResolver.TryGetTarget(UiFxTarget.Coins, out _), Is.False);

                var hiddenTopRightText = topRightView.TextCoinsAmount.text;
                player.SetCoins(500);
                yield return null;
                Assert.That(topRightView.TextCoinsAmount.text, Is.EqualTo(hiddenTopRightText));

                player.AddCoinsWithFx(3);
                Assert.That(fxView.LastFxText, Is.EqualTo("+3"));

                topRightLauncher.Show();
                yield return WaitUntil(() => IsShown(topRightLauncher.CurrentWindow), "top-right reopened");
                Assert.That(topRightLauncher.CurrentWindow.GetInstanceID(), Is.EqualTo(topRightInstanceId));
                Assert.That(WindowPresenterBinder.TryGetBinding(topRightLauncher.CurrentWindow, out var topRightAgain),
                    Is.True);
                Assert.That(topRightAgain, Is.SameAs(topRightBinding));
                Assert.That(topRightFactory.CreatedCount, Is.EqualTo(1));
                Assert.That(topRightView.TextCoinsAmount.text, Is.EqualTo("503"));

                var visibleHintDescription = hintsView.LastDescription;
                var visibleFxText = fxView.LastFxText;
                hintsLauncher.Hide();
                fxLauncher.Hide();
                yield return WaitUntil(
                    () => hintsWindow.GetState() == ObjectState.Hidden && fxWindow.GetState() == ObjectState.Hidden,
                    "feedback overlays hidden");

                feedback.ShowHint("Hidden integrated hint", UiHintAnchor.Bottom, 0.1f);
                player.AddCoinsWithFx(4);
                Assert.That(hintsView.LastDescription, Is.EqualTo(visibleHintDescription));
                Assert.That(fxView.LastFxText, Is.EqualTo(visibleFxText));
                Assert.That(fxView.ActiveFxCount, Is.Zero);

                hintsLauncher.Show();
                fxLauncher.Show();
                yield return WaitUntil(
                    () => IsShown(hintsLauncher.CurrentWindow) && IsShown(fxLauncher.CurrentWindow),
                    "feedback overlays reopened");
                Assert.That(hintsLauncher.CurrentWindow.GetInstanceID(), Is.EqualTo(hintsInstanceId));
                Assert.That(fxLauncher.CurrentWindow.GetInstanceID(), Is.EqualTo(fxInstanceId));
                Assert.That(WindowPresenterBinder.TryGetBinding(hintsLauncher.CurrentWindow, out var hintsAgain),
                    Is.True);
                Assert.That(WindowPresenterBinder.TryGetBinding(fxLauncher.CurrentWindow, out var fxAgain), Is.True);
                Assert.That(hintsAgain, Is.SameAs(hintsBinding));
                Assert.That(fxAgain, Is.SameAs(fxBinding));
                Assert.That(hintsFactory.CreatedCount, Is.EqualTo(1));
                Assert.That(fxFactory.CreatedCount, Is.EqualTo(1));

                objectIndicatorLauncher.Hide();
                yield return WaitUntil(
                    () => objectIndicatorWindow.GetState() == ObjectState.Hidden,
                    "object indicator hidden");
                Assert.That(fxTargetResolver.TryGetTarget(UiFxTarget.CharacterReward, out _), Is.False);
                var coinsBeforeHiddenReward = GetCurrentValue<int>(player, "Coins");
                objectIndicatorView.ButtonAction.onClick.Invoke();
                Assert.That(GetCurrentValue<int>(player, "Coins"), Is.EqualTo(coinsBeforeHiddenReward));

                objectIndicatorLauncher.Show();
                yield return WaitUntil(
                    () => IsShown(objectIndicatorLauncher.CurrentWindow),
                    "object indicator reopened");
                Assert.That(objectIndicatorLauncher.CurrentWindow.GetInstanceID(),
                    Is.EqualTo(objectIndicatorInstanceId));
                Assert.That(
                    WindowPresenterBinder.TryGetBinding(
                        objectIndicatorLauncher.CurrentWindow,
                        out var objectIndicatorAgain),
                    Is.True);
                Assert.That(objectIndicatorAgain, Is.SameAs(objectIndicatorBinding));
                Assert.That(objectIndicatorFactory.CreatedCount, Is.EqualTo(1));

                topCenterLauncher.Hide();
                yield return WaitUntil(
                    () => topCenterWindow.GetState() == ObjectState.Hidden,
                    "top-center hidden");
                var hintBeforeHiddenHold = hintsView.LastDescription;
                topCenterView.HoldInput.OnPointerDown(null);
                yield return new WaitForSeconds(0.65f);
                Assert.That(hintsView.LastDescription, Is.EqualTo(hintBeforeHiddenHold));

                topCenterLauncher.Show();
                yield return WaitUntil(() => IsShown(topCenterLauncher.CurrentWindow), "top-center reopened");
                Assert.That(topCenterLauncher.CurrentWindow.GetInstanceID(), Is.EqualTo(topCenterInstanceId));
                Assert.That(
                    WindowPresenterBinder.TryGetBinding(topCenterLauncher.CurrentWindow, out var topCenterAgain),
                    Is.True);
                Assert.That(topCenterAgain, Is.SameAs(topCenterBinding));
                Assert.That(topCenterFactory.CreatedCount, Is.EqualTo(1));

                downRightLauncher.Hide();
                yield return WaitUntil(() => downRightWindow.GetState() == ObjectState.Hidden, "down-right hidden");
                downRightView.ButtonSettings.onClick.Invoke();
                yield return null;
                Assert.That(settingsWindow.GetState(), Is.EqualTo(ObjectState.Hidden));

                downRightLauncher.Show();
                yield return WaitUntil(() => IsShown(downRightLauncher.CurrentWindow), "down-right reopened");
                Assert.That(downRightLauncher.CurrentWindow.GetInstanceID(), Is.EqualTo(downRightInstanceId));
                Assert.That(
                    WindowPresenterBinder.TryGetBinding(downRightLauncher.CurrentWindow, out var downRightAgain),
                    Is.True);
                Assert.That(downRightAgain, Is.SameAs(downRightBinding));
                Assert.That(downRightFactory.CreatedCount, Is.EqualTo(1));

                topLeftLauncher.Hide();
                topRightLauncher.Hide();
                topCenterLauncher.Hide();
                downRightLauncher.Hide();
                downLeftLauncher.Hide();
                settingsLauncher.Hide();
                shopLauncher.Hide();
                modalLauncher.Hide();
                hintsLauncher.Hide();
                fxLauncher.Hide();
                objectIndicatorLauncher.Hide();
                yield return WaitUntil(
                    () =>
                        IsHidden(topLeftWindow) &&
                        IsHidden(topRightWindow) &&
                        IsHidden(topCenterWindow) &&
                        IsHidden(downRightWindow) &&
                        IsHidden(downLeftLauncher.CurrentWindow) &&
                        IsHidden(settingsWindow) &&
                        IsHidden(shopWindow) &&
                        IsHidden(modalWindow) &&
                        IsHidden(hintsWindow) &&
                        IsHidden(fxWindow) &&
                        IsHidden(objectIndicatorWindow),
                    "all integrated SampleScene windows hidden before cleanup");

                CleanHidden(topLeftWindow);
                CleanHidden(topRightWindow);
                CleanHidden(topCenterWindow);
                CleanHidden(downRightWindow);
                CleanHidden(downLeftLauncher.CurrentWindow);
                CleanHidden(settingsWindow);
                CleanHidden(shopWindow);
                CleanHidden(modalWindow);
                CleanHidden(hintsWindow);
                CleanHidden(fxWindow);
                CleanHidden(objectIndicatorWindow);
                cleaned = true;

                Assert.That(topLeftBinding.IsDisposed, Is.True);
                Assert.That(topRightBinding.IsDisposed, Is.True);
                Assert.That(topCenterBinding.IsDisposed, Is.True);
                Assert.That(downRightBinding.IsDisposed, Is.True);
                Assert.That(downLeftBinding.IsDisposed, Is.True);
                Assert.That(settingsBinding.IsDisposed, Is.True);
                Assert.That(shopBinding.IsDisposed, Is.True);
                Assert.That(modalBinding.IsDisposed, Is.True);
                Assert.That(hintsBinding.IsDisposed, Is.True);
                Assert.That(fxBinding.IsDisposed, Is.True);
                Assert.That(objectIndicatorBinding.IsDisposed, Is.True);
            }
            finally
            {
                topLeftLauncher?.Dispose();
                topRightLauncher?.Dispose();
                topCenterLauncher?.Dispose();
                downRightLauncher?.Dispose();
                downLeftLauncher?.Dispose();
                settingsLauncher?.Dispose();
                shopLauncher?.Dispose();
                modalLauncher?.Dispose();
                hintsLauncher?.Dispose();
                fxLauncher?.Dispose();
                objectIndicatorLauncher?.Dispose();

                if (!cleaned)
                {
                    DestroyWindowSystemsImmediate();
                }

                ClearPersistentSampleState();
            }
        }

        private static IEnumerator WaitUntil(System.Func<bool> predicate, string description)
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

        private static void CleanHidden(WindowBase window)
        {
            if (window != null && window.GetState() == ObjectState.Hidden)
            {
                WindowSystem.Clean(window);
            }
        }

        private static T GetCurrentValue<T>(object owner, string propertyName)
        {
            var property = owner.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            Assert.That(property, Is.Not.Null);
            var reactiveSurface = property.GetValue(owner);
            var currentValueProperty =
                reactiveSurface.GetType().GetProperty("CurrentValue", BindingFlags.Public | BindingFlags.Instance);
            Assert.That(currentValueProperty, Is.Not.Null);
            return (T)currentValueProperty.GetValue(reactiveSurface);
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

using System.Collections;
using CompositionRoot.Runtime;
using NUnit.Framework;
using ProjectContext.Localization;
using ProjectContext.Shop;
using ProjectContext.UiRequests;
using UiWindowsMvp.SampleSceneWindows;
using UiWindowsMvp.UIAdapter;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI.Windows;

namespace UiWindowsMvp.Tests.PlayMode
{
    public sealed class UiDownLeftWindowLifecycleTests
    {
        [UnityTest]
        public IEnumerator LauncherOpensShop_HidesWhileShopVisible_AndItemClickShowsModalOncePerCycle()
        {
            PlayerPrefs.DeleteKey(LocalizationService.LanguageKey);
            DestroyWindowSystemsImmediate();
            yield return null;

            SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
            yield return null;

            var root = Object.FindFirstObjectByType<SceneCompositionRoot>();
            Assert.That(root, Is.Not.Null);
            Assert.That(root.IsBootstrapped, Is.True);

            var launcher = root.Services.Resolve<UiDownLeftDemoLauncher>();
            var shopLauncher = root.Services.Resolve<UiShopDemoLauncher>();
            var modalLauncher = root.Services.Resolve<UiModalDemoLauncher>();
            var modal = root.Services.Resolve<IUiModalService>();
            var localization = root.Services.Resolve<ILocalizationService>();
            var presenterFactory = root.Services.Resolve<UiDownLeftPresenterFactory>();
            var shopPresenterFactory = root.Services.Resolve<UiShopPresenterFactory>();
            UiDownLeftWindow window = null;
            UiShopWindow shopWindow = null;
            UiModalWindow modalWindow = null;
            IWindowPresenterBinding<UiDownLeftWindow> binding = null;
            IWindowPresenterBinding<UiShopWindow> shopBinding = null;
            var cleanedDownLeft = false;
            var cleanedShop = false;
            var lastModalId = 0;

            try
            {
                yield return WaitUntil(
                    () => launcher.CurrentWindow != null && launcher.CurrentWindow.GetState() == ObjectState.Shown,
                    "initial UiDownLeft window show");

                localization.ChangeLanguage(SystemLanguage.English);
                yield return null;

                window = launcher.CurrentWindow;
                Assert.That(window.createPool, Is.True);
                Assert.That(WindowPresenterBinder.TryGetBinding(window, out binding), Is.True);
                Assert.That(presenterFactory.CreatedCount, Is.EqualTo(1));
                Assert.That(window.TryGetView(out var view), Is.True);
                Assert.That(view.TextItemsShop.text, Is.EqualTo("Items Shop"));

                var initialInstanceId = window.GetInstanceID();
                var initialView = view;

                for (var cycle = 0; cycle < 3; cycle++)
                {
                    Assert.That(window.GetState(), Is.EqualTo(ObjectState.Shown));
                    Assert.That(window.TryGetView(out view), Is.True);
                    Assert.That(view, Is.SameAs(initialView));

                    view.ButtonItemsShop.onClick.Invoke();

                    yield return WaitUntil(
                        () => shopLauncher.CurrentWindow != null &&
                              shopLauncher.CurrentWindow.GetState() == ObjectState.Shown,
                        $"shop opened from UiDownLeft cycle {cycle}");
                    yield return WaitUntil(
                        () => launcher.CurrentWindow != null &&
                              launcher.CurrentWindow.GetState() == ObjectState.Hidden,
                        $"UiDownLeft hidden while shop shown cycle {cycle}");

                    shopWindow ??= shopLauncher.CurrentWindow;
                    Assert.That(shopLauncher.CurrentWindow, Is.SameAs(shopWindow));
                    Assert.That(shopWindow.createPool, Is.True);
                    Assert.That(WindowPresenterBinder.TryGetBinding(shopWindow, out var currentShopBinding), Is.True);
                    shopBinding ??= currentShopBinding;
                    Assert.That(currentShopBinding, Is.SameAs(shopBinding));
                    Assert.That(shopPresenterFactory.CreatedCount, Is.EqualTo(1));
                    Assert.That(presenterFactory.CreatedCount, Is.EqualTo(1));

                    Assert.That(shopWindow.TryGetView(out var shopView), Is.True);
                    Assert.That(shopView.ShopItems, Has.Count.GreaterThan(1));

                    shopView.ShopItems[cycle % shopView.ShopItems.Count].ButtonItem.onClick.Invoke();
                    yield return WaitUntil(
                        () =>
                        {
                            var request = GetCurrentModal(modal);
                            return request != null &&
                                   request.Id == lastModalId + 1 &&
                                   modalLauncher.CurrentWindow != null &&
                                   modalLauncher.CurrentWindow.GetState() == ObjectState.Shown;
                        },
                        $"modal opened from shop item cycle {cycle}");

                    lastModalId = GetCurrentModal(modal).Id;
                    modalWindow = modalLauncher.CurrentWindow;
                    Assert.That(modalWindow.TryGetView(out var modalView), Is.True);
                    Assert.That(modalView.TextCaption.text, Is.EqualTo("Choice of item"));
                    Assert.That(modalView.TextDescription.text, Does.StartWith("The player selected item"));

                    modalView.ButtonOk.onClick.Invoke();
                    yield return WaitUntil(() => modalWindow.GetState() == ObjectState.Hidden,
                        $"modal hidden cycle {cycle}");

                    shopView.ButtonClose.onClick.Invoke();
                    yield return WaitUntil(() => shopWindow.GetState() == ObjectState.Hidden,
                        $"shop hidden cycle {cycle}");
                    yield return WaitUntil(
                        () => launcher.CurrentWindow != null &&
                              launcher.CurrentWindow.GetState() == ObjectState.Shown,
                        $"UiDownLeft shown again cycle {cycle}");

                    Assert.That(launcher.CurrentWindow.GetInstanceID(), Is.EqualTo(initialInstanceId));
                    Assert.That(WindowPresenterBinder.TryGetBinding(launcher.CurrentWindow, out var reopenedBinding),
                        Is.True);
                    Assert.That(reopenedBinding, Is.SameAs(binding));
                    Assert.That(presenterFactory.CreatedCount, Is.EqualTo(1));
                }

                launcher.Hide();
                yield return WaitUntil(() => window.GetState() == ObjectState.Hidden, "final UiDownLeft hide");

                WindowSystem.Clean(window);
                cleanedDownLeft = true;

                WindowSystem.Clean(shopWindow);
                cleanedShop = true;

                Assert.That(binding.IsDisposed, Is.True);
                Assert.That(shopBinding.IsDisposed, Is.True);
            }
            finally
            {
                modalLauncher?.Dispose();
                shopLauncher?.Dispose();
                launcher?.Dispose();

                CleanHidden(modalWindow);
                if (!cleanedShop)
                {
                    CleanHidden(shopWindow);
                }

                if (!cleanedDownLeft)
                {
                    CleanHidden(window);
                }

                PlayerPrefs.DeleteKey(LocalizationService.LanguageKey);
                DestroyWindowSystemsImmediate();
            }
        }

        private static IEnumerator WaitUntil(System.Func<bool> predicate, string description)
        {
            const int MaxFrames = 180;
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

        private static void CleanHidden(WindowBase window)
        {
            if (window != null && window.GetState() == ObjectState.Hidden)
            {
                WindowSystem.Clean(window);
            }
        }

        private static UiModalRequest GetCurrentModal(IUiModalReadModel readModel)
        {
            var property = readModel.GetType().GetProperty(
                "CurrentModal",
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            Assert.That(property, Is.Not.Null);
            var reactiveSurface = property.GetValue(readModel);
            var currentValueProperty =
                reactiveSurface.GetType().GetProperty(
                    "CurrentValue",
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            Assert.That(currentValueProperty, Is.Not.Null);
            return (UiModalRequest)currentValueProperty.GetValue(reactiveSurface);
        }

        private static void DestroyWindowSystemsImmediate()
        {
            DestroyObjectsImmediate<UiObjectIndicatorWindow>();
            DestroyObjectsImmediate<UiModalWindow>();
            DestroyObjectsImmediate<UiHintsWindow>();
            DestroyObjectsImmediate<UiFxWindow>();
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

using System.Collections;
using CompositionRoot.Runtime;
using NUnit.Framework;
using ProjectContext.Localization;
using ProjectContext.Shop;
using UiWindowsMvp.SampleSceneWindows;
using UiWindowsMvp.UIAdapter;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI.Windows;

namespace UiWindowsMvp.Tests.PlayMode
{
    public sealed class UiShopWindowLifecycleTests
    {
        [UnityTest]
        public IEnumerator ReopenCyclesThroughWindowSystem_ReusePooledWindowWithoutDuplicateCollectionHandlers()
        {
            PlayerPrefs.DeleteKey(LocalizationService.LanguageKey);
            DestroyWindowSystemsImmediate();
            yield return null;

            SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
            yield return null;

            var root = Object.FindFirstObjectByType<SceneCompositionRoot>();
            Assert.That(root, Is.Not.Null);
            Assert.That(root.IsBootstrapped, Is.True);

            var launcher = root.Services.Resolve<UiShopDemoLauncher>();
            var uiTopLeftLauncher = root.Services.Resolve<UiTopLeftDemoLauncher>();
            var shop = root.Services.Resolve<IShopService>();
            var presenterFactory = root.Services.Resolve<UiShopPresenterFactory>();
            UiShopWindow window = null;
            IWindowPresenterBinding<UiShopWindow> binding = null;
            var cleaned = false;

            try
            {
                yield return WaitUntil(
                    () => uiTopLeftLauncher.CurrentWindow != null &&
                          uiTopLeftLauncher.CurrentWindow.GetState() == ObjectState.Shown,
                    "initial UiTopLeft window show before shop isolation");
                var uiTopLeftWindow = uiTopLeftLauncher.CurrentWindow;
                uiTopLeftLauncher.Hide();
                yield return WaitUntil(() => uiTopLeftWindow.GetState() == ObjectState.Hidden,
                    "hide initial UiTopLeft before shop isolation");
                WindowSystem.Clean(uiTopLeftWindow);

                launcher.Show();
                yield return WaitUntil(
                    () => launcher.CurrentWindow != null && launcher.CurrentWindow.GetState() == ObjectState.Shown,
                    "initial UiShop window show");

                window = launcher.CurrentWindow;
                Assert.That(window.createPool, Is.True);
                Assert.That(WindowPresenterBinder.TryGetBinding(window, out binding), Is.True);
                Assert.That(presenterFactory.CreatedCount, Is.EqualTo(1));
                Assert.That(window.TryGetView(out var view), Is.True);

                var initialInstanceId = window.GetInstanceID();
                var initialView = view;

                for (var cycle = 0; cycle < 3; cycle++)
                {
                    shop.SelectItem(ShopItemType.Item_2);
                    yield return null;

                    Assert.That(window.TryGetView(out view), Is.True);
                    Assert.That(view, Is.SameAs(initialView));
                    Assert.That(view.ShopItems, Has.Count.EqualTo(8));
                    Assert.That(view.DetailsLayout.TextName.text, Is.EqualTo("Item name 2"));

                    view.GroupItems[1].Toggle.isOn = true;
                    Assert.That(UiShopPresenterTests.GetCurrentValue<ShopItemGroup>(shop, "SelectedGroup"),
                        Is.EqualTo(ShopItemGroup.Group_2));
                    Assert.That(view.ShopItems, Has.Count.EqualTo(2));

                    var staleButton = view.ShopItems[0].ButtonItem;
                    view.ShopItems[1].ButtonItem.onClick.Invoke();
                    Assert.That(UiShopPresenterTests.GetCurrentValue<ShopItemData>(shop, "SelectedItem").Type,
                        Is.EqualTo(ShopItemType.Item_10));
                    Assert.That(view.DetailsLayout.TextName.text, Is.EqualTo("Item name 10"));

                    launcher.Hide();
                    yield return WaitUntil(() => window.GetState() == ObjectState.Hidden, $"hide cycle {cycle}");

                    shop.SelectItem(ShopItemType.Item_3);
                    Assert.That(view.DetailsLayout.TextName.text, Is.EqualTo("Item name 10"));

                    staleButton.onClick.Invoke();
                    Assert.That(UiShopPresenterTests.GetCurrentValue<ShopItemData>(shop, "SelectedItem").Type,
                        Is.EqualTo(ShopItemType.Item_3));

                    launcher.Show();
                    yield return WaitUntil(
                        () => launcher.CurrentWindow != null && launcher.CurrentWindow.GetState() == ObjectState.Shown,
                        $"reopen cycle {cycle}");

                    Assert.That(launcher.CurrentWindow.GetInstanceID(), Is.EqualTo(initialInstanceId));
                    Assert.That(WindowPresenterBinder.TryGetBinding(launcher.CurrentWindow, out var reopenedBinding),
                        Is.True);
                    Assert.That(reopenedBinding, Is.SameAs(binding));
                    Assert.That(presenterFactory.CreatedCount, Is.EqualTo(1));
                    Assert.That(view.ShopItems, Has.Count.EqualTo(8));
                    Assert.That(view.DetailsLayout.TextName.text, Is.EqualTo("Item name 3"));
                }

                launcher.Hide();
                yield return WaitUntil(() => window.GetState() == ObjectState.Hidden, "final hide before clean");

                WindowSystem.Clean(window);
                cleaned = true;

                Assert.That(binding.IsDisposed, Is.True);
                Assert.DoesNotThrow(() => binding.Dispose());
            }
            finally
            {
                launcher?.Dispose();
                uiTopLeftLauncher?.Dispose();

                if (!cleaned && window != null && window.GetState() == ObjectState.Hidden)
                {
                    WindowSystem.Clean(window);
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

        private static void DestroyWindowSystemsImmediate()
        {
            DestroyObjectsImmediate<UiDownRightWindow>();
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

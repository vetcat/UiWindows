using System.Collections.Generic;
using NUnit.Framework;
using ProjectContext.Localization;
using ProjectContext.Shop;
using ProjectContext.UiRequests;
using UiWindowsMvp.SampleSceneWindows;
using UiWindowsMvp.UIAdapter;
using UnityEngine;

namespace UiWindowsMvp.Tests.PlayMode
{
    public sealed class UiShopPresenterTests
    {
        [Test]
        public void Presenter_BuildsGroupsItemsAndUpdatesDetailsSelection()
        {
            var viewObject = new GameObject("UiShop Test View", typeof(RectTransform), typeof(UiShopView));
            var windowObject = new GameObject("UiShop Test Window", typeof(RectTransform), typeof(UiShopWindow));
            using var shop = new ShopService();
            using var localization = new LocalizationService(SystemLanguage.English);
            using var showScope = new WindowPresenterShowScope();

            try
            {
                var view = BuildView(viewObject);
                var window = windowObject.GetComponent<UiShopWindow>();
                var presenter = new UiShopPresenter(shop, shop, localization, _ => view);

                presenter.Bind(window);
                presenter.Initialize();
                presenter.OnShowBegin(showScope);

                Assert.That(view.TextHeader.text, Is.EqualTo("Items Shop"));
                Assert.That(view.GroupItems, Has.Count.EqualTo(2));
                Assert.That(view.ShopItems, Has.Count.EqualTo(8));
                Assert.That(view.DetailsLayout.TextName.text, Is.EqualTo("Item name 1"));
                Assert.That(view.DetailsLayout.TextAmount.text, Is.EqualTo("Amount: 10"));

                view.GroupItems[1].Toggle.isOn = true;

                Assert.That(GetCurrentValue<ShopItemGroup>(shop, "SelectedGroup"), Is.EqualTo(ShopItemGroup.Group_2));
                Assert.That(view.TextItemsType.text, Is.EqualTo("Group 2"));
                Assert.That(view.ShopItems, Has.Count.EqualTo(2));
                Assert.That(view.DetailsLayout.TextName.text, Is.EqualTo("Item name 9"));
                Assert.That(view.PooledShopItemCount, Is.EqualTo(6));

                view.ShopItems[1].ButtonItem.onClick.Invoke();

                Assert.That(GetCurrentValue<ShopItemData>(shop, "SelectedItem").Type, Is.EqualTo(ShopItemType.Item_10));
                Assert.That(view.DetailsLayout.TextName.text, Is.EqualTo("Item name 10"));
                Assert.That(view.DetailsLayout.TextAmount.text, Is.EqualTo("Amount: 100"));

                localization.ChangeLanguage(SystemLanguage.German);

                Assert.That(view.TextHeader.text, Is.EqualTo("Gegenstandsladen"));
                Assert.That(view.DetailsLayout.TextName.text, Is.EqualTo("Gegenstand 10"));
            }
            finally
            {
                Object.DestroyImmediate(windowObject);
                Object.DestroyImmediate(viewObject);
            }
        }

        [Test]
        public void Presenter_CleansEntryHandlersOnHideAndDoesNotDuplicateAfterReopen()
        {
            var viewObject = new GameObject("UiShop Test View", typeof(RectTransform), typeof(UiShopView));
            var windowObject = new GameObject("UiShop Test Window", typeof(RectTransform), typeof(UiShopWindow));
            using var shop = new ShopService();
            using var localization = new LocalizationService(SystemLanguage.English);
            var commands = new TrackingShopCommands(shop);
            var firstShowScope = new WindowPresenterShowScope();
            var secondShowScope = new WindowPresenterShowScope();

            try
            {
                var view = BuildView(viewObject);
                var window = windowObject.GetComponent<UiShopWindow>();
                var presenter = new UiShopPresenter(shop, commands, localization, _ => view);

                presenter.Bind(window);
                presenter.Initialize();
                presenter.OnShowBegin(firstShowScope);

                view.ShopItems[1].ButtonItem.onClick.Invoke();
                Assert.That(commands.SelectItemCalls, Is.EqualTo(1));
                Assert.That(GetCurrentValue<ShopItemData>(shop, "SelectedItem").Type, Is.EqualTo(ShopItemType.Item_2));

                var oldButton = view.ShopItems[2].ButtonItem;
                presenter.OnHideBegin();
                firstShowScope.Dispose();

                oldButton.onClick.Invoke();
                shop.SelectItem(ShopItemType.Item_4);

                Assert.That(commands.SelectItemCalls, Is.EqualTo(1));
                Assert.That(view.DetailsLayout.TextName.text, Is.EqualTo("Item name 2"));

                presenter.OnShowBegin(secondShowScope);
                Assert.That(view.ShopItems, Has.Count.EqualTo(8));

                view.GroupItems[1].Toggle.isOn = true;
                Assert.That(commands.SelectGroupCalls, Is.EqualTo(1));
                Assert.That(view.ShopItems, Has.Count.EqualTo(2));

                view.ShopItems[1].ButtonItem.onClick.Invoke();
                Assert.That(commands.SelectItemCalls, Is.EqualTo(2));
                Assert.That(GetCurrentValue<ShopItemData>(shop, "SelectedItem").Type, Is.EqualTo(ShopItemType.Item_10));
            }
            finally
            {
                firstShowScope.Dispose();
                secondShowScope.Dispose();
                Object.DestroyImmediate(windowObject);
                Object.DestroyImmediate(viewObject);
            }
        }

        [Test]
        public void Presenter_ClickingItemRequestsModalAndUpdatesVisibilityThroughPorts()
        {
            var viewObject = new GameObject("UiShop Test View", typeof(RectTransform), typeof(UiShopView));
            var windowObject = new GameObject("UiShop Test Window", typeof(RectTransform), typeof(UiShopWindow));
            using var shop = new ShopService();
            using var localization = new LocalizationService(SystemLanguage.English);
            using var visibility = new UiShopVisibilityState();
            var modal = new TrackingUiModalCommands();
            using var showScope = new WindowPresenterShowScope();

            try
            {
                var view = BuildView(viewObject);
                var window = windowObject.GetComponent<UiShopWindow>();
                var presenter = new UiShopPresenter(shop, shop, localization, modal, visibility, _ => view);

                presenter.Bind(window);
                presenter.Initialize();

                Assert.That(GetCurrentValue<bool>(visibility, "IsShopVisible"), Is.False);

                presenter.OnShowBegin(showScope);

                Assert.That(GetCurrentValue<bool>(visibility, "IsShopVisible"), Is.True);

                view.ShopItems[1].ButtonItem.onClick.Invoke();

                Assert.That(GetCurrentValue<ShopItemData>(shop, "SelectedItem").Type, Is.EqualTo(ShopItemType.Item_2));
                Assert.That(modal.ShowInfoOkCalls, Is.EqualTo(1));
                Assert.That(modal.LastCaption, Is.EqualTo("Choice of item"));
                Assert.That(modal.LastDescription, Is.EqualTo("The player selected item Item name 2"));

                presenter.OnHideBegin();
                showScope.Dispose();
                presenter.OnHideEnd();

                Assert.That(GetCurrentValue<bool>(visibility, "IsShopVisible"), Is.False);
            }
            finally
            {
                Object.DestroyImmediate(windowObject);
                Object.DestroyImmediate(viewObject);
            }
        }

        [Test]
        public void Presenter_CleanupToleratesDisposedVisibilityPortDuringTeardown()
        {
            var viewObject = new GameObject("UiShop Test View", typeof(RectTransform), typeof(UiShopView));
            var windowObject = new GameObject("UiShop Test Window", typeof(RectTransform), typeof(UiShopWindow));
            using var shop = new ShopService();
            using var localization = new LocalizationService(SystemLanguage.English);
            var visibility = new UiShopVisibilityState();
            using var showScope = new WindowPresenterShowScope();

            try
            {
                var view = BuildView(viewObject);
                var window = windowObject.GetComponent<UiShopWindow>();
                var presenter = new UiShopPresenter(shop, shop, localization, null, visibility, _ => view);

                presenter.Bind(window);
                presenter.Initialize();
                presenter.OnShowBegin(showScope);
                presenter.OnHideBegin();

                visibility.Dispose();

                Assert.DoesNotThrow(() => presenter.OnHideEnd());
                showScope.Dispose();
                Assert.DoesNotThrow(() => presenter.Dispose());
                Assert.DoesNotThrow(() => presenter.Dispose());
            }
            finally
            {
                visibility.Dispose();
                Object.DestroyImmediate(windowObject);
                Object.DestroyImmediate(viewObject);
            }
        }

        [Test]
        public void View_RebuildShopItems_PreservesActiveRowsAndPoolsOnlySurplusForLargeLists()
        {
            var viewObject = new GameObject("UiShop Large List Test View", typeof(RectTransform), typeof(UiShopView));

            try
            {
                var view = BuildView(viewObject);
                var initialItems = CreateItems(32, 101);
                var replacementItems = CreateItems(32, 201);
                var smallerItems = CreateItems(10, 301);

                var firstBuild = view.RebuildShopItems(initialItems);
                var firstRows = new ShopItemView[firstBuild.Count];
                var firstRowIds = new HashSet<int>();
                for (var i = 0; i < firstBuild.Count; i++)
                {
                    firstRows[i] = firstBuild[i];
                    firstRowIds.Add(firstBuild[i].GetInstanceID());
                }

                Assert.That(firstRows, Has.Length.EqualTo(32));
                Assert.That(view.PooledShopItemCount, Is.Zero);

                var sameSizeRebuild = view.RebuildShopItems(replacementItems);
                Assert.That(sameSizeRebuild, Has.Count.EqualTo(32));
                Assert.That(view.PooledShopItemCount, Is.Zero);
                for (var i = 0; i < sameSizeRebuild.Count; i++)
                {
                    Assert.That(sameSizeRebuild[i], Is.SameAs(firstRows[i]));
                    Assert.That(sameSizeRebuild[i].Type, Is.EqualTo(replacementItems[i].Type));
                }

                var smallerRebuild = view.RebuildShopItems(smallerItems);
                Assert.That(smallerRebuild, Has.Count.EqualTo(10));
                Assert.That(view.PooledShopItemCount, Is.EqualTo(22));
                for (var i = 0; i < smallerRebuild.Count; i++)
                {
                    Assert.That(smallerRebuild[i], Is.SameAs(firstRows[i]));
                    Assert.That(smallerRebuild[i].Type, Is.EqualTo(smallerItems[i].Type));
                }

                var growBackRebuild = view.RebuildShopItems(replacementItems);
                Assert.That(growBackRebuild, Has.Count.EqualTo(32));
                Assert.That(view.PooledShopItemCount, Is.Zero);
                for (var i = 0; i < growBackRebuild.Count; i++)
                {
                    Assert.That(firstRowIds.Contains(growBackRebuild[i].GetInstanceID()), Is.True);
                }
            }
            finally
            {
                Object.DestroyImmediate(viewObject);
            }
        }

        internal static UiShopView BuildView(GameObject viewObject)
        {
            var view = viewObject.GetComponent<UiShopView>();
            return UiViewTestFixtures.Configure(view);
        }

        internal static T GetCurrentValue<T>(object owner, string propertyName)
        {
            var property = owner.GetType().GetProperty(
                propertyName,
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            Assert.That(property, Is.Not.Null);
            var reactiveSurface = property.GetValue(owner);
            var currentValueProperty =
                reactiveSurface.GetType().GetProperty(
                    "CurrentValue",
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            Assert.That(currentValueProperty, Is.Not.Null);
            return (T)currentValueProperty.GetValue(reactiveSurface);
        }

        private static IReadOnlyList<ShopItemData> CreateItems(int count, int firstTypeValue)
        {
            var items = new ShopItemData[count];
            for (var i = 0; i < items.Length; i++)
            {
                items[i] = new ShopItemData(
                    (ShopItemType)(firstTypeValue + i),
                    ShopItemGroup.Group_1,
                    (i + 1) * 10);
            }

            return items;
        }

        private sealed class TrackingShopCommands : IShopCommands
        {
            private readonly IShopCommands inner;

            public TrackingShopCommands(IShopCommands inner)
            {
                this.inner = inner;
            }

            public int SelectGroupCalls { get; private set; }
            public int SelectItemCalls { get; private set; }

            public void SelectGroup(ShopItemGroup group)
            {
                SelectGroupCalls++;
                inner.SelectGroup(group);
            }

            public void SelectItem(ShopItemType type)
            {
                SelectItemCalls++;
                inner.SelectItem(type);
            }
        }

        private sealed class TrackingUiModalCommands : IUiModalCommands
        {
            public int ShowInfoOkCalls { get; private set; }
            public string LastCaption { get; private set; }
            public string LastDescription { get; private set; }

            public void ShowInfoOk(string caption, string description, System.Action handlerClose = null)
            {
                ShowInfoOkCalls++;
                LastCaption = caption;
                LastDescription = description;
            }

            public void ShowInfoOkCancel(
                string caption,
                string description,
                System.Action handlerOk = null,
                System.Action handlerCancel = null)
            {
            }

            public void ShowWait(string caption = "")
            {
            }

            public void HideWait()
            {
            }

            public void CompleteCurrent(UiModalResult result)
            {
            }

            public void Clear()
            {
            }
        }
    }
}

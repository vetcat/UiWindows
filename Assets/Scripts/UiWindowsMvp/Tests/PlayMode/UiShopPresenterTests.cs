using NUnit.Framework;
using ProjectContext.Localization;
using ProjectContext.Shop;
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

        internal static UiShopView BuildView(GameObject viewObject)
        {
            var view = viewObject.GetComponent<UiShopView>();
            view.EnsureLayout();
            return view;
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
    }
}

using System;
using System.Collections.Generic;
using NUnit.Framework;
using ProjectContext.Shop;

namespace ProjectContext.Player.Tests.PlayMode
{
    public sealed class ShopServiceTests
    {
        [Test]
        public void DefaultCatalog_ExposesGroupsItemsAndInitialSelection()
        {
            using var service = new ShopService();

            Assert.That(service.Groups, Is.EqualTo(new[] { ShopItemGroup.Group_1, ShopItemGroup.Group_2 }));
            Assert.That(service.Items.Count, Is.EqualTo(10));
            Assert.That(GetCurrentValue<ShopItemGroup>(service, "SelectedGroup"), Is.EqualTo(ShopItemGroup.Group_1));
            Assert.That(GetCurrentValue<IReadOnlyList<ShopItemData>>(service, "ItemsInSelectedGroup").Count,
                Is.EqualTo(8));
            Assert.That(GetCurrentValue<ShopItemData>(service, "SelectedItem").Type, Is.EqualTo(ShopItemType.Item_1));
        }

        [Test]
        public void SelectGroupAndItem_PublishReadOnlyState()
        {
            using var service = new ShopService();
            var selectedGroups = new List<ShopItemGroup>();
            var visibleItemCounts = new List<int>();
            var selectedItems = new List<ShopItemType>();

            using var groupSubscription = Subscribe<ShopItemGroup>(service, "SelectedGroup", selectedGroups.Add);
            using var itemsSubscription =
                Subscribe<IReadOnlyList<ShopItemData>>(
                    service,
                    "ItemsInSelectedGroup",
                    items => visibleItemCounts.Add(items.Count));
            using var itemSubscription =
                Subscribe<ShopItemData>(
                    service,
                    "SelectedItem",
                    item => selectedItems.Add(item?.Type ?? ShopItemType.None));

            service.SelectGroup(ShopItemGroup.Group_2);
            service.SelectItem(ShopItemType.Item_10);

            Assert.That(selectedGroups, Is.EqualTo(new[] { ShopItemGroup.Group_1, ShopItemGroup.Group_2 }));
            Assert.That(visibleItemCounts, Is.EqualTo(new[] { 8, 2 }));
            Assert.That(selectedItems,
                Is.EqualTo(new[] { ShopItemType.Item_1, ShopItemType.Item_9, ShopItemType.Item_10 }));
            Assert.That(GetCurrentValue<ShopItemData>(service, "SelectedItem").Amount, Is.EqualTo(100));
        }

        [Test]
        public void InvalidCommands_DoNotMutateStateAndDisposeRejectsFurtherCommands()
        {
            using var service = new ShopService();

            service.SelectGroup(ShopItemGroup.None);
            service.SelectItem(ShopItemType.None);

            Assert.That(GetCurrentValue<ShopItemGroup>(service, "SelectedGroup"), Is.EqualTo(ShopItemGroup.Group_1));
            Assert.That(GetCurrentValue<ShopItemData>(service, "SelectedItem").Type, Is.EqualTo(ShopItemType.Item_1));

            service.Dispose();

            Assert.Throws<System.ObjectDisposedException>(() => service.SelectGroup(ShopItemGroup.Group_2));
            Assert.Throws<System.ObjectDisposedException>(() => service.SelectItem(ShopItemType.Item_2));
        }

        private static IDisposable Subscribe<T>(object service, string propertyName, System.Action<T> onNext)
        {
            var observable = GetReactiveSurface(service, propertyName);
            var subscribeExtensions = observable.GetType().Assembly.GetType("R3.ObservableSubscribeExtensions");
            Assert.That(subscribeExtensions, Is.Not.Null, "R3 ObservableSubscribeExtensions must be available.");

            foreach (var method in subscribeExtensions.GetMethods(System.Reflection.BindingFlags.Public |
                                                                  System.Reflection.BindingFlags.Static))
            {
                if (!method.IsGenericMethodDefinition || method.Name != "Subscribe")
                {
                    continue;
                }

                var parameters = method.GetParameters();
                if (method.GetGenericArguments().Length == 1 &&
                    parameters.Length == 2 &&
                    parameters[1].ParameterType.IsGenericType &&
                    parameters[1].ParameterType.GetGenericTypeDefinition() == typeof(System.Action<>))
                {
                    return (IDisposable)method.MakeGenericMethod(typeof(T))
                        .Invoke(null, new object[] { observable, onNext });
                }
            }

            Assert.Fail("R3 Subscribe<T>(Observable<T>, Action<T>) overload was not found.");
            return null;
        }

        private static object GetReactiveSurface(object service, string propertyName)
        {
            var property = service.GetType().GetProperty(
                propertyName,
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            Assert.That(property, Is.Not.Null, $"{propertyName} must be a public read-model property.");
            return property.GetValue(service);
        }

        private static T GetCurrentValue<T>(object service, string propertyName)
        {
            var reactiveSurface = GetReactiveSurface(service, propertyName);
            var currentValueProperty =
                reactiveSurface.GetType().GetProperty(
                    "CurrentValue",
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            Assert.That(currentValueProperty, Is.Not.Null);
            return (T)currentValueProperty.GetValue(reactiveSurface);
        }
    }
}

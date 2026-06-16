using System;
using System.Collections.Generic;
using R3;

namespace ProjectContext.Shop
{
    public sealed class ShopService : IShopService, IDisposable
    {
        private readonly ShopItemData[] items;
        private readonly ShopItemGroup[] groups;
        private readonly Dictionary<ShopItemType, ShopItemData> itemsByType;
        private readonly Dictionary<ShopItemGroup, ShopItemData[]> itemsByGroup;
        private readonly ReactiveProperty<ShopItemGroup> selectedGroup;
        private readonly ReactiveProperty<IReadOnlyList<ShopItemData>> itemsInSelectedGroup;
        private readonly ReactiveProperty<ShopItemData> selectedItem;
        private bool disposed;

        public ShopService()
            : this(ShopCatalog.CreateDefaultItems())
        {
        }

        public ShopService(IReadOnlyList<ShopItemData> sourceItems)
        {
            if (sourceItems == null)
            {
                throw new ArgumentNullException(nameof(sourceItems));
            }

            items = CopyItems(sourceItems);
            groups = ResolveGroups(items);
            itemsByType = BuildItemsByType(items);
            itemsByGroup = BuildItemsByGroup(items, groups);

            var initialGroup = groups.Length > 0 ? groups[0] : ShopItemGroup.None;
            selectedGroup = new ReactiveProperty<ShopItemGroup>(initialGroup);
            itemsInSelectedGroup =
                new ReactiveProperty<IReadOnlyList<ShopItemData>>(GetItemsInGroup(initialGroup));
            selectedItem = new ReactiveProperty<ShopItemData>(GetFirstItem(initialGroup));
        }

        public IReadOnlyList<ShopItemGroup> Groups => groups;
        public IReadOnlyList<ShopItemData> Items => items;
        public ReadOnlyReactiveProperty<ShopItemGroup> SelectedGroup => selectedGroup;
        public ReadOnlyReactiveProperty<IReadOnlyList<ShopItemData>> ItemsInSelectedGroup => itemsInSelectedGroup;
        public ReadOnlyReactiveProperty<ShopItemData> SelectedItem => selectedItem;

        public void SelectGroup(ShopItemGroup group)
        {
            ThrowIfDisposed();
            if (group == ShopItemGroup.None || !itemsByGroup.ContainsKey(group))
            {
                return;
            }

            selectedGroup.Value = group;
            var nextItems = GetItemsInGroup(group);
            itemsInSelectedGroup.Value = nextItems;

            var currentItem = selectedItem.CurrentValue;
            if (currentItem == null || currentItem.Group != group)
            {
                selectedItem.Value = nextItems.Count > 0 ? nextItems[0] : null;
            }
        }

        public void SelectItem(ShopItemType type)
        {
            ThrowIfDisposed();
            if (!itemsByType.TryGetValue(type, out var item))
            {
                return;
            }

            if (selectedGroup.CurrentValue != item.Group)
            {
                SelectGroup(item.Group);
            }

            selectedItem.Value = item;
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            selectedItem.Dispose();
            itemsInSelectedGroup.Dispose();
            selectedGroup.Dispose();
        }

        private IReadOnlyList<ShopItemData> GetItemsInGroup(ShopItemGroup group)
        {
            return itemsByGroup.TryGetValue(group, out var groupItems)
                ? groupItems
                : Array.Empty<ShopItemData>();
        }

        private ShopItemData GetFirstItem(ShopItemGroup group)
        {
            var groupItems = GetItemsInGroup(group);
            return groupItems.Count > 0 ? groupItems[0] : null;
        }

        private void ThrowIfDisposed()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(nameof(ShopService));
            }
        }

        private static ShopItemData[] CopyItems(IReadOnlyList<ShopItemData> sourceItems)
        {
            var copy = new ShopItemData[sourceItems.Count];
            for (var i = 0; i < sourceItems.Count; i++)
            {
                copy[i] = sourceItems[i] ?? throw new ArgumentException("Shop item cannot be null.",
                    nameof(sourceItems));
            }

            return copy;
        }

        private static ShopItemGroup[] ResolveGroups(IReadOnlyList<ShopItemData> sourceItems)
        {
            var groupsList = new List<ShopItemGroup>();
            for (var i = 0; i < sourceItems.Count; i++)
            {
                var group = sourceItems[i].Group;
                if (!groupsList.Contains(group))
                {
                    groupsList.Add(group);
                }
            }

            groupsList.Sort();
            return groupsList.ToArray();
        }

        private static Dictionary<ShopItemType, ShopItemData> BuildItemsByType(IReadOnlyList<ShopItemData> sourceItems)
        {
            var result = new Dictionary<ShopItemType, ShopItemData>(sourceItems.Count);
            for (var i = 0; i < sourceItems.Count; i++)
            {
                result.Add(sourceItems[i].Type, sourceItems[i]);
            }

            return result;
        }

        private static Dictionary<ShopItemGroup, ShopItemData[]> BuildItemsByGroup(
            IReadOnlyList<ShopItemData> sourceItems,
            IReadOnlyList<ShopItemGroup> sourceGroups)
        {
            var result = new Dictionary<ShopItemGroup, ShopItemData[]>(sourceGroups.Count);
            for (var groupIndex = 0; groupIndex < sourceGroups.Count; groupIndex++)
            {
                var group = sourceGroups[groupIndex];
                var groupItems = new List<ShopItemData>();
                for (var itemIndex = 0; itemIndex < sourceItems.Count; itemIndex++)
                {
                    if (sourceItems[itemIndex].Group == group)
                    {
                        groupItems.Add(sourceItems[itemIndex]);
                    }
                }

                result.Add(group, groupItems.ToArray());
            }

            return result;
        }
    }
}

using System;

namespace ProjectContext.Shop
{
    public sealed class ShopItemData
    {
        public ShopItemData(ShopItemType type, ShopItemGroup group, int amount)
        {
            if (type == ShopItemType.None)
            {
                throw new ArgumentOutOfRangeException(nameof(type), "Shop item type must be explicit.");
            }

            if (group == ShopItemGroup.None)
            {
                throw new ArgumentOutOfRangeException(nameof(group), "Shop item group must be explicit.");
            }

            Type = type;
            Group = group;
            Amount = Math.Max(0, amount);
        }

        public ShopItemType Type { get; }
        public ShopItemGroup Group { get; }
        public int Amount { get; }
    }
}

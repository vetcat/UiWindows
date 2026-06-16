using System.Collections.Generic;

namespace ProjectContext.Shop
{
    public static class ShopCatalog
    {
        public static IReadOnlyList<ShopItemData> CreateDefaultItems()
        {
            return new[]
            {
                new ShopItemData(ShopItemType.Item_1, ShopItemGroup.Group_1, 10),
                new ShopItemData(ShopItemType.Item_2, ShopItemGroup.Group_1, 20),
                new ShopItemData(ShopItemType.Item_3, ShopItemGroup.Group_1, 30),
                new ShopItemData(ShopItemType.Item_4, ShopItemGroup.Group_1, 40),
                new ShopItemData(ShopItemType.Item_5, ShopItemGroup.Group_1, 50),
                new ShopItemData(ShopItemType.Item_6, ShopItemGroup.Group_1, 60),
                new ShopItemData(ShopItemType.Item_7, ShopItemGroup.Group_1, 70),
                new ShopItemData(ShopItemType.Item_8, ShopItemGroup.Group_1, 80),
                new ShopItemData(ShopItemType.Item_9, ShopItemGroup.Group_2, 90),
                new ShopItemData(ShopItemType.Item_10, ShopItemGroup.Group_2, 100)
            };
        }
    }
}

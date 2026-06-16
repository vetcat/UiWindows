using System.Collections.Generic;
using R3;

namespace ProjectContext.Shop
{
    public interface IShopReadModel
    {
        IReadOnlyList<ShopItemGroup> Groups { get; }
        IReadOnlyList<ShopItemData> Items { get; }
        ReadOnlyReactiveProperty<ShopItemGroup> SelectedGroup { get; }
        ReadOnlyReactiveProperty<IReadOnlyList<ShopItemData>> ItemsInSelectedGroup { get; }
        ReadOnlyReactiveProperty<ShopItemData> SelectedItem { get; }
    }
}

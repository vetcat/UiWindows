namespace ProjectContext.Shop
{
    public interface IShopCommands
    {
        void SelectGroup(ShopItemGroup group);
        void SelectItem(ShopItemType type);
    }
}

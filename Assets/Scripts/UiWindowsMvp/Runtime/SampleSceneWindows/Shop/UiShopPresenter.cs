using System;
using ProjectContext.Localization;
using ProjectContext.Shop;
using ProjectContext.UiRequests;
using R3;
using UiWindowsMvp.UIAdapter;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.UI.Windows;

namespace UiWindowsMvp.SampleSceneWindows
{
    public sealed class UiShopPresenter : IWindowPresenter<UiShopWindow>
    {
        private readonly IShopReadModel readModel;
        private readonly IShopCommands commands;
        private readonly ILocalizationReadModel localizationReadModel;
        private readonly IUiModalCommands modalCommands;
        private readonly IUiShopVisibilityCommands visibilityCommands;
        private readonly Func<UiShopWindow, UiShopView> viewResolver;

        private UiShopWindow window;
        private UiShopView view;
        private DisposableGroup groupBindings;
        private DisposableGroup itemBindings;
        private bool disposed;

        public UiShopPresenter(
            IShopReadModel readModel,
            IShopCommands commands,
            ILocalizationReadModel localizationReadModel)
            : this(readModel, commands, localizationReadModel, null, null, ResolveView)
        {
        }

        internal UiShopPresenter(
            IShopReadModel readModel,
            IShopCommands commands,
            ILocalizationReadModel localizationReadModel,
            Func<UiShopWindow, UiShopView> viewResolver)
            : this(readModel, commands, localizationReadModel, null, null, viewResolver)
        {
        }

        internal UiShopPresenter(
            IShopReadModel readModel,
            IShopCommands commands,
            ILocalizationReadModel localizationReadModel,
            IUiModalCommands modalCommands,
            IUiShopVisibilityCommands visibilityCommands,
            Func<UiShopWindow, UiShopView> viewResolver)
        {
            this.readModel = readModel ?? throw new ArgumentNullException(nameof(readModel));
            this.commands = commands ?? throw new ArgumentNullException(nameof(commands));
            this.localizationReadModel =
                localizationReadModel ?? throw new ArgumentNullException(nameof(localizationReadModel));
            this.modalCommands = modalCommands;
            this.visibilityCommands = visibilityCommands;
            this.viewResolver = viewResolver ?? throw new ArgumentNullException(nameof(viewResolver));
        }

        public void Bind(UiShopWindow window)
        {
            ThrowIfDisposed();
            this.window = window ?? throw new ArgumentNullException(nameof(window));
        }

        public void Initialize()
        {
            ThrowIfDisposed();
            view = ResolveRequiredView();
            view.EnsureLayout();
        }

        public void OnShowBegin(IUiShowScope showScope)
        {
            ThrowIfDisposed();
            if (showScope == null)
            {
                throw new ArgumentNullException(nameof(showScope));
            }

            view = ResolveRequiredView();
            view.EnsureLayout();
            visibilityCommands?.SetShopVisible(true);

            groupBindings = new DisposableGroup();
            itemBindings = new DisposableGroup();
            showScope.Add(groupBindings);
            showScope.Add(itemBindings);

            RebuildGroups();

            showScope.Add(readModel.SelectedGroup.Subscribe(RefreshGroupSelection));
            showScope.Add(readModel.ItemsInSelectedGroup.Subscribe(RebuildItems));
            showScope.Add(readModel.SelectedItem.Subscribe(RefreshDetails));
            showScope.Add(localizationReadModel.CurrentLanguage.Subscribe(_ => RefreshLocalizedText()));

            AddButtonListener(showScope, view.ButtonClose, HideWindow);
        }

        public void OnShowEnd()
        {
        }

        public void OnHideBegin()
        {
            groupBindings?.Dispose();
            itemBindings?.Dispose();
            groupBindings = null;
            itemBindings = null;
            view?.ReleaseCollectionItems();
        }

        public void OnHideEnd()
        {
            visibilityCommands?.SetShopVisible(false);
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            visibilityCommands?.SetShopVisible(false);
            groupBindings?.Dispose();
            itemBindings?.Dispose();
            view = null;
            window = null;
        }

        private void RebuildGroups()
        {
            groupBindings?.Clear();
            var groupItems = view.RebuildGroupItems(readModel.Groups);
            for (var i = 0; i < groupItems.Count; i++)
            {
                var item = groupItems[i];
                SetText(item.TextName, Translate(item.Group.ToString()));

                UnityAction<bool> action = isOn =>
                {
                    if (isOn)
                    {
                        commands.SelectGroup(item.Group);
                    }
                };

                item.Toggle.onValueChanged.AddListener(action);
                groupBindings?.Add(new DisposableAction(() => item.Toggle.onValueChanged.RemoveListener(action)));
            }

            RefreshGroupSelection(readModel.SelectedGroup.CurrentValue);
        }

        private void RebuildItems(System.Collections.Generic.IReadOnlyList<ShopItemData> items)
        {
            itemBindings?.Clear();
            var itemViews = view.RebuildShopItems(items);
            for (var i = 0; i < itemViews.Count; i++)
            {
                var itemView = itemViews[i];
                var itemData = items[i];
                itemView.Type = itemData.Type;
                SetText(itemView.TextName, Translate(itemData.Type.ToString()));
                SetText(itemView.TextAmount, itemData.Amount.ToString());

                UnityAction action = () => HandleItemClicked(itemView.Type);
                itemView.ButtonItem.onClick.AddListener(action);
                itemBindings?.Add(new DisposableAction(() => itemView.ButtonItem.onClick.RemoveListener(action)));
            }

            RefreshItemSelection(readModel.SelectedItem.CurrentValue);
        }

        private void RefreshGroupSelection(ShopItemGroup selectedGroup)
        {
            if (view == null)
            {
                return;
            }

            SetText(view.TextItemsType, Translate(selectedGroup.ToString()));
            var groupItems = view.GroupItems;
            for (var i = 0; i < groupItems.Count; i++)
            {
                groupItems[i].SetSelected(groupItems[i].Group == selectedGroup);
            }
        }

        private void RefreshDetails(ShopItemData item)
        {
            if (view?.DetailsLayout == null)
            {
                return;
            }

            if (item == null)
            {
                view.SetDetailsIcon(ShopItemType.None);
                SetText(view.DetailsLayout.TextName, string.Empty);
                SetText(view.DetailsLayout.TextAmount, string.Empty);
                SetText(view.DetailsLayout.TextGroup, string.Empty);
                SetText(view.DetailsLayout.TextDescription, string.Empty);
                RefreshItemSelection(null);
                return;
            }

            var itemName = Translate(item.Type.ToString());
            var groupName = Translate(item.Group.ToString());
            view.SetDetailsIcon(item.Type);
            SetText(view.DetailsLayout.TextName, itemName);
            SetText(view.DetailsLayout.TextAmount, Translate("ShopAmount", item.Amount));
            SetText(view.DetailsLayout.TextGroup, Translate("ShopGroup", groupName));
            SetText(view.DetailsLayout.TextDescription, Translate("ChoiceItemDescription", itemName));
            RefreshItemSelection(item);
        }

        private void RefreshItemSelection(ShopItemData selectedItem)
        {
            if (view == null)
            {
                return;
            }

            var itemViews = view.ShopItems;
            for (var i = 0; i < itemViews.Count; i++)
            {
                itemViews[i].SetSelected(selectedItem != null && itemViews[i].Type == selectedItem.Type);
            }
        }

        private void HandleItemClicked(ShopItemType type)
        {
            commands.SelectItem(type);
            RequestItemModal(type);
        }

        private void RequestItemModal(ShopItemType type)
        {
            if (modalCommands == null)
            {
                return;
            }

            var itemName = Translate(type.ToString());
            modalCommands.ShowInfoOk(
                Translate("ChoiceItemCaption"),
                Translate("ChoiceItemDescription", itemName));
        }

        private void RefreshLocalizedText()
        {
            if (view == null)
            {
                return;
            }

            SetText(view.TextHeader, Translate("ItemsShop"));
            RefreshGroupSelection(readModel.SelectedGroup.CurrentValue);

            var groupItems = view.GroupItems;
            for (var i = 0; i < groupItems.Count; i++)
            {
                SetText(groupItems[i].TextName, Translate(groupItems[i].Group.ToString()));
            }

            var itemViews = view.ShopItems;
            for (var i = 0; i < itemViews.Count; i++)
            {
                var itemData = FindItem(itemViews[i].Type);
                if (itemData == null)
                {
                    continue;
                }

                SetText(itemViews[i].TextName, Translate(itemData.Type.ToString()));
                SetText(itemViews[i].TextAmount, itemData.Amount.ToString());
            }

            RefreshDetails(readModel.SelectedItem.CurrentValue);
        }

        private ShopItemData FindItem(ShopItemType type)
        {
            var items = readModel.Items;
            for (var i = 0; i < items.Count; i++)
            {
                if (items[i].Type == type)
                {
                    return items[i];
                }
            }

            return null;
        }

        private UiShopView ResolveRequiredView()
        {
            var resolved = viewResolver(window);
            if (resolved == null)
            {
                throw new InvalidOperationException("UiShopView was not loaded by the UiShopWindow layout.");
            }

            return resolved;
        }

        private static UiShopView ResolveView(UiShopWindow window)
        {
            if (window == null)
            {
                return null;
            }

            return window.TryGetView(out var resolved) ? resolved : null;
        }

        private void HideWindow()
        {
            window?.Hide(TransitionParameters.Default.ReplaceImmediately(true));
        }

        private string Translate(string key, params object[] args)
        {
            return localizationReadModel.Translate(key, args);
        }

        private static void SetText(Text text, string value)
        {
            if (text != null)
            {
                text.text = value;
            }
        }

        private static void AddButtonListener(IUiShowScope showScope, Button button, UnityAction action)
        {
            if (button == null)
            {
                return;
            }

            button.onClick.AddListener(action);
            showScope.Add(new DisposableAction(() => button.onClick.RemoveListener(action)));
        }

        private void ThrowIfDisposed()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(nameof(UiShopPresenter));
            }
        }
    }
}

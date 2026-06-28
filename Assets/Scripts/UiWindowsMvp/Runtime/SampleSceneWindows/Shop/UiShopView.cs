using System;
using System.Collections.Generic;
using ProjectContext.Shop;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Windows;

namespace UiWindowsMvp.SampleSceneWindows
{
    public sealed class UiShopView : WindowComponent
    {
        public RectTransform Body;
        public Button ButtonClose;
        public Text TextHeader;
        public Text TextItemsType;
        public ToggleGroup ToggleGroup;
        public ScrollRect ScrollRect;
        public RectTransform GroupItemsRoot;
        public RectTransform GroupPoolRoot;
        public RectTransform ShopItemsRoot;
        public RectTransform ShopItemPoolRoot;
        public ShopDetailsLayout DetailsLayout = new();

        private PooledViewCollection<ShopGroupItemView> groupCollection;
        private PooledViewCollection<ShopItemView> itemCollection;

        public IReadOnlyList<ShopGroupItemView> GroupItems =>
            groupCollection?.ActiveItems ?? Array.Empty<ShopGroupItemView>();

        public IReadOnlyList<ShopItemView> ShopItems =>
            itemCollection?.ActiveItems ?? Array.Empty<ShopItemView>();

        public int PooledShopItemCount => itemCollection?.PooledCount ?? 0;

        public void EnsureLayout()
        {
            if (Body == null ||
                ButtonClose == null ||
                TextHeader == null ||
                TextItemsType == null ||
                ToggleGroup == null ||
                ScrollRect == null ||
                GroupItemsRoot == null ||
                GroupPoolRoot == null ||
                ShopItemsRoot == null ||
                ShopItemPoolRoot == null ||
                DetailsLayout == null ||
                DetailsLayout.ImageIcon == null ||
                DetailsLayout.TextName == null ||
                DetailsLayout.TextAmount == null ||
                DetailsLayout.TextGroup == null ||
                DetailsLayout.TextDescription == null)
            {
                throw UiPrefabReferenceGuard.Missing(
                    this,
                    "Body, ButtonClose, TextHeader, TextItemsType, ToggleGroup, ScrollRect, GroupItemsRoot, " +
                    "GroupPoolRoot, ShopItemsRoot, ShopItemPoolRoot, DetailsLayout");
            }

            EnsureCollections();
        }

        public IReadOnlyList<ShopGroupItemView> RebuildGroupItems(IReadOnlyList<ShopItemGroup> groups)
        {
            EnsureLayout();
            groupCollection.Rebuild(groups, BindGroupShell);
            return groupCollection.ActiveItems;
        }

        public IReadOnlyList<ShopItemView> RebuildShopItems(IReadOnlyList<ShopItemData> items)
        {
            EnsureLayout();
            itemCollection.Rebuild(items, BindItemShell);
            ScrollToTop();
            return itemCollection.ActiveItems;
        }

        public void ReleaseCollectionItems()
        {
            groupCollection?.ReleaseActive();
            itemCollection?.ReleaseActive();
        }

        public void SetDetailsIcon(ShopItemType type)
        {
            if (DetailsLayout?.ImageIcon == null)
            {
                return;
            }

            DetailsLayout.ImageIcon.color = ColorForItemType(type);
        }

        public void ScrollToTop()
        {
            if (ScrollRect == null)
            {
                return;
            }

            if (ScrollRect.content != null)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(ScrollRect.content);
            }

            ScrollRect.verticalNormalizedPosition = 1f;
        }

        private void EnsureCollections()
        {
            groupCollection ??= new PooledViewCollection<ShopGroupItemView>(
                GroupItemsRoot,
                GroupPoolRoot,
                CreateGroupItem);
            itemCollection ??= new PooledViewCollection<ShopItemView>(
                ShopItemsRoot,
                ShopItemPoolRoot,
                CreateShopItem);
        }

        private static void BindGroupShell(ShopGroupItemView item, ShopItemGroup group, int index)
        {
            item.Group = group;
            if (item.Toggle != null)
            {
                item.Toggle.group = item.GetComponentInParent<ToggleGroup>();
            }

            if (item.ImageIcon != null)
            {
                item.ImageIcon.color = ColorForGroup(group, index);
            }

            item.SetSelected(false);
        }

        private static void BindItemShell(ShopItemView item, ShopItemData data, int index)
        {
            item.Type = data.Type;
            if (item.ImageIcon != null)
            {
                item.ImageIcon.color = ColorForItemType(data.Type, index);
            }

            item.SetSelected(false);
        }

        private static ShopGroupItemView CreateGroupItem(RectTransform parent)
        {
            var itemObject = CreatePanel("ShopGroupItem", parent, new Color(0.18f, 0.2f, 0.24f, 1f));
            itemObject.gameObject.AddComponent<Toggle>();
            itemObject.gameObject.AddComponent<CanvasGroup>();
            AddLayoutElement(itemObject.gameObject, 84f, -1f, -1f, -1f);

            var glow = CreateImage("Glow", itemObject, new Color(0.35f, 0.75f, 0.95f, 0.12f));
            Stretch((RectTransform)glow.transform);
            glow.raycastTarget = false;

            var icon = CreateImage("Icon", itemObject, new Color(0.35f, 0.75f, 0.95f, 1f));
            var iconRect = (RectTransform)icon.transform;
            iconRect.anchorMin = new Vector2(0.5f, 1f);
            iconRect.anchorMax = new Vector2(0.5f, 1f);
            iconRect.pivot = new Vector2(0.5f, 1f);
            iconRect.anchoredPosition = new Vector2(0f, -10f);
            iconRect.sizeDelta = new Vector2(38f, 38f);
            icon.raycastTarget = false;

            var text = CreateText("Group", itemObject, 14, TextAnchor.MiddleCenter);
            var textRect = (RectTransform)text.transform;
            textRect.anchorMin = new Vector2(0f, 0f);
            textRect.anchorMax = new Vector2(1f, 0f);
            textRect.pivot = new Vector2(0.5f, 0f);
            textRect.anchoredPosition = new Vector2(0f, 8f);
            textRect.sizeDelta = new Vector2(-8f, 26f);

            var toggle = itemObject.GetComponent<Toggle>();
            toggle.targetGraphic = itemObject.GetComponent<Image>();

            var item = itemObject.gameObject.AddComponent<ShopGroupItemView>();
            item.Toggle = toggle;
            item.ImageIcon = icon;
            item.ImageGlow = glow;
            item.TextName = text;
            return item;
        }

        private static ShopItemView CreateShopItem(RectTransform parent)
        {
            var itemObject = CreatePanel("ShopItem", parent, new Color(0.17f, 0.18f, 0.22f, 1f));
            itemObject.gameObject.AddComponent<Button>();
            itemObject.gameObject.AddComponent<CanvasGroup>();
            AddLayoutElement(itemObject.gameObject, 78f, -1f, -1f, -1f);

            var selection = CreateImage("Selection", itemObject, new Color(0.42f, 0.76f, 0.56f, 0.08f));
            Stretch((RectTransform)selection.transform);
            selection.raycastTarget = false;

            var icon = CreateImage("Icon", itemObject, new Color(0.35f, 0.6f, 0.95f, 1f));
            var iconRect = (RectTransform)icon.transform;
            iconRect.anchorMin = new Vector2(0f, 0.5f);
            iconRect.anchorMax = new Vector2(0f, 0.5f);
            iconRect.pivot = new Vector2(0f, 0.5f);
            iconRect.anchoredPosition = new Vector2(12f, 0f);
            iconRect.sizeDelta = new Vector2(54f, 54f);
            icon.raycastTarget = false;

            var nameText = CreateText("Item", itemObject, 18, TextAnchor.MiddleLeft);
            var nameRect = (RectTransform)nameText.transform;
            nameRect.anchorMin = new Vector2(0f, 0.5f);
            nameRect.anchorMax = new Vector2(1f, 1f);
            nameRect.offsetMin = new Vector2(82f, -2f);
            nameRect.offsetMax = new Vector2(-18f, -8f);

            var amountText = CreateText("0", itemObject, 16, TextAnchor.MiddleLeft);
            var amountRect = (RectTransform)amountText.transform;
            amountRect.anchorMin = new Vector2(0f, 0f);
            amountRect.anchorMax = new Vector2(1f, 0.5f);
            amountRect.offsetMin = new Vector2(82f, 8f);
            amountRect.offsetMax = new Vector2(-18f, 2f);
            amountText.color = new Color(0.74f, 0.84f, 0.92f, 1f);

            var button = itemObject.GetComponent<Button>();
            button.targetGraphic = itemObject.GetComponent<Image>();

            var item = itemObject.gameObject.AddComponent<ShopItemView>();
            item.ButtonItem = button;
            item.ImageIcon = icon;
            item.ImageSelection = selection;
            item.TextName = nameText;
            item.TextAmount = amountText;
            return item;
        }

        private static RectTransform CreateRect(string name, Transform parent)
        {
            var gameObject = new GameObject(name, typeof(RectTransform));
            gameObject.transform.SetParent(parent, false);
            var rectTransform = (RectTransform)gameObject.transform;
            rectTransform.localScale = Vector3.one;
            return rectTransform;
        }

        private static RectTransform CreatePanel(
            string name,
            Transform parent,
            Color color,
            bool raycastTarget = true)
        {
            var rectTransform = CreateRect(name, parent);
            var image = rectTransform.gameObject.AddComponent<Image>();
            image.color = color;
            image.raycastTarget = raycastTarget;
            return rectTransform;
        }

        private static Image CreateImage(string name, Transform parent, Color color)
        {
            var gameObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            gameObject.transform.SetParent(parent, false);
            var image = gameObject.GetComponent<Image>();
            image.color = color;
            image.raycastTarget = false;
            return image;
        }

        private static Text CreateText(string value, Transform parent, int fontSize, TextAnchor alignment)
        {
            var gameObject = new GameObject(value + " Text", typeof(RectTransform), typeof(CanvasRenderer),
                typeof(Text));
            gameObject.transform.SetParent(parent, false);
            var text = gameObject.GetComponent<Text>();
            text.text = value;
            text.color = Color.white;
            text.fontSize = fontSize;
            text.alignment = alignment;
            text.raycastTarget = false;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            return text;
        }

        private static void AddLayoutElement(
            GameObject gameObject,
            float preferredHeight,
            float preferredWidth,
            float flexibleHeight,
            float flexibleWidth)
        {
            var layoutElement = gameObject.GetComponent<LayoutElement>() ?? gameObject.AddComponent<LayoutElement>();
            if (preferredHeight >= 0f)
            {
                layoutElement.preferredHeight = preferredHeight;
            }

            if (preferredWidth >= 0f)
            {
                layoutElement.preferredWidth = preferredWidth;
            }

            if (flexibleHeight >= 0f)
            {
                layoutElement.flexibleHeight = flexibleHeight;
            }

            if (flexibleWidth >= 0f)
            {
                layoutElement.flexibleWidth = flexibleWidth;
            }
        }

        private static void Stretch(RectTransform rectTransform)
        {
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = Vector2.zero;
        }

        private static Color ColorForGroup(ShopItemGroup group, int index)
        {
            return group == ShopItemGroup.Group_1
                ? new Color(0.3f, 0.62f, 0.92f, 1f)
                : new Color(0.9f, 0.58f, 0.22f, 1f);
        }

        private static Color ColorForItemType(ShopItemType type, int index = 0)
        {
            var value = Mathf.Max(1, (int)type + index);
            var hue = (value * 0.097f) % 1f;
            return Color.HSVToRGB(hue, 0.55f, 0.9f);
        }
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UiWindowsMvp.SampleSceneWindows
{
    internal sealed class PooledViewCollection<TView>
        where TView : Component
    {
        private readonly RectTransform activeRoot;
        private readonly RectTransform poolRoot;
        private readonly Func<RectTransform, TView> createItem;
        private readonly List<TView> activeItems = new();
        private readonly Stack<TView> pooledItems = new();

        public PooledViewCollection(
            RectTransform activeRoot,
            RectTransform poolRoot,
            Func<RectTransform, TView> createItem)
        {
            this.activeRoot = activeRoot != null ? activeRoot : throw new ArgumentNullException(nameof(activeRoot));
            this.poolRoot = poolRoot != null ? poolRoot : throw new ArgumentNullException(nameof(poolRoot));
            this.createItem = createItem ?? throw new ArgumentNullException(nameof(createItem));
        }

        public IReadOnlyList<TView> ActiveItems => activeItems;
        public int PooledCount => pooledItems.Count;

        public void Rebuild<TData>(IReadOnlyList<TData> data, Action<TView, TData, int> bind)
        {
            if (bind == null)
            {
                throw new ArgumentNullException(nameof(bind));
            }

            if (data == null)
            {
                ReleaseActive();
                return;
            }

            TrimActive(data.Count);
            EnsureActive(data.Count);

            for (var i = 0; i < data.Count; i++)
            {
                bind(activeItems[i], data[i], i);
            }
        }

        public void ReleaseActive()
        {
            for (var i = activeItems.Count - 1; i >= 0; i--)
            {
                var item = activeItems[i];
                if (item == null)
                {
                    continue;
                }

                ConfigurePooledState(item, true);
                item.transform.SetParent(poolRoot, false);
                pooledItems.Push(item);
            }

            activeItems.Clear();
        }

        private void TrimActive(int count)
        {
            for (var i = activeItems.Count - 1; i >= count; i--)
            {
                var item = activeItems[i];
                activeItems.RemoveAt(i);
                if (item == null)
                {
                    continue;
                }

                ConfigurePooledState(item, true);
                item.transform.SetParent(poolRoot, false);
                pooledItems.Push(item);
            }
        }

        private void EnsureActive(int count)
        {
            while (activeItems.Count < count)
            {
                var item = TakeItem();
                item.transform.SetParent(activeRoot, false);
                ConfigurePooledState(item, false);
                activeItems.Add(item);
            }
        }

        private TView TakeItem()
        {
            while (pooledItems.Count > 0)
            {
                var item = pooledItems.Pop();
                if (item != null)
                {
                    return item;
                }
            }

            return createItem(activeRoot);
        }

        private static void ConfigurePooledState(TView item, bool pooled)
        {
            var canvasGroup = item.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = item.gameObject.AddComponent<CanvasGroup>();
            }

            canvasGroup.alpha = pooled ? 0f : 1f;
            canvasGroup.interactable = !pooled;
            canvasGroup.blocksRaycasts = !pooled;

            var layoutElement = item.GetComponent<LayoutElement>();
            if (layoutElement != null)
            {
                layoutElement.ignoreLayout = pooled;
            }
        }
    }
}

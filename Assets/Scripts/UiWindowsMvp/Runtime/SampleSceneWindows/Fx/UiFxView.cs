using System.Collections.Generic;
using DG.Tweening;
using ProjectContext.UiRequests;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Windows;

namespace UiWindowsMvp.SampleSceneWindows
{
    public sealed class UiFxView : WindowComponent
    {
        private const float FadeDuration = 0.2f;
        private const float FlyDuration = 0.65f;

        public RectTransform Body;
        public RectTransform FxRoot;
        public RectTransform PoolRoot;
        public RectTransform CollectSource;
        public RectTransform CollectTarget;
        public RectTransform SpendSource;

        private readonly List<Sequence> activeSequences = new();
        private readonly List<UiFxItemView> activeItems = new();
        private readonly Stack<UiFxItemView> pooledItems = new();
        private int requestIndex;
        private string lastFxText = string.Empty;

        public int ActiveFxCount => activeItems.Count;
        public int PooledFxCount => pooledItems.Count;
        public string LastFxText => lastFxText;

        public void EnsureLayout()
        {
            if (Body != null)
            {
                return;
            }

            BuildDefaultLayout();
        }

        public void PlayFx(UiFxRequest request)
        {
            EnsureLayout();

            if (request.Kind == UiFxKind.Spend)
            {
                PlaySpend(request);
                return;
            }

            PlayCollect(request);
        }

        public void StopAllFx()
        {
            for (var i = activeSequences.Count - 1; i >= 0; i--)
            {
                activeSequences[i]?.Kill();
            }

            activeSequences.Clear();

            for (var i = activeItems.Count - 1; i >= 0; i--)
            {
                ReleaseItem(activeItems[i]);
            }

            activeItems.Clear();
        }

        private void PlayCollect(UiFxRequest request)
        {
            var item = RentItem();
            var startPosition = CollectSource.anchoredPosition + ResolveOffset();
            var targetPosition = CollectTarget.anchoredPosition;
            var iconColor = ColorForTarget(request.Target);
            var amountColor = new Color(0.62f, 0.92f, 0.64f, 1f);
            var text = "+" + request.Amount;
            lastFxText = text;
            item.Prepare(text, amountColor, iconColor, startPosition);

            var sequence = DOTween.Sequence();
            Track(sequence, item);
            sequence.Append(item.CanvasGroup.DOFade(1f, FadeDuration));
            sequence.Join(item.RectTransform.DOScale(1.2f, FadeDuration).SetEase(Ease.OutBack));
            sequence.Append(item.RectTransform.DOAnchorPos(targetPosition, FlyDuration).SetEase(Ease.InOutSine));
            sequence.Join(item.RectTransform.DOScale(0.82f, FlyDuration));
            sequence.Join(item.TextAmount.DOColor(Color.white, FlyDuration));
            sequence.Append(item.CanvasGroup.DOFade(0f, FadeDuration));
            sequence.OnComplete(() => Complete(sequence, item));
        }

        private void PlaySpend(UiFxRequest request)
        {
            var item = RentItem();
            var startPosition = SpendSource.anchoredPosition;
            var endPosition = startPosition + ResolveOffset() + new Vector2(0f, -48f);
            var iconColor = ColorForTarget(request.Target);
            var amountColor = new Color(1f, 0.28f, 0.25f, 1f);
            var text = "-" + request.Amount;
            lastFxText = text;
            item.Prepare(text, amountColor, iconColor, startPosition);

            var sequence = DOTween.Sequence();
            Track(sequence, item);
            sequence.Append(item.RectTransform.DOAnchorPos(endPosition, 0.35f).SetEase(Ease.OutCubic));
            sequence.Join(item.CanvasGroup.DOFade(1f, FadeDuration));
            sequence.Join(item.RectTransform.DOScale(1.15f, FadeDuration).SetEase(Ease.OutBack));
            sequence.Append(item.RectTransform.DOScale(0.55f, 0.35f));
            sequence.Join(item.CanvasGroup.DOFade(0f, FadeDuration));
            sequence.OnComplete(() => Complete(sequence, item));
        }

        private void Track(Sequence sequence, UiFxItemView item)
        {
            activeSequences.Add(sequence);
            activeItems.Add(item);
        }

        private void Complete(Sequence sequence, UiFxItemView item)
        {
            activeSequences.Remove(sequence);
            activeItems.Remove(item);
            ReleaseItem(item);
        }

        private UiFxItemView RentItem()
        {
            UiFxItemView item = null;
            while (pooledItems.Count > 0 && item == null)
            {
                item = pooledItems.Pop();
            }

            item ??= CreateItem(FxRoot);
            item.transform.SetParent(FxRoot, false);
            return item;
        }

        private void ReleaseItem(UiFxItemView item)
        {
            if (item == null)
            {
                return;
            }

            item.Release();
            item.transform.SetParent(PoolRoot, false);
            pooledItems.Push(item);
        }

        private Vector2 ResolveOffset()
        {
            requestIndex++;
            var angle = requestIndex * 137.5f * Mathf.Deg2Rad;
            var radius = 18f + requestIndex % 4 * 8f;
            return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
        }

        private void BuildDefaultLayout()
        {
            var root = (RectTransform)transform;
            Stretch(root);

            Body = CreateRect("Body", transform);
            Stretch(Body);
            FxRoot = CreateRect("FxRoot", Body);
            Stretch(FxRoot);
            PoolRoot = CreateRect("PoolRoot", Body);
            Stretch(PoolRoot);
            var poolGroup = PoolRoot.gameObject.AddComponent<CanvasGroup>();
            poolGroup.alpha = 0f;
            poolGroup.interactable = false;
            poolGroup.blocksRaycasts = false;

            CollectSource = CreateAnchor("CollectSource", Body, new Vector2(0.18f, 0.28f), new Vector2(0f, 0f));
            CollectTarget = CreateAnchor("CollectTarget", Body, new Vector2(0f, 1f), new Vector2(80f, -64f));
            SpendSource = CreateAnchor("SpendSource", Body, new Vector2(0f, 1f), new Vector2(80f, -64f));
        }

        private static UiFxItemView CreateItem(RectTransform parent)
        {
            var itemRect = CreateRect("FxItem", parent);
            itemRect.sizeDelta = new Vector2(132f, 52f);
            var canvasGroup = itemRect.gameObject.AddComponent<CanvasGroup>();

            var icon = CreateImage("Icon", itemRect, new Color(0.95f, 0.78f, 0.26f, 1f));
            var iconRect = (RectTransform)icon.transform;
            iconRect.anchorMin = new Vector2(0f, 0.5f);
            iconRect.anchorMax = new Vector2(0f, 0.5f);
            iconRect.pivot = new Vector2(0f, 0.5f);
            iconRect.anchoredPosition = new Vector2(0f, 0f);
            iconRect.sizeDelta = new Vector2(42f, 42f);
            icon.raycastTarget = false;

            var amount = CreateText("+0", itemRect, 24, TextAnchor.MiddleLeft);
            var amountRect = (RectTransform)amount.transform;
            amountRect.anchorMin = new Vector2(0f, 0f);
            amountRect.anchorMax = new Vector2(1f, 1f);
            amountRect.offsetMin = new Vector2(52f, 0f);
            amountRect.offsetMax = Vector2.zero;

            var item = itemRect.gameObject.AddComponent<UiFxItemView>();
            item.Body = itemRect;
            item.CanvasGroup = canvasGroup;
            item.ImageIcon = icon;
            item.TextAmount = amount;
            item.Release();
            return item;
        }

        private static RectTransform CreateAnchor(
            string name,
            Transform parent,
            Vector2 anchor,
            Vector2 anchoredPosition)
        {
            var rect = CreateRect(name, parent);
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = Vector2.zero;
            rect.anchoredPosition = anchoredPosition;
            return rect;
        }

        private static RectTransform CreateRect(string name, Transform parent)
        {
            var gameObject = new GameObject(name, typeof(RectTransform));
            gameObject.transform.SetParent(parent, false);
            var rectTransform = (RectTransform)gameObject.transform;
            rectTransform.localScale = Vector3.one;
            return rectTransform;
        }

        private static Image CreateImage(string name, Transform parent, Color color)
        {
            var gameObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            gameObject.transform.SetParent(parent, false);
            var image = gameObject.GetComponent<Image>();
            image.color = color;
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

        private static Color ColorForTarget(UiFxTarget target)
        {
            switch (target)
            {
                case UiFxTarget.Experience:
                    return new Color(0.42f, 0.64f, 1f, 1f);
                case UiFxTarget.Health:
                    return new Color(1f, 0.32f, 0.36f, 1f);
                default:
                    return new Color(0.95f, 0.78f, 0.26f, 1f);
            }
        }

        private static void Stretch(RectTransform rectTransform)
        {
            rectTransform.localScale = Vector3.one;
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = Vector2.zero;
        }
    }
}
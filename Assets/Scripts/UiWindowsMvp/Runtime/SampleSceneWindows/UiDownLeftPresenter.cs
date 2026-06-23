using System;
using ProjectContext.Localization;
using R3;
using UiWindowsMvp.UIAdapter;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UiWindowsMvp.SampleSceneWindows
{
    public sealed class UiDownLeftPresenter : IWindowPresenter<UiDownLeftWindow>
    {
        private readonly Action openShop;
        private readonly ILocalizationReadModel localizationReadModel;
        private readonly Func<UiDownLeftWindow, UiDownLeftView> viewResolver;

        private UiDownLeftWindow window;
        private UiDownLeftView view;
        private bool disposed;

        public UiDownLeftPresenter(Action openShop, ILocalizationReadModel localizationReadModel)
            : this(openShop, localizationReadModel, ResolveView)
        {
        }

        internal UiDownLeftPresenter(
            Action openShop,
            ILocalizationReadModel localizationReadModel,
            Func<UiDownLeftWindow, UiDownLeftView> viewResolver)
        {
            this.openShop = openShop ?? throw new ArgumentNullException(nameof(openShop));
            this.localizationReadModel =
                localizationReadModel ?? throw new ArgumentNullException(nameof(localizationReadModel));
            this.viewResolver = viewResolver ?? throw new ArgumentNullException(nameof(viewResolver));
        }

        public void Bind(UiDownLeftWindow window)
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
            RefreshLocalizedText();

            showScope.Add(localizationReadModel.CurrentLanguage.Subscribe(_ => RefreshLocalizedText()));
            AddButtonListener(showScope, view.ButtonItemsShop, OpenShop);
        }

        public void OnShowEnd()
        {
        }

        public void OnHideBegin()
        {
        }

        public void OnHideEnd()
        {
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            view = null;
            window = null;
        }

        private void OpenShop()
        {
            openShop();
        }

        private void RefreshLocalizedText()
        {
            if (view == null)
            {
                return;
            }

            view.SetItemsShopText(localizationReadModel.Translate("ItemsShop"));
        }

        private UiDownLeftView ResolveRequiredView()
        {
            var resolved = viewResolver(window);
            if (resolved == null)
            {
                throw new InvalidOperationException("UiDownLeftView was not loaded by the UiDownLeftWindow layout.");
            }

            return resolved;
        }

        private static UiDownLeftView ResolveView(UiDownLeftWindow window)
        {
            if (window == null)
            {
                return null;
            }

            return window.TryGetView(out var resolved) ? resolved : null;
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
                throw new ObjectDisposedException(nameof(UiDownLeftPresenter));
            }
        }
    }
}

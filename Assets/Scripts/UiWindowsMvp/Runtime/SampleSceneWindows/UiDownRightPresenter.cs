using System;
using ProjectContext.Localization;
using R3;
using UiWindowsMvp.UIAdapter;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UiWindowsMvp.SampleSceneWindows
{
    public sealed class UiDownRightPresenter : IWindowPresenter<UiDownRightWindow>
    {
        private readonly Action openSettings;
        private readonly ILocalizationReadModel localizationReadModel;
        private readonly Func<UiDownRightWindow, UiDownRightView> viewResolver;

        private UiDownRightWindow window;
        private UiDownRightView view;
        private bool disposed;

        public UiDownRightPresenter(Action openSettings, ILocalizationReadModel localizationReadModel)
            : this(openSettings, localizationReadModel, ResolveView)
        {
        }

        internal UiDownRightPresenter(
            Action openSettings,
            ILocalizationReadModel localizationReadModel,
            Func<UiDownRightWindow, UiDownRightView> viewResolver)
        {
            this.openSettings = openSettings ?? throw new ArgumentNullException(nameof(openSettings));
            this.localizationReadModel =
                localizationReadModel ?? throw new ArgumentNullException(nameof(localizationReadModel));
            this.viewResolver = viewResolver ?? throw new ArgumentNullException(nameof(viewResolver));
        }

        public void Bind(UiDownRightWindow window)
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
            AddButtonListener(showScope, view.ButtonSettings, OpenSettings);
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

        private void OpenSettings()
        {
            openSettings();
        }

        private void RefreshLocalizedText()
        {
            if (view == null)
            {
                return;
            }

            view.SetSettingsText(localizationReadModel.Translate("Settings"));
        }

        private UiDownRightView ResolveRequiredView()
        {
            var resolved = viewResolver(window);
            if (resolved == null)
            {
                throw new InvalidOperationException("UiDownRightView was not loaded by the UiDownRightWindow layout.");
            }

            return resolved;
        }

        private static UiDownRightView ResolveView(UiDownRightWindow window)
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
                throw new ObjectDisposedException(nameof(UiDownRightPresenter));
            }
        }
    }
}

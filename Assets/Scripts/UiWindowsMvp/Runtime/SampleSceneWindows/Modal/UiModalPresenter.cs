using System;
using ProjectContext.UiRequests;
using R3;
using UiWindowsMvp.UIAdapter;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UiWindowsMvp.SampleSceneWindows
{
    public sealed class UiModalPresenter : IWindowPresenter<UiModalWindow>
    {
        private readonly IUiModalReadModel readModel;
        private readonly IUiModalCommands commands;
        private readonly Func<UiModalWindow, UiModalView> viewResolver;

        private UiModalWindow window;
        private UiModalView view;
        private bool disposed;

        public UiModalPresenter(IUiModalReadModel readModel, IUiModalCommands commands)
            : this(readModel, commands, ResolveView)
        {
        }

        internal UiModalPresenter(
            IUiModalReadModel readModel,
            IUiModalCommands commands,
            Func<UiModalWindow, UiModalView> viewResolver)
        {
            this.readModel = readModel ?? throw new ArgumentNullException(nameof(readModel));
            this.commands = commands ?? throw new ArgumentNullException(nameof(commands));
            this.viewResolver = viewResolver ?? throw new ArgumentNullException(nameof(viewResolver));
        }

        public void Bind(UiModalWindow window)
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
            view.Apply(readModel.CurrentModal.CurrentValue);

            showScope.Add(readModel.CurrentModal.Subscribe(view.Apply));
            AddButtonListener(showScope, view.ButtonOk, () => commands.CompleteCurrent(UiModalResult.Ok));
            AddButtonListener(showScope, view.ButtonCancel, () => commands.CompleteCurrent(UiModalResult.Cancel));
            AddButtonListener(showScope, view.ButtonClose, () => commands.CompleteCurrent(UiModalResult.Close));
            AddButtonListener(showScope, view.ButtonCloseBg, () => commands.CompleteCurrent(UiModalResult.Close));
        }

        public void OnShowEnd()
        {
        }

        public void OnHideBegin()
        {
            view?.Clear();
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

        private UiModalView ResolveRequiredView()
        {
            var resolved = viewResolver(window);
            if (resolved == null)
            {
                throw new InvalidOperationException("UiModalView was not loaded by the UiModalWindow layout.");
            }

            return resolved;
        }

        private static UiModalView ResolveView(UiModalWindow window)
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
                throw new ObjectDisposedException(nameof(UiModalPresenter));
            }
        }
    }
}
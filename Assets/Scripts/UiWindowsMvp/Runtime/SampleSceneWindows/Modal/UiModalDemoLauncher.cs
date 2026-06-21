using System;
using ProjectContext.UiRequests;
using R3;
using UiWindowsMvp.UIAdapter;
using UnityEngine.UI.Windows;

namespace UiWindowsMvp.SampleSceneWindows
{
    public sealed class UiModalDemoLauncher : IDisposable
    {
        private const int LayoutTagId = 10;

        private readonly UiModalView viewPrefab;
        private readonly UiModalPresenterFactory presenterFactory;
        private readonly IUiModalReadModel readModel;

        private UiRuntimeWindowSource<UiModalWindow, UiModalView> runtimeSource;
        private UiModalWindow currentWindow;
        private IDisposable modalSubscription;
        private bool disposed;

        public UiModalDemoLauncher(
            UiModalView viewPrefab,
            UiModalPresenterFactory presenterFactory,
            IUiModalReadModel readModel)
        {
            this.viewPrefab = viewPrefab != null ? viewPrefab : throw new ArgumentNullException(nameof(viewPrefab));
            this.presenterFactory = presenterFactory ?? throw new ArgumentNullException(nameof(presenterFactory));
            this.readModel = readModel ?? throw new ArgumentNullException(nameof(readModel));
        }

        public UiModalWindow CurrentWindow => currentWindow;

        public void Start()
        {
            ThrowIfDisposed();
            if (modalSubscription != null)
            {
                return;
            }

            modalSubscription = readModel.CurrentModal.Subscribe(HandleModalChanged);
        }

        public void Show()
        {
            ThrowIfDisposed();
            if (WindowSystem.HasInstance() == false)
            {
                throw new InvalidOperationException("A WindowSystem must exist before showing the modal slice.");
            }

            if (currentWindow != null && currentWindow.GetState() < ObjectState.Hiding)
            {
                return;
            }

            runtimeSource ??= UiRuntimeWindowSource<UiModalWindow, UiModalView>.Create(
                viewPrefab,
                nameof(UiModalWindow),
                LayoutTagId,
                takeFocus: true);

            var initialParameters = new InitialParameters
            {
                showSync = true
            };

            currentWindow = WindowSystem.Show(
                runtimeSource.WindowSource,
                initialParameters,
                BindPresenter,
                TransitionParameters.Default.ReplaceImmediately(true)).screen as UiModalWindow;
        }

        public void Hide()
        {
            if (currentWindow == null || WindowSystem.HasInstance() == false ||
                currentWindow.GetState() >= ObjectState.Hiding)
            {
                return;
            }

            currentWindow.Hide(TransitionParameters.Default.ReplaceImmediately(true));
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            modalSubscription?.Dispose();
            modalSubscription = null;
            Hide();
            currentWindow = null;
            runtimeSource?.Dispose();
            runtimeSource = null;
        }

        private void HandleModalChanged(UiModalRequest request)
        {
            if (request == null)
            {
                Hide();
                return;
            }

            Show();
        }

        private void BindPresenter(WindowBase window)
        {
            if (window is not UiModalWindow modalWindow)
            {
                throw new InvalidOperationException($"Expected {nameof(UiModalWindow)}, got {window.GetType().Name}.");
            }

            currentWindow = modalWindow;
            if (WindowPresenterBinder.TryGetBinding(modalWindow, out _))
            {
                return;
            }

            WindowPresenterBinder.Bind(modalWindow, presenterFactory);
        }

        private void ThrowIfDisposed()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(nameof(UiModalDemoLauncher));
            }
        }
    }
}
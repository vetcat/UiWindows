using System;
using ProjectContext.UiRequests;
using R3;

namespace UiWindowsMvp.SampleSceneWindows
{
    public sealed class UiModalDemoLauncher : IDisposable
    {
        private const int LayoutTagId = 10;

        private readonly UiRuntimeWindowHandle<UiModalWindow, UiModalView> windowHandle;
        private readonly IUiModalReadModel readModel;

        private IDisposable modalSubscription;
        private bool disposed;

        public UiModalDemoLauncher(
            UiModalView viewPrefab,
            UiModalPresenterFactory presenterFactory,
            IUiModalReadModel readModel)
        {
            windowHandle = new UiRuntimeWindowHandle<UiModalWindow, UiModalView>(
                viewPrefab,
                presenterFactory,
                nameof(UiModalWindow),
                LayoutTagId,
                takeFocus: true,
                "A WindowSystem must exist before showing the modal slice.");
            this.readModel = readModel ?? throw new ArgumentNullException(nameof(readModel));
        }

        public UiModalWindow CurrentWindow => windowHandle.CurrentWindow;

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
            windowHandle.Show();
        }

        public void Hide()
        {
            windowHandle.Hide();
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
            windowHandle.Dispose();
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

        private void ThrowIfDisposed()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(nameof(UiModalDemoLauncher));
            }
        }
    }
}

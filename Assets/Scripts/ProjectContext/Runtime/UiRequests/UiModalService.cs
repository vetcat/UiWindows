using System;
using R3;

namespace ProjectContext.UiRequests
{
    public sealed class UiModalService : IUiModalService, IDisposable
    {
        private readonly ReactiveProperty<UiModalRequest> currentModal = new(null);
        private int nextId;
        private bool disposed;

        public ReadOnlyReactiveProperty<UiModalRequest> CurrentModal => currentModal;
        public UiModalRequest CurrentRequest => currentModal.CurrentValue;

        public void ShowInfoOk(string caption, string description, Action handlerClose = null)
        {
            ThrowIfDisposed();
            currentModal.Value = Create(UiModalKind.InfoOk, caption, description, null, null, handlerClose);
        }

        public void ShowInfoOkCancel(
            string caption,
            string description,
            Action handlerOk = null,
            Action handlerCancel = null)
        {
            ThrowIfDisposed();
            currentModal.Value = Create(UiModalKind.InfoOkCancel, caption, description, handlerOk, handlerCancel, null);
        }

        public void ShowWait(string caption = "")
        {
            ThrowIfDisposed();
            if (currentModal.CurrentValue?.Kind == UiModalKind.Wait)
            {
                return;
            }

            currentModal.Value = Create(UiModalKind.Wait, caption, string.Empty, null, null, null);
        }

        public void HideWait()
        {
            ThrowIfDisposed();
            if (currentModal.CurrentValue?.Kind == UiModalKind.Wait)
            {
                currentModal.Value = null;
            }
        }

        public void CompleteCurrent(UiModalResult result)
        {
            ThrowIfDisposed();
            var request = currentModal.CurrentValue;
            if (request == null)
            {
                return;
            }

            currentModal.Value = null;
            InvokeCompletion(request, result);
        }

        public void Clear()
        {
            ThrowIfDisposed();
            currentModal.Value = null;
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            currentModal.Dispose();
        }

        private UiModalRequest Create(
            UiModalKind kind,
            string caption,
            string description,
            Action okHandler,
            Action cancelHandler,
            Action closeHandler)
        {
            nextId = nextId == int.MaxValue ? 1 : nextId + 1;
            return new UiModalRequest(nextId, kind, caption, description, okHandler, cancelHandler, closeHandler);
        }

        private static void InvokeCompletion(UiModalRequest request, UiModalResult result)
        {
            switch (request.Kind)
            {
                case UiModalKind.InfoOkCancel when result == UiModalResult.Ok:
                    request.OkHandler?.Invoke();
                    break;
                case UiModalKind.InfoOkCancel:
                    request.CancelHandler?.Invoke();
                    break;
                case UiModalKind.InfoOk:
                    request.CloseHandler?.Invoke();
                    break;
            }
        }

        private void ThrowIfDisposed()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(nameof(UiModalService));
            }
        }
    }
}
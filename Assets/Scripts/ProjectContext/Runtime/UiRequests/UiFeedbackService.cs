using System;
using R3;

namespace ProjectContext.UiRequests
{
    public sealed class UiFeedbackService : IUiFeedbackService, IDisposable
    {
        private readonly Subject<UiHintRequest> hintRequests = new();
        private readonly Subject<UiFxRequest> fxRequests = new();
        private bool disposed;

        public Observable<UiHintRequest> HintRequests => hintRequests;
        public Observable<UiFxRequest> FxRequests => fxRequests;

        public void ShowHint(
            string description,
            UiHintAnchor anchor = UiHintAnchor.Center,
            float durationSeconds = 1.5f)
        {
            ThrowIfDisposed();
            hintRequests.OnNext(new UiHintRequest(description, anchor, durationSeconds));
        }

        public void RequestCollectFx(int amount, UiFxTarget target = UiFxTarget.Coins)
        {
            ThrowIfDisposed();
            fxRequests.OnNext(new UiFxRequest(UiFxKind.Collect, amount, target));
        }

        public void RequestSpendFx(int amount, UiFxTarget target = UiFxTarget.Coins)
        {
            ThrowIfDisposed();
            fxRequests.OnNext(new UiFxRequest(UiFxKind.Spend, amount, target));
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            fxRequests.Dispose();
            hintRequests.Dispose();
        }

        private void ThrowIfDisposed()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(nameof(UiFeedbackService));
            }
        }
    }
}
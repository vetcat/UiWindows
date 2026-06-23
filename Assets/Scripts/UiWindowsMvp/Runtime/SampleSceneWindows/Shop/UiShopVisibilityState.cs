using System;
using R3;

namespace UiWindowsMvp.SampleSceneWindows
{
    public interface IUiShopVisibilityReadModel
    {
        ReadOnlyReactiveProperty<bool> IsShopVisible { get; }
    }

    public interface IUiShopVisibilityCommands
    {
        void SetShopVisible(bool visible);
    }

    public sealed class UiShopVisibilityState : IUiShopVisibilityReadModel, IUiShopVisibilityCommands, IDisposable
    {
        private readonly ReactiveProperty<bool> isShopVisible = new(false);
        private bool disposed;

        public ReadOnlyReactiveProperty<bool> IsShopVisible => isShopVisible;

        public void SetShopVisible(bool visible)
        {
            ThrowIfDisposed();
            isShopVisible.Value = visible;
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            isShopVisible.Dispose();
        }

        private void ThrowIfDisposed()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(nameof(UiShopVisibilityState));
            }
        }
    }
}

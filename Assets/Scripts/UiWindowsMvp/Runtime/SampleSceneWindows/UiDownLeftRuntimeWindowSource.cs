using System;

namespace UiWindowsMvp.SampleSceneWindows
{
    internal sealed class UiDownLeftRuntimeWindowSource : IDisposable
    {
        private const int LayoutTagId = 15;

        private readonly UiRuntimeWindowSource<UiDownLeftWindow, UiDownLeftView> source;

        private UiDownLeftRuntimeWindowSource(UiRuntimeWindowSource<UiDownLeftWindow, UiDownLeftView> source)
        {
            this.source = source ?? throw new ArgumentNullException(nameof(source));
        }

        public UiDownLeftWindow WindowSource => source.WindowSource;

        public static UiDownLeftRuntimeWindowSource Create(UiDownLeftView viewPrefab)
        {
            var source = UiRuntimeWindowSource<UiDownLeftWindow, UiDownLeftView>.Create(
                viewPrefab,
                nameof(UiDownLeftWindow),
                LayoutTagId,
                takeFocus: false);
            return new UiDownLeftRuntimeWindowSource(source);
        }

        public void Dispose()
        {
            source.Dispose();
        }
    }
}

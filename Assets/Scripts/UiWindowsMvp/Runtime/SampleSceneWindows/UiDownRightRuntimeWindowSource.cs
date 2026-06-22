using System;

namespace UiWindowsMvp.SampleSceneWindows
{
    internal sealed class UiDownRightRuntimeWindowSource : IDisposable
    {
        private const int LayoutTagId = 14;

        private readonly UiRuntimeWindowSource<UiDownRightWindow, UiDownRightView> source;

        private UiDownRightRuntimeWindowSource(UiRuntimeWindowSource<UiDownRightWindow, UiDownRightView> source)
        {
            this.source = source ?? throw new ArgumentNullException(nameof(source));
        }

        public UiDownRightWindow WindowSource => source.WindowSource;

        public static UiDownRightRuntimeWindowSource Create(UiDownRightView viewPrefab)
        {
            var source = UiRuntimeWindowSource<UiDownRightWindow, UiDownRightView>.Create(
                viewPrefab,
                nameof(UiDownRightWindow),
                LayoutTagId,
                takeFocus: false);
            return new UiDownRightRuntimeWindowSource(source);
        }

        public void Dispose()
        {
            source.Dispose();
        }
    }
}

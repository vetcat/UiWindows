using System;

namespace UiWindowsMvp.SampleSceneWindows
{
    internal sealed class UiTopRightRuntimeWindowSource : IDisposable
    {
        private const int LayoutTagId = 13;

        private readonly UiRuntimeWindowSource<UiTopRightWindow, UiTopRightView> source;

        private UiTopRightRuntimeWindowSource(UiRuntimeWindowSource<UiTopRightWindow, UiTopRightView> source)
        {
            this.source = source ?? throw new ArgumentNullException(nameof(source));
        }

        public UiTopRightWindow WindowSource => source.WindowSource;

        public static UiTopRightRuntimeWindowSource Create(UiTopRightView viewPrefab)
        {
            var source = UiRuntimeWindowSource<UiTopRightWindow, UiTopRightView>.Create(
                viewPrefab,
                nameof(UiTopRightWindow),
                LayoutTagId,
                takeFocus: false);
            return new UiTopRightRuntimeWindowSource(source);
        }

        public void Dispose()
        {
            source.Dispose();
        }
    }
}

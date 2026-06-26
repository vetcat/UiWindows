using System;

namespace UiWindowsMvp.SampleSceneWindows
{
    internal sealed class UiTopCenterRuntimeWindowSource : IDisposable
    {
        private const int LayoutTagId = 16;

        private readonly UiRuntimeWindowSource<UiTopCenterWindow, UiTopCenterView> source;

        private UiTopCenterRuntimeWindowSource(UiRuntimeWindowSource<UiTopCenterWindow, UiTopCenterView> source)
        {
            this.source = source ?? throw new ArgumentNullException(nameof(source));
        }

        public UiTopCenterWindow WindowSource => source.WindowSource;

        public static UiTopCenterRuntimeWindowSource Create(UiTopCenterView viewPrefab)
        {
            var source = UiRuntimeWindowSource<UiTopCenterWindow, UiTopCenterView>.Create(
                viewPrefab,
                nameof(UiTopCenterWindow),
                LayoutTagId,
                takeFocus: false);
            return new UiTopCenterRuntimeWindowSource(source);
        }

        public void Dispose()
        {
            source.Dispose();
        }
    }
}

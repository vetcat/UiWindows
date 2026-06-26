using System;

namespace UiWindowsMvp.SampleSceneWindows
{
    internal sealed class UiObjectIndicatorRuntimeWindowSource : IDisposable
    {
        private const int LayoutTagId = 22;

        private readonly UiRuntimeWindowSource<UiObjectIndicatorWindow, UiObjectIndicatorView> source;

        private UiObjectIndicatorRuntimeWindowSource(
            UiRuntimeWindowSource<UiObjectIndicatorWindow, UiObjectIndicatorView> source)
        {
            this.source = source ?? throw new ArgumentNullException(nameof(source));
        }

        public UiObjectIndicatorWindow WindowSource => source.WindowSource;

        public static UiObjectIndicatorRuntimeWindowSource Create(UiObjectIndicatorView viewPrefab)
        {
            var source = UiRuntimeWindowSource<UiObjectIndicatorWindow, UiObjectIndicatorView>.Create(
                viewPrefab,
                nameof(UiObjectIndicatorWindow),
                LayoutTagId,
                takeFocus: false);
            return new UiObjectIndicatorRuntimeWindowSource(source);
        }

        public void Dispose()
        {
            source.Dispose();
        }
    }
}

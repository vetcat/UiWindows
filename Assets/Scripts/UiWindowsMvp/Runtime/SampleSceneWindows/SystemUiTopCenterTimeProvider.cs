using System;
using R3;

namespace UiWindowsMvp.SampleSceneWindows
{
    public sealed class SystemUiTopCenterTimeProvider : IUiTopCenterTimeProvider
    {
        public static readonly TimeSpan DefaultUpdateInterval = TimeSpan.FromSeconds(0.5d);

        private readonly TimeSpan updateInterval;

        public SystemUiTopCenterTimeProvider()
            : this(DefaultUpdateInterval)
        {
        }

        internal SystemUiTopCenterTimeProvider(TimeSpan updateInterval)
        {
            if (updateInterval <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(updateInterval), updateInterval,
                    "Interval must be positive.");
            }

            this.updateInterval = updateInterval;
        }

        public DateTime UtcNow => DateTime.UtcNow;

        public IDisposable SubscribeUtcTimeChanged(Action callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            return Observable.Interval(updateInterval).Subscribe(_ => callback());
        }
    }
}

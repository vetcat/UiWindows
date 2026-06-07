using R3;
using UnityEngine;

namespace UiWindowsMvp.Reactive
{
    public static class R3MvpSmokeCheck
    {
        public static bool CanCreateReadOnlySurface()
        {
            using var mutable = new ReactiveProperty<int>(1);
            ReadOnlyReactiveProperty<int> readOnly = mutable.ToReadOnlyReactiveProperty();
            return readOnly.CurrentValue == 1;
        }

        public static T RegisterWithUnityLifetime<T>(T disposable, Component owner)
            where T : System.IDisposable
        {
            return disposable.AddTo(owner);
        }
    }
}

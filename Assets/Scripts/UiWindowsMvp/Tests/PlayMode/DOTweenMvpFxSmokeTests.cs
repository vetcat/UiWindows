using NUnit.Framework;
using UiWindowsMvp.SampleSceneWindows;
using UnityEngine;

namespace UiWindowsMvp.Tests.PlayMode
{
    public sealed class DOTweenMvpFxSmokeTests
    {
        [Test]
        public void SampleSceneWindowsAssemblyCanCreateUnityUiTween()
        {
            var owner = new GameObject("DOTweenMvpFxSmoke");

            try
            {
                var canvasGroup = owner.AddComponent<CanvasGroup>();

                Assert.That(DOTweenMvpFxSmokeCheck.CanCreateUnityUiTween(canvasGroup), Is.True);
            }
            finally
            {
                Object.DestroyImmediate(owner);
            }
        }
    }
}

using System;
using DG.Tweening;
using UnityEngine;

namespace UiWindowsMvp.SampleSceneWindows
{
    internal static class DOTweenMvpFxSmokeCheck
    {
        public static bool CanCreateUnityUiTween(CanvasGroup target)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            Tween tween = null;
            try
            {
                tween = target.DOFade(target.alpha, 0.01f);
                return tween != null;
            }
            finally
            {
                tween?.Kill();
            }
        }
    }
}

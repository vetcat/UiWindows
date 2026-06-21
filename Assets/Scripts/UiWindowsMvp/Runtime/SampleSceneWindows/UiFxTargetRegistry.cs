using System;
using System.Collections.Generic;
using ProjectContext.UiRequests;
using UnityEngine;

namespace UiWindowsMvp.SampleSceneWindows
{
    public interface IUiFxTargetResolver
    {
        bool TryGetTarget(UiFxTarget target, out RectTransform rectTransform);
    }

    public interface IUiFxTargetRegistry : IUiFxTargetResolver
    {
        IDisposable RegisterTarget(UiFxTarget target, RectTransform rectTransform);
    }

    public sealed class UiFxTargetRegistry : IUiFxTargetRegistry
    {
        private readonly Dictionary<UiFxTarget, RectTransform> targets = new();

        public bool TryGetTarget(UiFxTarget target, out RectTransform rectTransform)
        {
            if (targets.TryGetValue(target, out rectTransform) && rectTransform != null)
            {
                return true;
            }

            targets.Remove(target);
            rectTransform = null;
            return false;
        }

        public IDisposable RegisterTarget(UiFxTarget target, RectTransform rectTransform)
        {
            if (rectTransform == null)
            {
                throw new ArgumentNullException(nameof(rectTransform));
            }

            targets[target] = rectTransform;
            return new DisposableAction(() => RemoveTarget(target, rectTransform));
        }

        private void RemoveTarget(UiFxTarget target, RectTransform rectTransform)
        {
            if (targets.TryGetValue(target, out var registered) && registered == rectTransform)
            {
                targets.Remove(target);
            }
        }
    }
}

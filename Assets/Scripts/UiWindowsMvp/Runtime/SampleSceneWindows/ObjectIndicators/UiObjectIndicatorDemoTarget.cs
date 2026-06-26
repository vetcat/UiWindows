using System;
using UnityEngine;

namespace UiWindowsMvp.SampleSceneWindows
{
    public sealed class UiObjectIndicatorDemoTarget : IUiObjectIndicatorAnchor, IDisposable
    {
        private const float RewardOffsetY = 2f;
        private readonly GameObject targetObject;
        private bool disposed;

        public UiObjectIndicatorDemoTarget()
        {
            targetObject = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            targetObject.name = "UiObjectIndicator Reward Source";
            targetObject.transform.position = new Vector3(0f, 0f, 4f);
            targetObject.transform.localScale = new Vector3(0.8f, 1.2f, 0.8f);
        }

        public string DisplayName => "Character";
        public int RewardAmount => 25;
        public Transform TargetTransform => targetObject != null ? targetObject.transform : null;

        public void MoveTo(Vector3 position)
        {
            if (TargetTransform != null)
            {
                TargetTransform.position = position;
            }
        }

        public bool TryGetWorldPosition(out Vector3 worldPosition)
        {
            if (TargetTransform == null)
            {
                worldPosition = Vector3.zero;
                return false;
            }

            worldPosition = TargetTransform.position + Vector3.up * RewardOffsetY;
            return true;
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            DestroyObject(targetObject);
        }

        private static void DestroyObject(UnityEngine.Object target)
        {
            if (target == null)
            {
                return;
            }

            if (Application.isPlaying)
            {
                UnityEngine.Object.Destroy(target);
                return;
            }

            UnityEngine.Object.DestroyImmediate(target);
        }
    }
}

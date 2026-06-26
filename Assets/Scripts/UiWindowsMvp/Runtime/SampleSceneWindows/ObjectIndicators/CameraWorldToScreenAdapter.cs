using System;
using UnityEngine;

namespace UiWindowsMvp.SampleSceneWindows
{
    public sealed class CameraWorldToScreenAdapter : IUiWorldToScreenAdapter
    {
        private readonly Func<Camera> cameraResolver;

        public CameraWorldToScreenAdapter()
            : this(() => Camera.main)
        {
        }

        public CameraWorldToScreenAdapter(Camera camera)
            : this(() => camera)
        {
        }

        private CameraWorldToScreenAdapter(Func<Camera> cameraResolver)
        {
            this.cameraResolver = cameraResolver ?? throw new ArgumentNullException(nameof(cameraResolver));
        }

        public bool TryGetScreenPosition(Vector3 worldPosition, out Vector2 screenPosition)
        {
            var camera = cameraResolver();
            if (camera == null)
            {
                screenPosition = Vector2.zero;
                return false;
            }

            var screenPoint = camera.WorldToScreenPoint(worldPosition);
            if (screenPoint.z < 0f)
            {
                screenPosition = Vector2.zero;
                return false;
            }

            screenPosition = new Vector2(screenPoint.x, screenPoint.y);
            return true;
        }
    }
}

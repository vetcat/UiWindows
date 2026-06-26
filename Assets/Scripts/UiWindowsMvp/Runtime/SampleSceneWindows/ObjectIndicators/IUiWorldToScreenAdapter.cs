using UnityEngine;

namespace UiWindowsMvp.SampleSceneWindows
{
    public interface IUiWorldToScreenAdapter
    {
        bool TryGetScreenPosition(Vector3 worldPosition, out Vector2 screenPosition);
    }
}

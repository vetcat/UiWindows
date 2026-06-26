using UnityEngine;

namespace UiWindowsMvp.SampleSceneWindows
{
    public interface IUiObjectIndicatorAnchor
    {
        string DisplayName { get; }
        int RewardAmount { get; }
        bool TryGetWorldPosition(out Vector3 worldPosition);
    }
}

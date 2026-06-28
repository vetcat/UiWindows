using System;
using UnityEngine;

namespace UiWindowsMvp.SampleSceneWindows
{
    internal static class UiPrefabReferenceGuard
    {
        public static InvalidOperationException Missing(MonoBehaviour owner, string references)
        {
            var ownerType = owner != null ? owner.GetType().Name : "UI view";
            var ownerName = owner != null ? owner.name : "unknown";
            return new InvalidOperationException(
                ownerType + " on '" + ownerName +
                "' is missing prefab-assigned UI references: " + references +
                ". Use the project prefab asset instead of runtime fallback layout construction.");
        }

        public static InvalidOperationException MissingComponent(
            MonoBehaviour owner,
            string componentName,
            string objectPath)
        {
            var ownerType = owner != null ? owner.GetType().Name : "UI view";
            var ownerName = owner != null ? owner.name : "unknown";
            return new InvalidOperationException(
                ownerType + " on '" + ownerName +
                "' requires a prefab " + componentName + " component on " + objectPath + ".");
        }
    }
}

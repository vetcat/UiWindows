using UnityEngine;
using UnityEngine.UI.Windows;
using UnityEngine.UI.Windows.Modules;
using UnityEngine.UI.Windows.Utilities;

namespace UiWindowsMvp.SampleSceneWindows
{
    [DefaultExecutionOrder(-2000)]
    public sealed class UiTopLeftDemoWindowSystemSetup : MonoBehaviour
    {
        [SerializeField] private WindowSystemSettings settings;

        private WindowSystemSettings runtimeSettings;

        private void Awake()
        {
            var windowSystem = GetOrAdd<WindowSystem>();
            windowSystem.showRootOnStart = false;
            windowSystem.breadcrumbs = GetOrAdd<WindowSystemBreadcrumbs>();
            windowSystem.events = GetOrAdd<WindowSystemEvents>();
            windowSystem.resources = GetOrAdd<WindowSystemResources>();
            windowSystem.pools = GetOrAdd<WindowSystemPools>();
            windowSystem.tweener = GetOrAdd<Tweener>();
            windowSystem.settings = settings != null ? settings : CreateRuntimeSettings();
        }

        private void OnDestroy()
        {
            if (runtimeSettings != null)
            {
                Destroy(runtimeSettings);
                runtimeSettings = null;
            }
        }

        private T GetOrAdd<T>()
            where T : Component
        {
            var component = GetComponent<T>();
            return component != null ? component : gameObject.AddComponent<T>();
        }

        private WindowSystemSettings CreateRuntimeSettings()
        {
            runtimeSettings = ScriptableObject.CreateInstance<WindowSystemSettings>();
            runtimeSettings.name = "UiTopLeft Demo WindowSystem Settings";
            runtimeSettings.canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            runtimeSettings.components.renderBehaviourOnHidden = RenderBehaviour.HideGameObject;
            return runtimeSettings;
        }
    }
}
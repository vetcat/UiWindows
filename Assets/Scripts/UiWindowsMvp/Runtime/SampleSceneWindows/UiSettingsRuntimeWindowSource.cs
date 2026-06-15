using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Windows;
using UnityEngine.UI.Windows.WindowTypes;

namespace UiWindowsMvp.SampleSceneWindows
{
    internal sealed class UiSettingsRuntimeWindowSource : IDisposable
    {
        private const int MainLayoutTagId = 2;
        private const int MainLayoutLocalTagId = 1;

        private readonly WindowLayout layoutSource;
        private bool disposed;

        private UiSettingsRuntimeWindowSource(UiSettingsWindow windowSource, WindowLayout layoutSource)
        {
            WindowSource = windowSource;
            this.layoutSource = layoutSource;
        }

        public UiSettingsWindow WindowSource { get; }

        public static UiSettingsRuntimeWindowSource Create(UiSettingsView viewPrefab)
        {
            if (viewPrefab == null)
            {
                throw new ArgumentNullException(nameof(viewPrefab));
            }

            var layout = CreateLayoutSource();
            var window = CreateWindowSource(layout, viewPrefab);
            return new UiSettingsRuntimeWindowSource(window, layout);
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            DestroyObject(WindowSource != null ? WindowSource.gameObject : null);
            DestroyObject(layoutSource != null ? layoutSource.gameObject : null);
        }

        private static UiSettingsWindow CreateWindowSource(WindowLayout layout, UiSettingsView viewPrefab)
        {
            var gameObject = new GameObject("UiSettingsWindow Source", typeof(RectTransform));
            UnityEngine.Object.DontDestroyOnLoad(gameObject);

            var window = gameObject.AddComponent<UiSettingsWindow>();
            window.rectTransform = (RectTransform)gameObject.transform;
            window.isObjectRoot = true;
            window.createPool = true;
            window.preferences = WindowPreferences.Default;
            window.preferences.singleInstance = true;
            window.preferences.takeFocus = true;
            window.preferences.forceSyncLoad = true;
            window.preferences.renderMode = UIWSRenderMode.ScreenSpaceOverlay;
            window.preferences.cameraMode = UIWSCameraMode.Orthographic;

            var cameraObject = new GameObject("UiSettingsWindow Camera", typeof(Camera));
            UnityEngine.Object.DontDestroyOnLoad(cameraObject);
            cameraObject.transform.SetParent(gameObject.transform, false);
            var camera = cameraObject.GetComponent<Camera>();
            camera.enabled = false;
            camera.clearFlags = CameraClearFlags.Depth;
            camera.cullingMask = 0;
            window.workCamera = camera;

            window.layouts.items = new[]
            {
                new LayoutItem
                {
                    windowLayout = layout,
                    components = new[]
                    {
                        new LayoutItem.LayoutComponentItem
                        {
                            windowLayout = layout,
                            tag = MainLayoutTagId,
                            localTag = MainLayoutLocalTagId,
                            component = CreateDirectComponentResource(viewPrefab)
                        }
                    }
                }
            };

            return window;
        }

        private static WindowLayout CreateLayoutSource()
        {
            var gameObject = new GameObject(
                "UiSettingsWindow Layout Source",
                typeof(RectTransform),
                typeof(Canvas),
                typeof(CanvasScaler),
                typeof(GraphicRaycaster),
                typeof(WindowLayout));
            UnityEngine.Object.DontDestroyOnLoad(gameObject);

            var rectTransform = (RectTransform)gameObject.transform;
            Stretch(rectTransform);

            var canvas = gameObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var canvasScaler = gameObject.GetComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(1280f, 720f);
            canvasScaler.matchWidthOrHeight = 0.5f;
            canvasScaler.referencePixelsPerUnit = 100f;

            var layout = gameObject.GetComponent<WindowLayout>();
            layout.rectTransform = rectTransform;
            layout.objectCanvas = canvas;
            layout.canvas = canvas;
            layout.canvasScaler = canvasScaler;
            layout.isObjectRoot = true;

            var elementObject =
                new GameObject("UiSettingsWindow Slot", typeof(RectTransform), typeof(WindowLayoutElement));
            elementObject.transform.SetParent(gameObject.transform, false);
            var elementRectTransform = (RectTransform)elementObject.transform;
            Stretch(elementRectTransform);

            var element = elementObject.GetComponent<WindowLayoutElement>();
            element.rectTransform = elementRectTransform;
            element.rootObject = layout;
            element.tagId = MainLayoutTagId;

            layout.subObjects.Add(element);
            layout.layoutElements = new[] { element };

            return layout;
        }

        private static Resource CreateDirectComponentResource(UiSettingsView viewPrefab)
        {
            return new Resource
            {
                type = Resource.Type.Direct,
                objectType = Resource.ObjectType.Component,
                directRef = viewPrefab
            };
        }

        private static void Stretch(RectTransform rectTransform)
        {
            rectTransform.localScale = Vector3.one;
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = Vector2.zero;
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
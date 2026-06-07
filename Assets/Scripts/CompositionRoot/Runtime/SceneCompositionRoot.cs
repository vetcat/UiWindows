using System;
using System.Collections.Generic;
using UnityEngine;

namespace CompositionRoot.Runtime
{
    [DefaultExecutionOrder(-10000)]
    public sealed class SceneCompositionRoot : MonoBehaviour
    {
        [SerializeField]
        private bool installComponentsOnSameGameObject = true;

        [SerializeField]
        private MonoBehaviour[] installers = Array.Empty<MonoBehaviour>();

        private ServiceRegistry registry;

        public bool IsBootstrapped => registry != null && registry.IsInitialized && !registry.IsDisposed;

        public IServiceResolver Services
        {
            get
            {
                if (registry == null)
                {
                    throw new InvalidOperationException("SceneCompositionRoot is not bootstrapped.");
                }

                return registry;
            }
        }

        public void Bootstrap()
        {
            if (registry != null)
            {
                return;
            }

            var newRegistry = new ServiceRegistry();
            try
            {
                var orderedInstallers = CollectInstallers();
                for (var i = 0; i < orderedInstallers.Count; i++)
                {
                    orderedInstallers[i].Install(newRegistry);
                }

                newRegistry.InitializeAll();
                registry = newRegistry;
            }
            catch
            {
                newRegistry.Dispose();
                throw;
            }
        }

        public void Shutdown()
        {
            if (registry == null)
            {
                return;
            }

            registry.Dispose();
            registry = null;
        }

        private void Awake()
        {
            Bootstrap();
        }

        private void OnDestroy()
        {
            Shutdown();
        }

        private List<ICompositionInstaller> CollectInstallers()
        {
            var orderedInstallers = new List<ICompositionInstaller>();

            if (installers != null && installers.Length > 0)
            {
                for (var i = 0; i < installers.Length; i++)
                {
                    AddRequiredInstaller(orderedInstallers, installers[i], i);
                }

                return orderedInstallers;
            }

            if (!installComponentsOnSameGameObject)
            {
                return orderedInstallers;
            }

            var components = GetComponents<MonoBehaviour>();
            for (var i = 0; i < components.Length; i++)
            {
                if (ReferenceEquals(components[i], this))
                {
                    continue;
                }

                if (components[i] is ICompositionInstaller installer)
                {
                    orderedInstallers.Add(installer);
                }
            }

            return orderedInstallers;
        }

        private static void AddRequiredInstaller(List<ICompositionInstaller> orderedInstallers, MonoBehaviour candidate, int index)
        {
            if (candidate == null)
            {
                throw new InvalidOperationException($"Composition installer at index {index} is missing.");
            }

            if (candidate is ICompositionInstaller installer)
            {
                orderedInstallers.Add(installer);
                return;
            }

            throw new InvalidOperationException($"Component '{candidate.GetType().FullName}' does not implement {nameof(ICompositionInstaller)}.");
        }
    }
}

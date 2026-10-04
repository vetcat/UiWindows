using System;
using System.Collections.Generic;

namespace CompositionRoot.Runtime
{
    public sealed class ServiceRegistry : IServiceRegistry, IDisposable
    {
        private readonly Dictionary<Type, object> services = new Dictionary<Type, object>();
        private readonly List<object> registrationOrder = new List<object>();
        private bool initialized;
        private bool disposed;

        public bool IsInitialized => initialized;

        public bool IsDisposed => disposed;

        public void Register<TService>(TService service) where TService : class
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            ThrowIfLocked();

            var serviceType = typeof(TService);
            if (services.ContainsKey(serviceType))
            {
                throw new InvalidOperationException($"Service type '{serviceType.FullName}' is already registered.");
            }

            services.Add(serviceType, service);
            AddToLifecycleOrder(service);
        }

        public TService Resolve<TService>() where TService : class
        {
            ThrowIfDisposed();

            if (TryResolve<TService>(out var service))
            {
                return service;
            }

            throw new InvalidOperationException($"Service type '{typeof(TService).FullName}' is not registered.");
        }

        public bool TryResolve<TService>(out TService service) where TService : class
        {
            ThrowIfDisposed();

            if (services.TryGetValue(typeof(TService), out var value))
            {
                service = (TService)value;
                return true;
            }

            service = null;
            return false;
        }

        public void InitializeAll()
        {
            ThrowIfDisposed();

            if (initialized)
            {
                return;
            }

            initialized = true;
            for (var i = 0; i < registrationOrder.Count; i++)
            {
                if (registrationOrder[i] is IInitializable initializable)
                {
                    initializable.Initialize();
                }
            }
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            for (var i = registrationOrder.Count - 1; i >= 0; i--)
            {
                if (registrationOrder[i] is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }

            services.Clear();
            registrationOrder.Clear();
        }

        private void AddToLifecycleOrder(object service)
        {
            for (var i = 0; i < registrationOrder.Count; i++)
            {
                if (ReferenceEquals(registrationOrder[i], service))
                {
                    return;
                }
            }

            registrationOrder.Add(service);
        }

        private void ThrowIfLocked()
        {
            ThrowIfDisposed();

            if (initialized)
            {
                throw new InvalidOperationException("Services cannot be registered after initialization starts.");
            }
        }

        private void ThrowIfDisposed()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(nameof(ServiceRegistry));
            }
        }
    }
}

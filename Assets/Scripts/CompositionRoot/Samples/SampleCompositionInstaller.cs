using System;
using CompositionRoot.Runtime;
using UnityEngine;

namespace CompositionRoot.Samples
{
    public sealed class SampleCompositionInstaller : MonoBehaviour, ICompositionInstaller
    {
        [SerializeField]
        private string modelName = "CompositionRoot Sample";

        public void Install(IServiceRegistry registry)
        {
            var model = new SampleCounterModel(modelName);
            registry.Register(model);
            registry.Register<ISampleCounterService>(new SampleCounterService(model));
        }
    }

    public sealed class SampleCounterModel
    {
        public SampleCounterModel(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public int Count { get; private set; }

        public void Add(int amount)
        {
            Count += amount;
        }
    }

    public interface ISampleCounterService
    {
        string ModelName { get; }

        int Count { get; }

        bool IsInitialized { get; }

        void Increment();
    }

    public sealed class SampleCounterService : ISampleCounterService, IInitializable, IDisposable
    {
        private readonly SampleCounterModel model;
        private bool disposed;

        public SampleCounterService(SampleCounterModel model)
        {
            this.model = model ?? throw new ArgumentNullException(nameof(model));
        }

        public string ModelName => model.Name;

        public int Count => model.Count;

        public bool IsInitialized { get; private set; }

        public void Initialize()
        {
            ThrowIfDisposed();
            IsInitialized = true;
        }

        public void Increment()
        {
            ThrowIfDisposed();
            model.Add(1);
        }

        public void Dispose()
        {
            disposed = true;
        }

        private void ThrowIfDisposed()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(nameof(SampleCounterService));
            }
        }
    }
}

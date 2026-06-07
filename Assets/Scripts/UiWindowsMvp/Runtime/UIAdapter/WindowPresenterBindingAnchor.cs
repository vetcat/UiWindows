using System;
using UnityEngine;

namespace UiWindowsMvp.UIAdapter
{
    [DisallowMultipleComponent]
    public sealed class WindowPresenterBindingAnchor : MonoBehaviour
    {
        public IWindowPresenterBinding Binding { get; private set; }

        public bool HasBinding => Binding != null && Binding.IsDisposed == false;

        public void SetBinding(IWindowPresenterBinding binding)
        {
            Binding = binding ?? throw new ArgumentNullException(nameof(binding));
        }

        public void ClearBinding(IWindowPresenterBinding binding)
        {
            if (ReferenceEquals(Binding, binding))
            {
                Binding = null;
            }
        }

        private void OnDestroy()
        {
            Binding?.Dispose();
            Binding = null;
        }
    }
}

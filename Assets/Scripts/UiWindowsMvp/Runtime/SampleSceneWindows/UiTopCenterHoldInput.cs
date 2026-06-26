using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UiWindowsMvp.SampleSceneWindows
{
    public sealed class UiTopCenterHoldInput : MonoBehaviour, IPointerDownHandler, IPointerUpHandler,
        IPointerExitHandler
    {
        public event Action PointerDown;
        public event Action PointerUp;
        public event Action PointerExit;

        public void OnPointerDown(PointerEventData eventData)
        {
            PointerDown?.Invoke();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            PointerUp?.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            PointerExit?.Invoke();
        }
    }
}

// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using UnityEngine;

#if ENABLE_WINMD_SUPPORT
using UnityEngine.XR.WSA.Input;
#endif

namespace HoloCore
{
    public class TapRecognizer : MonoBehaviour
    {
        public event Action Tap;
#if ENABLE_WINMD_SUPPORT
        private GestureRecognizer _recognizer;
#endif

        private void Awake()
        {
#if ENABLE_WINMD_SUPPORT
            _recognizer = new GestureRecognizer();
            _recognizer.Tapped += args =>
            {
                Tap?.Invoke();
            };
#endif
        }

        private void OnEnable()
        {
#if ENABLE_WINMD_SUPPORT
            _recognizer.StartCapturingGestures();
#endif
        }

        private void OnDisable()
        {
#if ENABLE_WINMD_SUPPORT
            _recognizer.StopCapturingGestures();
#endif
        }
    }
}
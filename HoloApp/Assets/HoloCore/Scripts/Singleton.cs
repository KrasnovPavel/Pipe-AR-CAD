// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace HoloCore
{
    /// <summary>
    /// Inherit from this base class to create a singleton.
    /// e.g. public class MyClassName : Singleton<MyClassName> {}
    /// </summary>
    [SuppressMessage("ReSharper", "InvalidXmlDocComment")]
    [SuppressMessage("ReSharper", "StaticMemberInGenericType")]
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        /// <summary> Access singleton instance through this propriety. </summary>
        public static T Instance
        {
            get
            {
                if (_shuttingDown)
                {
                    Debug.LogWarning($"[Singleton] Instance '{typeof(T)}' already destroyed. Returning null.");
                    return null;
                }
 
                lock (_lock)
                {
                    if (_instance != null) return _instance;
                
                    // Search for existing instance.
                    _instance = (T)FindObjectOfType(typeof(T));
 
                    // Create new instance if one doesn't already exist.
                    if (_instance != null) return _instance;
                
                    // Need to create a new GameObject to attach the singleton to.
                    GameObject singletonObject = new GameObject();
                    _instance = singletonObject.AddComponent<T>();
                    singletonObject.name = $"{typeof(T)} (Singleton)";
 
                    // Make instance persistent.
                    DontDestroyOnLoad(singletonObject);

                    return _instance;
                }
            }
        }

        #region Private definitions
        
        // Check to see if we're about to be destroyed.
        private static bool _shuttingDown;
        private static object _lock = new object();
        private static T _instance;
        
        private void OnApplicationQuit() //-V3013
        {
            _shuttingDown = true;
        }
 
        private void OnDestroy()
        {
            _shuttingDown = true;
        }
        
        #endregion
    }
}
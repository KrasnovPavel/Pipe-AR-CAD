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
                if (shuttingDown)
                {
                    Debug.LogWarning($"[Singleton] Instance '{typeof(T)}' already destroyed. Returning null.");
                    return null;
                }
 
                lock (Lock)
                {
                    if (instance != null) return instance;
                
                    // Search for existing instance.
                    instance = (T)FindObjectOfType(typeof(T));
 
                    // Create new instance if one doesn't already exist.
                    if (instance != null) return instance;
                
                    // Need to create a new GameObject to attach the singleton to.
                    GameObject singletonObject = new GameObject();
                    instance = singletonObject.AddComponent<T>();
                    singletonObject.name = $"{typeof(T)} (Singleton)";
 
                    // Make instance persistent.
                    DontDestroyOnLoad(singletonObject);

                    return instance;
                }
            }
        }

        #region Private definitions
        
        // Check to see if we're about to be destroyed.
        private static          bool   shuttingDown;
        private static readonly object Lock = new object();
        private static          T      instance;
        
        private void OnApplicationQuit() //-V3013
        {
            shuttingDown = true;
        }
 
        private void OnDestroy()
        {
            shuttingDown = true;
        }
        
        #endregion
    }
}
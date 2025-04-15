using UnityEngine;

namespace Singleton
{
    /// <summary>
    /// Generic singleton implementation for MonoBehaviours.
    /// Inherit from this class to make your MonoBehaviour a singleton.
    /// </summary>
    /// <typeparam name="T">The type of the singleton class</typeparam>
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        // The static instance of the singleton
        private static T _instance;
    
        /// <summary>
        /// Global access point to the singleton instance
        /// </summary>
        public static T Instance
        {
            get
            {
                // Check if instance exists
                if (_instance == null)
                {
                    // Search for existing instance in scene
                    _instance = FindObjectOfType<T>();
                
                    // If no instance exists, create a new GameObject with the component
                    if (_instance == null)
                    {
                        GameObject singletonObject = new GameObject();
                        _instance = singletonObject.AddComponent<T>();
                        singletonObject.name = $"{typeof(T)} (Singleton)";
                    }
                }
            
                return _instance;
            }
        }
    
        /// <summary>
        /// Initialize the singleton instance
        /// </summary>
        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }
    }
}

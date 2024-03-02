using UnityEngine;

namespace Project
{
    public abstract class SingletonComponent<T> : MonoBehaviour where T : SingletonComponent<T>, new()
    {
        public static T Instance => _instance;
     
        protected static T _instance;
        
        private void OnEnable()
        {
            if (_instance != null)
            {
                Debug.LogError("Multiple instances of the singleton detected. Only one is allowed.");
            }

            _instance = this as T;
        }
    }

}
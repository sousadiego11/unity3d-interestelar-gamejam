using UnityEngine;

namespace AGOUtils {
    public class ExtensibleSingleton<T> : MonoBehaviour where T : MonoBehaviour {
        public static T Singleton {get; private set;}

        void Awake() {
            if (Singleton != null && Singleton != this) {
                Destroy(this);
            } else {
                Singleton = this as T;
            }
        }
    }   
}

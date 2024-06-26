using UnityEngine;

//Singleton class can be inherited by other classes when needed
public class Singleton<T> : MonoBehaviour where T : Singleton<T> {
    private static T instance;
    public static T Instance { get { return instance; } }

    protected virtual void Awake () {
        if (instance != null) {
            Destroy(this.gameObject);
        } else {
            instance = (T)this;
        }
        DontDestroyOnLoad(gameObject);
    }

    protected virtual void OnDestroy () {
        if (instance == this) {
            instance = null;
        }
    }
}
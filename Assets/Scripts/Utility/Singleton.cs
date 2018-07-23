using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T singleton;

    public static T Instance
    {
        get
        {
            return singleton;
        }
    }

    public static bool IsInitialized
    {
        get
        {
            return singleton != null;
        }
    }

    protected virtual void Awake()
    {
        if (singleton != null)
        {
            Debug.LogErrorFormat("[Singleton] Trying to instantiate a second instance of singleton class {0}", GetType().Name);
        }
        else
        {
            singleton = (T)this;
        }
    }

    protected virtual void OnDestroy()
    {
        if (singleton == this)
        {
            singleton = null;
        }
    }
}

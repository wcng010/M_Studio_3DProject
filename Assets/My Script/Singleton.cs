using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T:Singleton<T>//约束T只能是本类
{
    private static T instance;
    public static T Instance
    {
        get { return instance;}
        
    }
    protected virtual void Awake()
    {if (instance != null)
            Destroy(gameObject);
        else instance = (T)this;
    }

    public static bool IsInitialized
    {
        get { return instance != null;}
    }
    protected virtual void Destroy()
    {
        if (instance == this)
        { instance = null; }
    }
}


using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Mono singleton Class. Extend this class to make singleton component.
/// Example: 
/// <code>
/// public class Foo : MonoSingleton<Foo>
/// </code>. To get the instance of Foo class, use <code>Foo.instance</code>
/// Override <code>Init()</code> method instead of using <code>Awake()</code>
/// from this class.
/// </summary>
public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    private static T m_Instance = null;
    public static T Instance
    {
        get
        {
            // Instance requiered for the first time, we look for it
            if (m_Instance == null)
            {
                m_Instance = GameObject.FindObjectOfType(typeof(T)) as T;

                // Object not found, we create a temporary one
                if (m_Instance == null)
                {
#if UNITY_EDITOR
                    Debug.LogWarning("No instance of " + typeof(T).ToString() + ", a temporary one is created.");
#endif
                    isTemporaryInstance = true;
                    m_Instance = new GameObject("Temp Instance of " + typeof(T).ToString(), typeof(T)).GetComponent<T>();

                    // Problem during the creation, this should not happen
                    if (m_Instance == null)
                    {
#if UNITY_EDITOR
                        Debug.LogError("Problem during the creation of " + typeof(T).ToString());
#endif
                    }
                }
                if (!_isInitialized)
                {
                    _isInitialized = true;
                    m_Instance.Init();
                }
            }
            return m_Instance;
        }
    }

    public static bool isTemporaryInstance { private set; get; }

    private static bool _isInitialized;

    public static bool _IsDontDestroyOnLoad = true;
    // If no other monobehaviour request the instance in an awake function
    // executing before this one, no need to search the object.
    private void Awake()
    {
        if (m_Instance == null)
        {
            m_Instance = this as T;
        }
        else if (m_Instance != this)
        {
#if UNITY_EDITOR
            Debug.LogError("Another instance of " + GetType() + " is already exist! Destroying self...");
#endif
            DestroyImmediate(this);
            return;
        }
        if (!_isInitialized)
        {
            _isInitialized = true;
            m_Instance.Init();

            if (_IsDontDestroyOnLoad && transform.parent == null)
                DontDestroyOnLoad(gameObject);
        }
    }

    /// <summary>
    /// This function is called when the instance is used the first time
    /// Put all the initializations you need here, as you would do in Awake
    /// </summary>
    public virtual void Init() { }

    /// Make sure the instance isn't referenced anymore when the user quit, just in case.
    private void OnApplicationQuit()
    {
        m_Instance = null;
    }

    public static bool HasInstance()
    {
        return m_Instance != null;
    }
}


public class SingletonMonoBehaviour<T> : MonoBehaviour where T : SingletonMonoBehaviour<T>
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            CreateInstance();
            return _instance;
        }
    }

    public static void CreateInstance()
    {
        if (_instance == null)
        {
            //find existing instance
            _instance = FindObjectOfType<T>();
            if (_instance == null)
            {
                //create new instance
                var go = new GameObject(typeof(T).Name);
                _instance = go.AddComponent<T>();
            }
            //initialize instance if necessary
            if (!_instance.initialized)
            {
                _instance.Initialize();
                _instance.initialized = true;
            }
        }
    }

    public virtual void Awake()
    {
        if (Application.isPlaying)
        {
            DontDestroyOnLoad(this);
        }

        //check if instance already exists when reloading original scene
        if (_instance != null)
        {
            DestroyImmediate(gameObject);
        }
    }

    protected bool initialized;

    protected virtual void Initialize() { }
}

//========================================================
// class BaseSingleton
//========================================================
// - for making singleton object
// - usage
//		+ declare class(derived )	
//			public class OnlyOne : BaseSingleton< OnlyOne >
//		+ client
//			OnlyOne.Instance.[method]
//========================================================
public abstract class Singleton<T> where T : new()
{
    protected static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new T();
            }
            return instance;
        }
    }

    public static bool IsInstanceValid()
    {
        return instance != null;
    }
}

/// <summary>
/// Singleton for mono behavior object, only return exsited object, don't create new
/// </summary>
/// <typeparam name="T"></typeparam>
public class ManualSingletonMono<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T instance;
    public static T Instance
    {
        get
        {
            //if(instance == null)
            //	Debug.LogWarning("Khong tim thay Object chua mono nay. Phai add san object nay vao scene " + typeof(T));
            return instance;
        }
    }

    public static bool IsInstanceValid()
    {
        //		if(instance == null)
        //			Debug.LogWarning("Khong tim thay mono nay. Phai add san object nay vao scene  " + typeof(T));
        return instance != null;
    }

    //Tam thoi ko chep Awake tai lop con, neu muon phai virtual ham nay
    public virtual void Awake()
    {
        if (instance != null)
        {
            GameObject.DestroyImmediate(this);
            return;
        }

        if (instance == null)
            instance = (T)(MonoBehaviour)this;

        if (instance == null)
        {
            Debug.LogError("Awake xong van NULL " + typeof(T));
        }

    }

    protected virtual void OnDestroy()
    {
        //Debug.LogError ("OnDestroy " + typeof(T));
        if (instance == this)
            instance = null;
    }
}

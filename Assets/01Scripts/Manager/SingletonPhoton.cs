using UnityEngine;
using Photon.Pun;

public class SingletonPhoton<T> : MonoBehaviourPunCallbacks where T : MonoBehaviourPunCallbacks
{
    private static T _instance;
    private static readonly object _lock = new object();
    private static bool _applicationIsQuitting = false;

    public static T Instance
    {
        get
        {
            if (_applicationIsQuitting)
            {
                Debug.LogWarning($"[SingletonPhoton] Instance {typeof(T)} already destroyed. Returning null.");
                return null;
            }

            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = (T)FindObjectOfType(typeof(T));

                    if (FindObjectsOfType(typeof(T)).Length > 1)
                    {
                        Debug.LogError($"[SingletonPhoton] Something went really wrong - there should never be more than 1 singleton! Reopening the scene might fix it.");
                        return _instance;
                    }

                    if (_instance == null)
                    {
                        GameObject singleton = new GameObject();
                        _instance = singleton.AddComponent<T>();
                        singleton.name = $"(SingletonPhoton) {typeof(T)}";

                        DontDestroyOnLoad(singleton);

                        Debug.Log($"[SingletonPhoton] An instance of {typeof(T)} is needed in the scene, so '{singleton}' was created.");
                    }
                    else
                    {
                        Debug.Log($"[SingletonPhoton] Using instance already created: {_instance.gameObject.name}");
                    }
                }

                return _instance;
            }
        }
    }

    private void OnDestroy()
    {
        _applicationIsQuitting = true;
    }
}

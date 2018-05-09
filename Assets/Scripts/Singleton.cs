using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Component
{
    public static T Instance { private set; get; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this as T;
            //DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }
}
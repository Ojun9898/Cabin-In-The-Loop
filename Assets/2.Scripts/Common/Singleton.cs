using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<T>();

            if (_instance == null)
                Debug.LogError($"[Singleton] Instance of {typeof(T)} not found in the scene.");

            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
        }
        else if (_instance != this)
        {
            // ★ 프리팹에 붙은 중복 컴포넌트만 제거
            Destroy(this);
            return;
        }
    }
}

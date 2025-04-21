using UnityEngine;

public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                T obj = FindObjectOfType<T>();

                if (obj == null)
                {
                    GameObject newObj = new GameObject();
                    newObj.AddComponent<T>();
                    
                    instance = newObj.GetComponent<T>();
                }
                else
                    instance = obj.GetComponent<T>();

            }

            return instance;
        }
    }

    protected virtual void Awake()
    {
        if (Instance == null)
        {
            instance = this as T;
            // DontDestroyOnLoad(this.gameObject);
        }
        // else
        //     Destroy(this.gameObject);
    }
}
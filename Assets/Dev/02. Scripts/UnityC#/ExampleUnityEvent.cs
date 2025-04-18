using UnityEngine;
using UnityEngine.Events;

public class ExampleUnityEvent : MonoBehaviour
{
    public UnityEvent unityEvent;

    void Start()
    {
        unityEvent.AddListener(delegate
        {
            OnLog1("Hello");
            OnLog2();
            OnLog3();
        });
        
        unityEvent.AddListener(OnLog2);
        
        unityEvent.RemoveListener(delegate
        {
            OnLog2();
        });

        unityEvent.RemoveAllListeners();
    }

    public void OnLog1(string msg)
    {
        Debug.Log("Hello");
    }
    
    public void OnLog2()
    {
        Debug.Log("몬스터 죽음");
    }
    
    public void OnLog3()
    {
        Debug.Log("Hello");
    }
}
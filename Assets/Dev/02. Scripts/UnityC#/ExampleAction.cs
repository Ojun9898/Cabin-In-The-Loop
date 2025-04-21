using System;
using UnityEngine;
using UnityEngine.Events;

public class ExampleAction : MonoBehaviour
{
    public UnityEvent unityEvent;

    public UnityAction unityAction;

    void Start()
    {
        unityAction += OnMethod1;
        unityAction += OnMethod2;
        unityAction += OnMethod3;

        unityEvent.AddListener(unityAction);
    }

    public void OnMethod1()
    {
        Debug.Log("Method1");
    }
    
    public void OnMethod2()
    {
        Debug.Log("Method2");
    }
    
    public void OnMethod3()
    {
        Debug.Log("Method3");
    }
}
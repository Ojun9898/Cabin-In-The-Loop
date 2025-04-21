using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleEvent : MonoBehaviour
{
    public delegate void Inputkey(string s);
    public event Inputkey onInputKey;

    void Start()
    {
        onInputKey = Respond;
    }

    void Update()
    {
        if (Input.anyKeyDown)
        {
            onInputKey?.Invoke("Hello");
        }
    }

    public void Respond(string msg)
    {
        Debug.Log(msg);
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;

public class ExampleFunc : MonoBehaviour
{
    public List<Func<int, int, int>> calculations = new List<Func<int, int, int>>();

    void Start()
    {
        calculations.Add((x, y) => x + y);
        calculations.Add((x, y) => x - y);

        foreach (var func in calculations)
        {
            int result = func(10, 20);
            Debug.Log(result);
        }
    }
}
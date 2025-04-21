using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleIterator : MonoBehaviour
{
    private bool isPlay = true;
    
    void Start()
    {
        foreach (var number in Numbers())
        {
            Debug.Log(number);
        }

        StartCoroutine(Routine());
    }

    void Update()
    {
        if (!isPlay) return;
        
        // 플레이 했을 때 기능
    }
    
    void Method()
    {
        Debug.Log("기능1");
        Debug.Log("기능2");

        return;
        
        Debug.Log("기능3");
    }

    IEnumerable<int> Numbers()
    {
        yield return 1;
        yield return 2;
        yield return 3;
        yield return 4;
        yield return 5;
    }

    IEnumerator Routine()
    {
        yield return null;
        // 어떠한 기능

        yield return new WaitForSeconds(1f);
        // 어떠한 기능
    }
}
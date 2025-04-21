using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoroutineEx : MonoBehaviour
{
    public List<int> intList = new List<int>();
    
    IEnumerator Start()
    {
        yield return null;

        for (int i = 1; i <= 10; i++)
        {
            intList.Add(i);
        }

        intList.Insert(3, 100);

        intList.RemoveAt(3);
    }
}
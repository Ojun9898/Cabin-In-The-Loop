using System;
using System.Collections.Generic;
using UnityEngine;

public class SingtonEx : MonoBehaviour
{
    private static SingtonEx instance;
    public static SingtonEx Instance
    {
        get
        {
            if (instance == null)
            {
                SingtonEx obj = FindObjectOfType<SingtonEx>();

                if (obj == null)
                {
                    GameObject newObj = new GameObject("SingletonEx");
                    newObj.AddComponent<SingtonEx>();
                    
                    instance = newObj.GetComponent<SingtonEx>();
                }
                else
                    instance = obj.GetComponent<SingtonEx>();

            }

            return instance;
        }
    }

    void Awake()
    {
        if (Instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
    }
    
}
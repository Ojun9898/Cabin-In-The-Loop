using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MONSTER"))
        {
            other.GetComponent<Monster>().OnHit(1);
        }
    }
}
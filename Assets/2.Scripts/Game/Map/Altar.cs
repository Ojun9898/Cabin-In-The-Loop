using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Altar : MonoBehaviour
{
    [HideInInspector] public bool isPlayerInAltar = false;

    private void OnTriggerEnter(Collider other)
    {
        isPlayerInAltar = true;
        Debug.Log("Player in Altar");
    }

    private void OnTriggerExit(Collider other)
    {
        isPlayerInAltar = false;
        Debug.Log("Player out Altar");
    }
}

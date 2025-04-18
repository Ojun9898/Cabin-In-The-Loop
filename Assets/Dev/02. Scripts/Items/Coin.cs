using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Coin : MonoBehaviour, IItem
{
    private ItemManager itemManager;
    
    void Start()
    {
        itemManager = FindObjectOfType<ItemManager>();
    }
    
    void OnMouseDown()
    {
        itemManager.GetItem(this.gameObject);
        this.gameObject.SetActive(false);
    }

    public void Use()
    {
        Debug.Log("코인 사용");
    }
}
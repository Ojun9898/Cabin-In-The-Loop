using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public List<GameObject> items = new List<GameObject>();
    
    public List<GameObject> getItems = new List<GameObject>();
    
    public GameObject CreateItem()
    {
        int randomInt = Random.Range(0, items.Count);

        GameObject item = Instantiate(items[randomInt]);
        
        return item;
    }

    public void GetItem(GameObject item)
    {
        getItems.Add(item);
    }
}
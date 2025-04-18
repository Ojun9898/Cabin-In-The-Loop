using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MagicStone : MonoBehaviour, IItem
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
        Debug.Log("마석 사용 -> 마나++");
    }
}
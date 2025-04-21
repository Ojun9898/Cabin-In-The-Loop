using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Potion : MonoBehaviour, IItem
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
        Debug.Log("포션 사용 -> 체력++");
    }
}
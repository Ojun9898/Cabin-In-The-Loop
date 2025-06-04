using UnityEngine;

public class DroppedGold : MonoBehaviour
{
    private int amount;
    private ItemData itemData;
    public void Initialize(int goldamount)
    {
        amount = goldamount;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
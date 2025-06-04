using UnityEngine;

public class DroppedItem : MonoBehaviour
{
    private ItemData itemData;
    private int count;

    public void Initialize(ItemData data, int count)
    {
        itemData = data;
        this.count = count;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
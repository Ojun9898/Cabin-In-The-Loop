using UnityEngine;

public enum PickupType { Gold, Potion, Material, Equipment }

public class ItemPickup : MonoBehaviour
{
    public PickupType type = PickupType.Gold;
    public int amount = 1;
    public float autoDestroyAfter = 120f; // 2분 뒤 정리 (옵션)

    void Start()
    {
        if (autoDestroyAfter > 0) Destroy(gameObject, autoDestroyAfter);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // 여기를 프로젝트의 시스템에 맞게 연결
        // 예: PlayerStatus.Instance.AddGold(amount);
        Debug.Log($"Picked: {type} x{amount}");

        Destroy(gameObject);
    }
}
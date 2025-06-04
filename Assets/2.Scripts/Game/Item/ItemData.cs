using UnityEngine;

[System.Serializable]
public class ItemData
{
    public string itemName; // 아이템 이름
    public ItemType type; // 아이템 타입(무기, 방어구, 악세서리 등)
    public string description; // 아이템 설명
    public GameObject itemprefab; // 아이템 프리팹
}
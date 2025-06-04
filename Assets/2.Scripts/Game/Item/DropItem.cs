[System.Serializable]
public class DropItem
{
    public ItemData itemData; // 드롭될 아이템의 데이터
    public float dropRate; // 드롭 확률(0~1)
    public int minCount; // 최소 드롭 개수
    public int maxCount; // 최대 드롭 개수
}
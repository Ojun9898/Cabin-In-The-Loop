using System.Collections.Generic;

[System.Serializable]
public class MonsterDropTable
{
    public List<DropItem> weaponDrops;
    public List<DropItem> armorDrops;
    public List<DropItem> accessoryDrops;
    public int ritualFragmentAmount;        // 의식 조각 개수 (100% 드롭)
    public int maxDropCount = 2;            // 최대 드롭 개수
}
using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "MonsterDropEvent", menuName = "Game/Monster Drop Event")]
public class MonsterDropEvent : ScriptableObject
{
    [Serializable]
    public class DropItem
    {
        [Header("드랍 아이템 프리팹")]
        public GameObject prefab;

        [Header("드랍 가중치 (높을수록 잘 나옴)")]
        [Min(0f)] public float weight = 1f;

        [Header("드랍 확률 (0~1)")]
        [Range(0f, 1f)] public float dropChance = 1f;

        [Header("수량 범위 (x~y개)")]
        public Vector2Int quantityRange = new Vector2Int(1, 1);
    }

    [Tooltip("몬스터가 죽을 때 드랍할 수 있는 아이템 목록")]
    public List<DropItem> dropItems = new List<DropItem>();

    /// <summary>
    /// 드랍 아이템 하나를 확률적으로 뽑음
    /// </summary>
    public bool TryGetDrop(out DropItem result)
    {
        result = null;
        if (dropItems == null || dropItems.Count == 0)
            return false;

        // 1차 필터: dropChance
        var candidates = new List<DropItem>();
        foreach (var item in dropItems)
        {
            if (item.prefab == null || item.weight <= 0) continue;
            if (Random.value <= item.dropChance)
                candidates.Add(item);
        }

        if (candidates.Count == 0) return false;

        // 2차 추첨: 가중치 기반 선택
        float totalWeight = 0;
        foreach (var c in candidates)
            totalWeight += c.weight;

        float r = Random.value * totalWeight;
        foreach (var c in candidates)
        {
            r -= c.weight;
            if (r <= 0)
            {
                result = c;
                return true;
            }
        }

        result = candidates[candidates.Count - 1];
        return true;
    }
}
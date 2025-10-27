using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoundData", menuName = "Game/Round Data")]
// ScriptableObject : 씬에 종속되지 않는 데이터 컨테이너
public class RoundData : ScriptableObject
{
    [System.Serializable]
    public struct MonsterCount
    {
        public MonsterType type;    
        public int count;
    }
    
    [Tooltip("이 라운드에서 스폰할 몬스터 목록 및 수량")]
    public List<MonsterCount> monsters;
}

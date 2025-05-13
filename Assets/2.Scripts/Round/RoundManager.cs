using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class RoundManager : MonoBehaviour
{
    private static RoundManager _instance;
    
    [Header("라운드별 데이터 (ScriptableObjects)")]
    public List<RoundData> roundDatas;

    [Header("스폰 관리자 참조")] public SpawnManager spawnManager;

    [Header("몬스터 스폰 간격 (초)")] public float spawnInterval = 3.5f;

    private Coroutine spawnRoutine;
    private int currentRound = 0;
    
    private void Awake()
    {
        // 이미 인스턴스가 있고, 그게 나(this)가 아니라면
        if (_instance != null && _instance != this)
        {
            // 이전에 남아있던 RoundManager를 파괴
            Destroy(_instance.gameObject);
        }

        // 이 인스턴스를 대표로 갱신
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Start 트리거가 발동되면 호출
    /// </summary>
    public void StartRounds(int startIndex)
    {
        spawnManager.ReturnAllToPool();
        
        if (spawnRoutine != null)
            StopCoroutine(spawnRoutine);
        
        currentRound = startIndex;
        spawnRoutine = StartCoroutine(RunRounds());
    }

    /// <summary>
    /// 각 라운드를 순서대로 돌며,
    /// 첫 소환까지 spawnInterval 대기 → 소환 → spawnInterval 대기 → 소환 … 
    /// Count만큼만 소환하고 다음 라운드로 넘어감
    /// </summary>
    private IEnumerator RunRounds()
    {
        RoundData data = roundDatas[currentRound];
        yield return new WaitForSeconds(spawnInterval);

        // ③ 설정된 타입·수량만큼 스폰
        foreach (var info in data.monsters)
        {
            for (int i = 0; i < info.count; i++)
            {
                spawnManager.Spawn(info.type, currentRound);
                yield return new WaitForSeconds(spawnInterval);
            }
        }
    }
}

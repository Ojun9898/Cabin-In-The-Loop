using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;



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
        
        // 현재 재생 중인 모든 몬스터 SFX 중단 & 풀로 리셋
        MonsterSFXManager.Instance.StopAllSounds();
        
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
        // 1) 현재 라운드 데이터 가져오기
        RoundData data = roundDatas[currentRound];

        // 2) 타입별 수량만큼 MonsterType 리스트에 추가
        List<MonsterType> spawnQueue = data.monsters
            .SelectMany(info => Enumerable.Repeat(info.type, info.count))
            .ToList();

        // 3) Fisher–Yates 알고리즘으로 리스트 섞기 (혹은 LINQ .OrderBy(x => Random.value))
        for (int i = spawnQueue.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            var tmp = spawnQueue[i];
            spawnQueue[i] = spawnQueue[j];
            spawnQueue[j] = tmp;
        }

        // 4) 딜레이 후, 섞인 순서대로 스폰
        yield return new WaitForSeconds(spawnInterval);
        foreach (var type in spawnQueue)
        {
            spawnManager.Spawn(type, currentRound);
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}

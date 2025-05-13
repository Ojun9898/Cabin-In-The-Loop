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
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);  
        }
        else
        {
            Destroy(gameObject);            
            return;
        }
    }

    /// <summary>
    /// Start 트리거가 발동되면 호출
    /// </summary>
    public void StartRounds(int startIndex)
    {
        if (spawnRoutine != null)
            StopCoroutine(spawnRoutine);
        
        currentRound = 0;
        spawnRoutine = StartCoroutine(RunRounds());
    }

    /// <summary>
    /// 각 라운드를 순서대로 돌며,
    /// 첫 소환까지 spawnInterval 대기 → 소환 → spawnInterval 대기 → 소환 … 
    /// Count만큼만 소환하고 다음 라운드로 넘어감
    /// </summary>
    private IEnumerator RunRounds()
    {
        // ① 전달받은 currentRound (예: 0) 의 데이터만 꺼내고
        RoundData data = roundDatas[currentRound];

        // ② 첫 소환 전 3.5초 대기
        yield return new WaitForSeconds(spawnInterval);

        // ③ 설정된 타입·수량만큼 스폰
        foreach (var info in data.monsters)
        {
            for (int i = 0; i < info.count; i++)
            {
                spawnManager.Spawn(info.type);
                yield return new WaitForSeconds(spawnInterval);
            }
        }
    }
}

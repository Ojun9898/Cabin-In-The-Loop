using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class RoundManager : MonoBehaviour
{
    [Header("라운드별 데이터 (ScriptableObjects)")]
    public List<RoundData> roundDatas;
    
    [Header("스폰 관리자 참조")]
    public SpawnManager spawnManager; 

    [Header("몬스터 스폰 간격 (초)")]
    public float spawnInterval = 3.5f;

    private int currentRound = 0;

    public void StartRounds()
    {
        currentRound = 0;
        StartCoroutine(RunAllRounds());
    }
    
    private IEnumerator RunAllRounds()
    {
        while (currentRound < roundDatas.Count)
        {
            var data = roundDatas[currentRound];
            yield return StartCoroutine(RunRound(data));
            currentRound++;
            yield return new WaitForSeconds(2f);
        }
    }

    private IEnumerator RunRound(RoundData data)
    {
        // 1) 스폰 큐 구성
        var queue = new List<RoundData.MonsterCount>();
        queue.AddRange(data.monsters);

        // 2) 실제 스폰
        foreach (var mc in queue)
        {
            for (int i = 0; i < mc.count; i++)
            {
                spawnManager.Spawn(mc.type);
                yield return new WaitForSeconds(spawnInterval);
            }
        }

        // 3) 몬스터 전멸 대기
        yield return new WaitUntil(
            () => GameObject.FindGameObjectsWithTag("Monster").Length == 0
        );
    }
}

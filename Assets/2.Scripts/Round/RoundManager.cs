using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class RoundManager : MonoBehaviour
{
    private static RoundManager _instance;
    
    [Header("라운드별 데이터")]
    public List<RoundData> roundDatas;

    [Header("스폰 관리자 참조")] 
    public SpawnManager spawnManager;

    // 몬스터 스폰 간격 (초) 
    private float _spawnInterval = 1f;

    private Coroutine spawnRoutine;
    public int currentRound = 0;
    
    private int totalMonsterCount = 0;
    private int deadMonsterCount = 0;
    
    private bool _CanSpawnThisScene = true;  
    
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        bool isMain = scene.name == "Main";
        _CanSpawnThisScene = !isMain;

        // 혹시 이전 라운드 코루틴이 돌고 있으면 중지
        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
            spawnRoutine = null;
        }

        // 새 씬으로 넘어갈 때는 항상 몬스터를 풀로 되돌림
        if (spawnManager != null)
        {
            spawnManager.ReturnAllToPool();
            spawnManager.ResetDeadMonsterCount();
        }
    }
    
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
        // 메인 씬에서는 무시
        if (!_CanSpawnThisScene)
        {
            Debug.Log("[RoundManager] 이 씬에서는 라운드를 시작하지 않습니다.");
            return;
        }
        
        spawnManager.ReturnAllToPool();
        MonsterSFXManager.Instance.StopAllSounds();

        if (spawnRoutine != null)
            StopCoroutine(spawnRoutine);

        currentRound = startIndex;
        deadMonsterCount = 0;

        // 라운드 데이터에서 총합 계산
        // 해당 currentRound 에 대한 RoundData 에셋에 있는 몬스터들의 count 수만 모두 더해서 
        // totalMonsterCount 에 할당
        totalMonsterCount = roundDatas[currentRound].monsters.Sum(info => info.count);

        // 라운드에 맞는 몬스터들을 스폰
        spawnRoutine = StartCoroutine(RunRounds());
    }

    public void ReportMonsterDead()
    {
        deadMonsterCount++;

        if (deadMonsterCount >= totalMonsterCount)
        {
            RoundClear();
        }
    }
    
    private void RoundClear()
    {
        switch (currentRound)
        {
            case 1: QuestManager.Instance.isPlayerHunt1F = true; break;
            case 2: QuestManager.Instance.isPlayerHunt2F = true; break;
            case 3: QuestManager.Instance.isPlayerHunt3F = true; break;
            case 4: QuestManager.Instance.isPlayerHunt4F = true; break;
            case 5: QuestManager.Instance.isPlayerHunt5F = true; break;
            case 6: QuestManager.Instance.isPlayerHunt6F = true; break;
            case 7: QuestManager.Instance.isPlayerHuntHallway = true; break;
        }

        Debug.Log($"Round {currentRound} 클리어!");
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
        HUDManager.Instance.SetStageUI(currentRound);

        // 2) 타입별 수량만큼 MonsterType 리스트에 추가
        // Enumerable.Repeat(info.type, info.count) : info.type 값을 info.count번 반복한 시퀀스를 생성
        // 예: {Zombie, 3} → {Zombie, Zombie, Zombie}
        // SelectMany() : 각 원소를 여러 개(시퀀스)로 바꾸고, 그것들을 전부 평탄화 해서 하나의 시퀀스로 이어붙이는 연산
        // 예 : [{Zombie,2}, {Ripper,1}, {Beast,3}]
        // Repeat 결과: [Zombie,Zombie], [Ripper], [Beast,Beast,Beast]
        // SelectMany로 평탄화: [Zombie,Zombie,Ripper,Beast,Beast,Beast]
        // 그 다음 리스트 조작 등을 하기 위해 ToList()로 확정
        List<MonsterType> monsterQueue = data.monsters
            .SelectMany(info => Enumerable.Repeat(info.type, info.count))
            .ToList();

        // 3) 튜플 분해로 monsterQueue 리스트 안에 있는 요소들 스왑하기
        for (int i = monsterQueue.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            
            (monsterQueue[i], monsterQueue[j]) = (monsterQueue[j], monsterQueue[i]);
        }

        // 4) 딜레이 후, 섞인 순서대로 0 자리부터 마지막 자리까지 스폰
        yield return new WaitForSeconds(_spawnInterval);
        foreach (var type in monsterQueue)
        {
            spawnManager.Spawn(type, currentRound);
            yield return new WaitForSeconds(_spawnInterval);
        }
    }
}

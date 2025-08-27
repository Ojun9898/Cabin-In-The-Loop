using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuestManager : Singleton<QuestManager>
{
    private HashSet<string> currentQuests = new HashSet<string>();
    private HashSet<string> allQuests = new HashSet<string>();
    private HashSet<string> endQuests = new HashSet<string>();

    public event Action<string> OnQuestGotten;
    public event Action<string> OnQuestCleared;

    [Header("< 퀘스트 트리거 >")] public bool isSceneCavin = false;
    public bool isSceneLab = false;

    public bool isPlayerInCavin = false;
    public bool isPlayerFindBook = false;
    public bool isPlayerOutCavin = false;
    public bool isPlayerFindManhole = false;
    public bool isPlayerHunt1F = false;
    public bool isPlayerHunt2F = false;
    public bool isPlayerHunt3F = false;
    public bool isPlayerHunt4F = false;
    public bool isPlayerHunt5F = false;
    public bool isPlayerHunt6F = false;
    public bool isPlayerHuntHallway = false;
    public bool isPlayerInWhat = false;

    private Door door;
    private ManholeController manholeController;

    private string playerInFloor = null;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        isSceneCavin = scene.buildIndex == 2;
        isSceneLab = scene.buildIndex == 1;

        door = FindObjectOfType<Door>();
        manholeController = FindObjectOfType<ManholeController>();

        StartCoroutine(QuestLoop());
    }

    void Update()
    {
        if (door != null)
        {
            isPlayerInCavin = door.GetIsPlayerInCavin();
            isPlayerOutCavin = door.GetIsPlayerOutCavin();
        }

        if (manholeController != null)
            isPlayerFindManhole = manholeController.GetIsPlayerInManhole();
    }

    #region 퀘스트 관리

    private void AddQuest(string questCode)
    {
        if (!allQuests.Contains(questCode))
        {
            allQuests.Add(questCode);
            currentQuests.Add(questCode);
            OnQuestGotten?.Invoke(questCode);
        }
    }

    private void ClearQuest(string questCode)
    {
        if (currentQuests.Contains(questCode))
        {
            currentQuests.Remove(questCode);
            endQuests.Add(questCode);
            OnQuestCleared?.Invoke(questCode);
        }
    }

    public bool GetIsQuestActive(string questCode) { return currentQuests.Contains(questCode); } 
    public bool GetIsEndQuest(string questCode) { return endQuests.Contains(questCode); }
    
    #endregion

    #region 퀘스트 진행

    private IEnumerator QuestLoop()
    {
        // 시작 퀘스트
        yield return PlayQuest("M1");

        while (true)
        {
            // Cavin 씬
            if (isSceneCavin)
            {
                if (currentQuests.Contains("M1") && isPlayerInCavin)
                {
                    ClearQuest("M1");
                    yield return PlayQuest("F1");
                }

                if (currentQuests.Contains("F1") && isPlayerFindBook)
                {
                    ClearQuest("F1");
                    yield return PlayQuest("M2");
                }

                if (currentQuests.Contains("M2") && isPlayerOutCavin)
                {
                    ClearQuest("M2");
                    yield return PlayQuest("F2");
                }

                if (currentQuests.Contains("F2") && isPlayerFindManhole)
                {
                    ClearQuest("F2");
                }
            }

            // Lab 씬
            if (isSceneLab)
            {
                if (currentQuests.Contains("H1") && playerInFloor == "1" && isPlayerHunt1F)
                {
                    ClearQuest("H1");
                    yield return PlayQuest("H2");
                }

                if (currentQuests.Contains("H2") && playerInFloor == "2" && isPlayerHunt2F)
                {
                    ClearQuest("H2");
                    yield return PlayQuest("H3");
                }

                if (currentQuests.Contains("H3") && playerInFloor == "3" && isPlayerHunt3F)
                {
                    ClearQuest("H3");
                    yield return PlayQuest("H4");
                }

                if (currentQuests.Contains("H4") && playerInFloor == "4" && isPlayerHunt4F)
                {
                    ClearQuest("H4");
                    yield return PlayQuest("H5");
                }

                if (currentQuests.Contains("H5") && playerInFloor == "5" && isPlayerHunt5F)
                {
                    ClearQuest("H5");
                    yield return PlayQuest("H6");
                }

                if (currentQuests.Contains("H6") && playerInFloor == "6" && isPlayerHunt6F)
                {
                    ClearQuest("H6");
                    yield return PlayQuest("H7");
                }

                if (currentQuests.Contains("H7") && playerInFloor == "7" && isPlayerHuntHallway)
                {
                    ClearQuest("H7");
                    yield return PlayQuest("M3");
                }

                if (currentQuests.Contains("M3") && isPlayerHuntHallway && isPlayerInWhat)
                {
                    ClearQuest("M3");
                    yield return PlayQuest("END");
                    EndingManager.Instance.ShowAliveEnding();
                }
            }

            yield return null; // 매 프레임 체크
        }
    }

    private IEnumerator PlayQuest(string questCode)
    {
        if (currentQuests.Contains(questCode)) yield break;

        AddQuest(questCode);
        Debug.Log($"{questCode} 시작");

        switch (questCode)
        {
            case "M1":
                MessageManager.Instance.Message("이쯤인데...");
                MessageManager.Instance.Message("여긴가.");
                MessageManager.Instance.Message("윽, 폐가라고 해도 믿겠네.");
                MessageManager.Instance.Message("...일단 들어가보자.");
                break;
            case "F1":
                MessageManager.Instance.Message("콜록! 콜록!");
                MessageManager.Instance.Message("...");
                MessageManager.Instance.Message("이 정도 먼지라면 금방 폐가 굳어도 이상하지 않겠어.");
                MessageManager.Instance.Message("음...? 테이블 위에 저건...");
                break;
            case "M2":
                MessageManager.Instance.Message("말도 안되는 소리겠지만... 왠지 불길한 걸.");
                MessageManager.Instance.Message("밖으로 나가보자.");
                break;
            case "F2":
                MessageManager.Instance.Message("당장 맨홀을 찾아!!!!");
                break;
            case "H1":
                MessageManager.Instance.Message("여긴... 어디지?");
                MessageManager.Instance.Message("미친 몬스터를 피해 맨홀로 뛰어든 것만 기억 나.");
                MessageManager.Instance.Message("정신을 차린 것 같군.");
                MessageManager.Instance.MessageOther("놀랐지? 내 깜짝 선물을 보고 말이야.");
                MessageManager.Instance.MessageOther("이제부터 진짜 시작이니까 정신차려.");
                MessageManager.Instance.MessageOther("네가 모든 것을 파괴해야만 해.");
                MessageManager.Instance.MessageOther("마지막 장소에서 '그'가 기다리고 있으니...");
                MessageManager.Instance.Message("...뭐라는지 하나도 모르겠어.");
                MessageManager.Instance.Message("일단, '그'를 만나보자.");
                MessageManager.Instance.Message("거기까지...죽기살기로 해보는 수밖에.");
                break;
            case "H2":
                MessageManager.Instance.Message("대체 언제까지 반복해야하는거야...");
                break;
            case "H3":
                MessageManager.Instance.Message("처음 보는 몬스터다.");
                MessageManager.Instance.Message("저건... 거미?");
                break;
            case "H4":
                MessageManager.Instance.Message("저 낫에 스쳤다간 죽을지도...");
                break;
            case "H5":
                MessageManager.Instance.Message("하하... 이젠 걸어다니는 동물인가...");
                break;
            case "H6":
                MessageManager.Instance.Message("어마어마한 녀석이다...");
                break;
            case "H7":
                MessageManager.Instance.Message("더 이상 올라갈 곳은 없다는 건가.");
                MessageManager.Instance.Message("드디어 끝이 보인다...");
                break;
            case "M3":
                MessageManager.Instance.Message("여긴... 제단?");
                break;
            case "END":
                MessageManager.Instance.Message("...뭐지...?");
                MessageManager.Instance.Message("몸이... 움직이질 않아...");
                MessageManager.Instance.MessageOther("여기까지 오느라 고생이 많았어.");
                MessageManager.Instance.MessageOther("많이 괴롭고 힘들었니?");
                MessageManager.Instance.MessageOther("그랬으면 좋겠다...");
                MessageManager.Instance.MessageOther("그래야 네가 다음 회차에서 나를 더 즐겁게 해줄테니까.");
                MessageManager.Instance.MessageOther("금방 다시 시작될거야.");
                MessageManager.Instance.MessageOther("잘 자.");
                MessageManager.Instance.Message("...");
                MessageManager.Instance.Message("...");
                MessageManager.Instance.Message("...");
                MessageManager.Instance.Message("...");
                break;
        }

        // 모든 메시지 종료까지 대기
        yield return new WaitUntil(() => MessageManager.Instance.IsDone);
    }

    #endregion
}
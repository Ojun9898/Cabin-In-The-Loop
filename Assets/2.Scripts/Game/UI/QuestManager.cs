using System;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : Singleton<QuestManager>
{
    private HashSet<string> currentQuests = new HashSet<string>();
    private HashSet<string> allQuests = new HashSet<string>();

    public event Action<string> OnQuestGotten;
    public event Action<string> OnQuestCleared;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }
    
    // 이미 가지고 있는 퀘스트인지 체크해서 중복 추가 방지
    private void AddQuest(string questCode)
    {
        if (allQuests.Contains(questCode))
            return;

        allQuests.Add(questCode);
        currentQuests.Add(questCode);
        OnQuestGotten?.Invoke(questCode);
    }

    public void ClearQuest(string questCode)
    {
        if (!currentQuests.Contains(questCode))
            return;

        currentQuests.Remove(questCode);
        OnQuestCleared?.Invoke(questCode);
    }

    public bool IsQuestActive(string questCode)
    {
        return currentQuests.Contains(questCode);
    }
}
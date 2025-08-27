using UnityEngine;

public class Book : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OnBookTouched();
        }
    }

    private void OnBookTouched()
    {
        // F1 퀘스트가 진행 중일 때만
        if (QuestManager.Instance.GetIsQuestActive("F1"))
        {
            MessageManager.Instance.Message("수상한 책이다.");
            MessageManager.Instance.Message("'몬스터를 피해 맨홀을 찾아라...'");
            MessageManager.Instance.Message("무슨 소리지...?");

            // 퀘스트 상태 활성화
            QuestManager.Instance.isPlayerFindBook = true;

            // PlayQuest("M2") 호출은 QuestManager가 자동으로 처리
        }
    }
}
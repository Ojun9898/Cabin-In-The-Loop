using UnityEngine;

public class CavinExit : MonoBehaviour
{
    private bool isEndQuest = false;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && QuestManager.Instance.GetIsEndQuest("F1")) // 이전 퀘스트가 완료되었다면
        {
            Door door = FindObjectOfType<Door>();
            if (door != null)
                door.isPlayerOutCavin = true; // 나갔다고 판정
        }
    }
}
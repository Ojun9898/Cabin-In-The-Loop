using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class RoundStartTrigger : MonoBehaviour
{
    [SerializeField] private int roundIndex = 0;
    public RoundManager roundManager;

    [SerializeField] private float delayBeforeStart = 2f;

    private bool triggered;
    private Collider _col;

    private void Awake()
    {
        _col = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        if (other.CompareTag("Player"))
        {
            // 플레이어가 콜라이더를 지나쳤다면 riggered 를 true로 변경하고 콜라이더를 비활성화한다음
            triggered = true;
            _col.enabled = false;
            
            // 약간의 딜레이후 스폰시작
            StartCoroutine(SpawnStart());
        }
    }

    private IEnumerator SpawnStart()
    {
        yield return new WaitForSeconds(delayBeforeStart);
        
        QuestManager.Instance.playerInFloor = roundIndex;

        // 메시지 완료 후 라운드 시작
        MessageManager msgMgr = MessageManager.Instance;
        if (msgMgr != null && !msgMgr.IsDone)
        {
            bool started = false;
            msgMgr.OnMessagesEnded += () =>
            {
                if (!started)
                {
                    started = true;
                    roundManager.StartRounds(roundIndex);
                }
            };
        }
        else
        {
            roundManager.StartRounds(roundIndex);
        }
    }
}
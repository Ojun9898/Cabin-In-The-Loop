using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class RoundStartTrigger : MonoBehaviour
{
    [SerializeField] private int roundIndex = 0;
    public RoundManager roundManager;

    [SerializeField] private float delayBeforeStart = 0.5f;

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
            triggered = true;
            _col.enabled = false;

            StartCoroutine(DelayedStart());
        }
    }

    private IEnumerator DelayedStart()
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
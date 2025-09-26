using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class RoundStartTrigger : MonoBehaviour
{
    [SerializeField] private int roundIndex = 0;
    public RoundManager roundManager;
    
    // 트리거 밟고 0.5초 대기
    // 그 이후에 RoundManager에서 3,5초마다 스폰
    [SerializeField] private float delayBeforeStart = 0.5f; 
    
    private bool triggered;
    private Collider _col;

    private void Awake()
    {
        // 콜라이더 캐싱
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
        roundManager.StartRounds(roundIndex);
    }
    
}

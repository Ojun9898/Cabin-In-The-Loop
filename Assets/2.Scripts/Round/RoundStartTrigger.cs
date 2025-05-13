using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class RoundStartTrigger : MonoBehaviour
{
    [Tooltip("라운드를 시작할 RoundManager")]
    public RoundManager roundManager;

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        // 플레이어 태그로만 반응하고, 한 번만 실행
        if (triggered) return;
        if (other.CompareTag("Player"))
        {
            if (roundManager != null)
                roundManager.StartRounds();
            else
                Debug.LogWarning("RoundManager가 할당되지 않았습니다.");

            triggered = true;
            // 트리거 비활성화 
            GetComponent<Collider>().enabled = false;
        }
    }
}

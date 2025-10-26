using UnityEngine;
using UnityEngine.AI;

public class MonsterMovement
{
    private NavMeshAgent navMeshAgent;
    private float moveSpeed;

    public MonsterMovement(NavMeshAgent navMeshAgent, float moveSpeed)
    {
        this.navMeshAgent = navMeshAgent;
        this.moveSpeed = moveSpeed;
        navMeshAgent.speed = moveSpeed;
        
        // autoTraverseOffMeshLink : 링크(사다리/낭떠러지 등) 구간을 에이전트가 자동으로 건너가게 할지를 결정하는 스위치
        // True 이면 평범하게 이동, false면 링크(사다리/낭떠러지 등) 에 맞는 스크립트 처리와 애니메이션을 설정해야 함
        navMeshAgent.autoTraverseOffMeshLink = true;
    }

    public void MoveToTarget(Vector3 targetPosition)
    {
        if (navMeshAgent != null)
        {
            navMeshAgent.isStopped = false; // 이동 활성화
            // NavMeshAgent에게 다음 목표 지점을 호출
            navMeshAgent.SetDestination(targetPosition);
        }
    }

    public void Stop()
    {
        if (navMeshAgent != null && navMeshAgent.enabled && navMeshAgent.isOnNavMesh)
            navMeshAgent.isStopped = true;
    }
    
    public void Resume() => navMeshAgent.isStopped = false;
    
    public void SetSpeed(float speed)
    {
        moveSpeed = speed;
        navMeshAgent.speed = speed;
    }
}

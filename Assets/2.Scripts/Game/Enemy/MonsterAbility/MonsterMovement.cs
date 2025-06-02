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
        
        
        navMeshAgent.autoTraverseOffMeshLink = true;
    }

    public void MoveToTarget(Vector3 targetPosition)
    {
        if (navMeshAgent != null)
        {
            navMeshAgent.isStopped = false; // 이동 활성화
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

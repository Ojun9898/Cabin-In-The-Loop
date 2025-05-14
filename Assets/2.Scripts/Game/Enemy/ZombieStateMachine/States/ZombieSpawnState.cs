using UnityEngine;
using UnityEngine.AI;

public class ZombieSpawnState : ZombieBaseState
{
    private const float SPAWN_DURATION = 1.5f;
    private NavMeshAgent navMeshAgent;
    
    protected override void SetStateKey()
    {
        stateKey = EState.Spawn;
    }
    
    public override void Initialize(Monster owner)
    {
        base.Initialize(owner);
        // NavMeshAgent 컴포넌트 가져오기
        navMeshAgent = owner.gameObject.GetComponent<NavMeshAgent>();
    }
    
    public override void EnterState()
    {
        base.EnterState();
        // ① 에이전트 위치 강제 동기화 (Warp)
        if (navMeshAgent != null)
        {
            navMeshAgent.Warp(zombie.transform.position);
            // ② 남아있는 경로 정보 초기화
            navMeshAgent.ResetPath();
            // ③ 이동 잠금 해제 준비 (필요 시)
            navMeshAgent.isStopped = true;
        }
        StopMoving();
        // (선택) 스폰 중엔 애니메이션 재생 등
        PlayAnimation("Spawn");
    }
    
    public override void UpdateState()
    {
        base.UpdateState();
    }
    
    
    public override bool IsStateEnd(out EState nextState)
    {
        nextState = EState.Spawn;
        
        if (stateTimer >= SPAWN_DURATION)
        {
            if (navMeshAgent != null)
                navMeshAgent.isStopped = false;
            nextState = EState.Idle;
            return true;
        }
        
        return false;
    }
} 
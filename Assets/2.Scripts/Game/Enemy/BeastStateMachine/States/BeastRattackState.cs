using UnityEngine;

public class BeastRattackState : BeastBaseState
{
    private const float HOWL_DURATION = 2f;
    private const float PARTICLE_START_TIME = 0.5f; // 애니메이션 시작 후 파티클이 나오는 시간
    private const float HOWL_COOLDOWN = 20f; // 하울링 쿨다운 시간
    private float lastHowlTime = 0f; // 마지막 하울링 시간 (static으로 모든 인스턴스가 공유)
    private bool hasStartedParticle = false;

    protected override void SetStateKey()
    {
        stateKey = EState.Rattack;
    }

    public override void EnterState()
    {
        base.EnterState();
        StopMoving();
        PlayAnimation("Shout");
        hasStartedParticle = false;
        
        // 하울링 시작 시간 기록
        lastHowlTime = Time.time;
    }

    public override void UpdateState()
    {
        base.UpdateState();

        // 애니메이션 중간에 파티클 시작
        if (!hasStartedParticle && stateTimer >= PARTICLE_START_TIME)
        {
            StartHowling();
            hasStartedParticle = true;
        }
    }

    public override void ExitState()
    {
        base.ExitState();
        EndHowling();
    }

    public override bool IsStateEnd(out EState nextState)
    {
        nextState = EState.Rattack;

        if (stateTimer >= HOWL_DURATION)
        {
            // 다음 상태 결정
            if (IsPlayerInAttackRange())
            {
                nextState = EState.Attack;
            }
            else if (IsPlayerInChaseRange())
            {
                nextState = EState.Chase;
            }
            else
            {
                nextState = EState.Wander;
            }
            return true;
        }

        return false;
    }
    
    // 하울링 사용 가능 여부 확인
    public bool CanUseHowl()
    {
        return Time.time - lastHowlTime >= HOWL_COOLDOWN;
    }
}
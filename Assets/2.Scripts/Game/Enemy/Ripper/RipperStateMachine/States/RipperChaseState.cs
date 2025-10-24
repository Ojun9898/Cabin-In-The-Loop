using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RipperChaseState : RipperBaseState
{
    private const float CHASE_RANGE = 48f;
    private const float ATTACK_RANGE = 1.5f;
    
    // 추격시에는 이동속도가 더 빨라짐
    private float runSpeed = 3f;
    public float BaseRunSpeed => runSpeed; 
    
    
    protected override void SetStateKey() => stateKey = EState.Chase;
    
    public override void Initialize(Monster owner)
    {
        base.Initialize(owner);
    }
    
    public override void ExitState()
    {
        base.ExitState();
        // Chase 상태 벗어나면 즉시 해당 몬스터 모든 SFX 중단
        MonsterSFXManager.Instance.StopAllAudio( ripper.transform.GetInstanceID());
    }
           

    public override void EnterState()
    {
        base.EnterState();
        // Run 상태 진입 시 속도 변경
        if (navMeshAgent != null) 
            navMeshAgent.speed = runSpeed;
            
        PlayAnimation("Ripper Run");
        // Chase 사운드 재생 요청
        MonsterSFXManager.Instance.RequestPlay(
            EState.Chase,
            EMonsterType.Ripper,
            ripper.transform
        );
        
    }
    
    public override void UpdateState()
    {
        base.UpdateState();
        // Chase 사운드 반복 재생
        MonsterSFXManager.Instance.RequestPlay(
            EState.Chase,
            EMonsterType.Ripper,
            ripper.transform
        );
        
        bool inRange = IsPlayerInRange(CHASE_RANGE);
        // 범위 안일 때만 직접 회전, 아닐 때는 NavMesh 회전
        if (navMeshAgent != null)
            navMeshAgent.updateRotation = !inRange;
        if (inRange)
            RotateTowards(playerTransform.position, 6f);
        
        MoveToPlayer();
        
        // 상태 전환 처리
        EState nextState = CheckStateTransitions();
        if (nextState != stateKey)
        {
            stateTimer = float.MaxValue;
        }
    }
    public override bool IsStateEnd(out EState nextState)
    {
        nextState = CheckStateTransitions();
        return nextState != stateKey;
    }

    private EState CheckStateTransitions()
    {
        if (!IsPlayerInRange(CHASE_RANGE))
        {
            return EState.Wander;
        }

        if (IsPlayerInRange(ATTACK_RANGE))
        {
            return EState.Attack;
        }

        return EState.Chase;
    }
}

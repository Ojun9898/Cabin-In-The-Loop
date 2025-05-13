using UnityEngine;

public class BeastAttackState : BeastBaseState
{
    private GameObject poisonThronPrefab; // Poison Thron 프리팹
    private Transform firePoint;          // 발사 위치
    private float thronSpeed = 10f;       // 발사 속도

    private const float ATTACK_DURATION = 1.5f;
    private const float ATTACK_COOLDOWN = 2f; // 공격 쿨다운 시간
    private float lastAttackTime = 0f;       // 마지막 공격 시간
    private float ATTACK_RANGE => beast.attackRange;  // 공격 사거리
    private float CHASE_RANGE => beast.chaseRange;    // 추적 사거리
    private const float DAMAGE_AMOUNT = 20f;          // 공격 데미지
    private bool hasAttacked = false;
    
    private const int MAX_ATTACK_ANIMATIONS = 4; // Attack1~4까지 있음

    // 상태 키 설정
    protected override void SetStateKey()
    {
        stateKey = EState.Attack;
    }

    // 공격 상태에 쓰일 필드 초기화
    public void InitializeAttack(GameObject prefab, Transform point, float speed)
    {
        poisonThronPrefab = prefab;
        firePoint = point;
        thronSpeed = speed;
    }

    public override void EnterState()
    {
        base.EnterState();
        StopMoving();
        
        // 랜덤한 공격 애니메이션 선택
        int randomAttackIndex = Random.Range(1, MAX_ATTACK_ANIMATIONS + 1);
        PlayAnimation($"Attack{randomAttackIndex}");
    }
    
    public override void UpdateState()
    {
        base.UpdateState();

        // 타겟을 향해 회전
        if (beast.player != null)
        {
            Vector3 direction = (beast.player.transform.position - beast.transform.position).normalized;
            direction.y = 0; // Y축 회전은 무시
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                beast.transform.rotation = Quaternion.Slerp(beast.transform.rotation, targetRotation, Time.deltaTime * 10f);
            }
        }

        // 공격 도중에는 상태를 유지
        if (stateTimer < ATTACK_DURATION)
        {
            return;
        }
        
        // 공격 종료 후 상태 전환 처리
        HandlePostAttack();
    }
    
    private void HandlePostAttack()
    {
        EState nextState;
        HandlePostAttack(out nextState);
        if (nextState != stateKey)
        {
            stateTimer = float.MaxValue; // 상태 강제 종료
        }
    }
    
    private void HandlePostAttack(out EState nextState)
    {
        nextState = EState.Idle;
        
        if (IsPlayerInAttackRange())
        {
            // 공격 쿨다운이 지났는지 확인
            if (Time.time - lastAttackTime >= ATTACK_COOLDOWN)
            {
                nextState = EState.Attack;
                lastAttackTime = Time.time;
            }
            else
            {
                nextState = EState.Idle; // 쿨다운 중에는 Idle 상태로 전환
            }
        }
        else if (IsPlayerInChaseRange())
        {
            nextState = EState.Chase;
        }
    }
    
    public override bool IsStateEnd(out EState nextState)
    {
        nextState = EState.Attack;
        
        if (stateTimer >= ATTACK_DURATION)
        {
            HandlePostAttack(out nextState);
            return true;
        }
        
        return false;
    }

    // 플레이어의 위치 가져오기
    private Vector3 GetPlayerPosition()
    {
        if (beast.player == null) return beast.transform.position;
        return beast.player.transform.position;
    }
}
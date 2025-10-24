using UnityEngine;

public class BeastAttackState : BeastBaseState
{
    private const float ATTACK_DURATION = 1.5f;
    private const float ATTACK_COOLDOWN = 2f; // 공격 쿨다운 시간
    private float lastAttackTime = 0f;       // 마지막 공격 시간
    private float ATTACK_RANGE => beast.attackRange;  // 공격 사거리
    private const float DAMAGE_FIELD_RADIUS = 2f;
    private float CHASE_RANGE => beast.chaseRange;    // 추적 사거리
    private const float DAMAGE_AMOUNT = 20f;          // 공격 데미지
    private BeastHowlEffect _howl;                 // 캐시

    private bool hasAttacked = false;
    
    private const int MAX_ATTACK_ANIMATIONS = 4; // Attack1~4까지 있음

    // 상태 키 설정
    protected override void SetStateKey()
    {
        stateKey = EState.Attack;
    }

    public override void EnterState()
    {
        base.EnterState();
        if (_howl == null) _howl = beast.GetComponent<BeastHowlEffect>(); // 1회 캐시
        
        StopMoving();
        hasAttacked = false; // 공격 플래그 초기화
        
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
            // Y축 회전은 무시(수평면만 사용)
            direction.y = 0; 
            // 타겟과 같은 위치(0, 0, 0)가 아닐떄만 회전
            if (direction != Vector3.zero)
            {
                // 타깃을 향한 회전값 생성
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                // 천천히 회전하게 보간을 함
                beast.transform.rotation = Quaternion.Slerp(beast.transform.rotation, targetRotation, Time.deltaTime * 10f);
            }
        }

        // 공격 도중에는 상태를 유지
        if (stateTimer < ATTACK_DURATION)
        {
            // 공격이 중간에 처리되지 않았다면 데미지 필드 생성 (모션 중간 시점)
            if (!hasAttacked && stateTimer >= ATTACK_DURATION * 0.5f)
            {
                CreateDamageField();
                hasAttacked = true;
            }
            return;
        }
        
        // 공격 종료 후 상태 전환 처리
        HandlePostAttack();
    }
    
    private void CreateDamageField()
    {
        GameObject damageField = GameManager.Instance.GetDamageField();
        if (damageField != null)
        {
            float amount = DAMAGE_AMOUNT * (stateMachine ? stateMachine.CurrentDamageMultiplier : 1f);
            
            damageField.transform.position = beast.transform.position;
            damageField.GetComponent<DamageField>().Initialize(
                beast.gameObject,
                amount,
                DAMAGE_FIELD_RADIUS,
                0.1f // 데미지 필드 지속 시간
            );
        }
    }
    
    private void HandlePostAttack()
    {
        // float.MaxValue : float이 표현할 수 있는 가장 큰 수
        // ** 강제 종료 플래그만 세팅 ** 
        stateTimer = float.MaxValue;
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
using UnityEngine;

public abstract class BeastBaseState : State<Monster>
{
    protected Monster beast;
    protected BeastStateMachine stateMachine;
    protected float stateTimer;
    
    // 하울링 관련 필드
    protected const float HOWL_COOLDOWN = 15f;
    protected float howlTimer = 0f;
    protected bool isHowling = false;
    
    public override void Initialize(Monster owner)
    {
        beast = owner;
        stateMachine = beast.GetComponent<BeastStateMachine>();
        stateTimer = 0f;
        howlTimer = 0f;
        isHowling = false;
        SetStateKey();
    }
    
    // 하위 클래스에서 구현하여 stateKey를 설정
    protected abstract void SetStateKey();
    
    public override void EnterState()
    {
        stateTimer = 0f;
    }
    
    public override void ExitState()
    {
        // 기본 구현은 비어있음
    }
    
    public override void UpdateState()
    {
        stateTimer += Time.deltaTime;
        
        // 하울링 중이 아닐 때만 쿨다운 타이머 증가
        if (!isHowling)
        {
            howlTimer += Time.deltaTime;
        }
    }
    
    public override void FixedUpdateState()
    {
        // 물리 기반 업데이트가 필요한 경우 하위 클래스에서 구현
    }
    
    protected bool IsPlayerInChaseRange()
    {
        return beast.IsPlayerInRange(beast.chaseRange);
    }

    protected bool IsPlayerInAttackRange()
    {
        return beast.IsPlayerInRange(beast.attackRange);
    }
    
    protected bool IsPlayerInHowlRange()
    {
        return beast.IsPlayerInRange(stateMachine.GetHowlRange());
    }
    
    protected void StartHowling()
    {
        stateMachine.StartHowling();
    }
    
    protected void EndHowling()
    {
        stateMachine.EndHowling();
    }
    
    protected void MoveToPlayer()
    {
        beast.MoveToPlayer();
    }
    
    protected void StopMoving()
    {
        beast.StopMoving();
    }
    
    protected void PlayAnimation(string animationName)
    {
        beast.PlayAnimation(animationName);
    }
    
    protected void TakeDamage(int damage)
    {
        beast.TakeDamage(damage);
    }
} 
using UnityEngine;

public abstract class InsectBaseState : State<Monster>
{
    protected Monster insect;
    protected float stateTimer;
    
    public override void Initialize(Monster owner)
    {
        insect = owner;
        stateTimer = 0f;
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
    }
    
    public override void FixedUpdateState()
    {
        // 물리 기반 업데이트가 필요한 경우 하위 클래스에서 구현
    }
    
    protected bool IsPlayerInChaseRange()
    {
        return insect.IsPlayerInRange(insect.chaseRange);
    }

    protected bool IsPlayerInAttackRange()
    {
        return insect.IsPlayerInRange(insect.attackRange);
    }
    
    protected void MoveToPlayer()
    {
        insect.MoveToPlayer();
    }
    
    protected void StopMoving()
    {
        insect.StopMoving();
    }
    
    protected void PlayAnimation(string animationName)
    {
        insect.PlayAnimation(animationName);
    }
    
    protected void TakeDamage(int damage)
    {
        insect.TakeDamage(damage);
    }
} 
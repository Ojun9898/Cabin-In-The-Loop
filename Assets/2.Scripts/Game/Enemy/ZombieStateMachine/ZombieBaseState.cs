using UnityEngine;

public abstract class ZombieBaseState : State<Monster>
{
    protected Monster zombie;
    protected float stateTimer;
    
    public override void Initialize(Monster owner)
    {
        zombie = owner;
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
    
    protected bool IsPlayerInRange(float range)
    {
        return zombie.IsPlayerInRange(range);
    }
    
    protected void MoveToPlayer()
    {
        zombie.MoveToPlayer();
    }
    
    protected void StopMoving()
    {
        zombie.StopMoving();
    }
    
    protected void PlayAnimation(string animationName)
    {
        zombie.PlayAnimation(animationName);
    }
    
    protected void TakeDamage(int damage)
    {
        zombie.TakeDamage(damage);
    }
} 
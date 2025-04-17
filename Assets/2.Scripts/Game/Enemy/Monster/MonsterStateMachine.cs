using UnityEngine;

public abstract class MonsterStateMachine : StateMachine<Monster>
{
    protected override Monster GetEntity()
    {
        return GetComponent<Monster>();
    }
    
    protected override void Initialize()
    {
        // 하위 클래스에서 상태 초기화
    }
    
    public void OnHit(int damage)
    {
        Monster monster = GetEntity();
        monster.OnHit(damage);
        
        if (!monster.IsDead())
        {
            ChangeState(CreateHitState());
        }
        else
        {
            ChangeState(CreateDeadState());
        }
    }
    
    protected abstract IState<Monster> CreateHitState();
    protected abstract IState<Monster> CreateDeadState();
} 
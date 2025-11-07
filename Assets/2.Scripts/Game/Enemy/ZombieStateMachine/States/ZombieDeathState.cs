using UnityEngine;

public class ZombieDeathState : ZombieBaseState
{
    private const float DEATH_DURATION = 2f;
    private bool hasStartedDeathAnimation = false;
    
    protected override void SetStateKey()
    {
        stateKey = EState.Death;
    }
    
    public override void EnterState()
    {
        base.EnterState();
        StopMoving();
        PlayAnimation("Death");
        hasStartedDeathAnimation = false;
    }
    
    public override void UpdateState()
    {
        base.UpdateState();
        
        // 사망 애니메이션이 끝나면 오브젝트를 비활성화
        if (!hasStartedDeathAnimation && stateTimer >= DEATH_DURATION)
        {
            hasStartedDeathAnimation = true;
            
            // 비활성화 이전에 아이템 드랍 이벤트 발생
            SpawnLootIfAny();
            
            zombie.gameObject.SetActive(false);
        }
    }
    
    public override bool IsStateEnd(out EState nextState)
    {
        nextState = EState.Death;
        return false; // Death 상태는 종료되지 않음
    }
    
    // ─────────────────────────────────────────────────────────
    // 각 몬스터 DeathState에 그대로 복붙해서 쓰는 유틸 메서드
    // ─────────────────────────────────────────────────────────
    private void SpawnLootIfAny()
    {
        // zombie는 보통 BaseState가 들고 있는 본체 컴포넌트(=MonoBehaviour) 참조라고 가정
        if (zombie != null && zombie.TryGetComponent(out LootSpawner spawner))
        {
            spawner.SpawnLoot(zombie.transform.position);
        }
        // spawner 경고 생략 (없으면 아무 일도 일어나지 않음)
    }
} 
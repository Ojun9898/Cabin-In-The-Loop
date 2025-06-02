using UnityEngine;

public class PlayerHitState : IPlayerState
{
    private float hitDuration = 0.5f;        // 피격 애니메이션 지속
    private float invincibleDuration = 1.0f; // 무적 지속 시간
    private float timer;

    public void Enter(PlayerStateMachine player)
    {
        timer = 0f;
        player.animator.SetTrigger("HIT");

        // 무적 상태 시작
        player.StartInvincibility(invincibleDuration);
    }

    public void Execute(PlayerStateMachine player)
    {
        timer += Time.deltaTime;
        if (timer >= hitDuration)
        {
            player.ChangeState(new PlayerIdleState());
        }
    }

    public void Exit(PlayerStateMachine player)
    {
        // 필요 시 애니메이션 트리거 초기화 가능
        // psm.animator.ResetTrigger("HIT");
    }
}
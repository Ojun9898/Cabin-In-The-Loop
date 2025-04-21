using UnityEngine;

public class PlayerHitState : IPlayerState
{
    private float _hitDuration = 0.5f;    // 피격 상태 지속 시간
    private float _timer;

    // 상태 진입 시: 히트 트리거 애니메이션 재생
    public void Enter(PlayerStateMachine psm)
    {
        _timer = 0f;
        psm.animator.SetTrigger("HIT");
        // 이동/입력 억제용 변수도 설정할 수 있음
    }

    // 매 프레임: 지정 시간 경과하면 Idle 상태로 복귀
    public void Execute(PlayerStateMachine psm)
    {
        _timer += Time.deltaTime;
        if (_timer >= _hitDuration)
        {
            psm.ChangeState(new PlayerIdleState());
        }
    }

    // 상태 종료 시: 필요하다면 트리거 리셋
    public void Exit(PlayerStateMachine psm)
    {
        // psm.animator.ResetTrigger("Hit");
    }
}
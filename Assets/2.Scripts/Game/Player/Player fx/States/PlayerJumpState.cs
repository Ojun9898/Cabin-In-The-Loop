using UnityEngine;

public class PlayerJumpState : IPlayerState
{
    public void Enter(PlayerStateMachine player)
    {
        player.animator.CrossFade("JUMP", 0.1f, 1);
        // 이전에 남아 있던 모든 속도 리셋
        player.velocity = Vector3.zero;

        // 오직 위 방향 속도만 설정
        player.velocity.y = player.jumpSpeed;
        
        // 초기 전진 임펄스
        Vector3 inputDir = new Vector3(player.moveInput.x, 0f, player.moveInput.y).normalized;
        player.velocity += inputDir * player.moveSpeed;
    }

    public void Execute(PlayerStateMachine player)
    {
        // 공중 입력으로만 수평 속도 결정 (이전 상태 잔여 속도 없음)
        Vector3 inputDir = player.GetCameraRelativeMoveDirection().normalized;
        player.velocity.x = inputDir.x * player.moveSpeed;
        player.velocity.z = inputDir.z * player.moveSpeed;

        // 이동
        player.controller.Move(player.velocity * Time.deltaTime);

        // 점프 중 회전 처리
        if (inputDir.sqrMagnitude > 0.01f)
            player.RotateTowardsCameraDirection();
        
        // 착지 체크
        if (player.controller.isGrounded)
        {
            player.velocity = Vector3.zero;
            
            // 다음 상태 전환 로직...
            if (player.moveInput.magnitude > 0.1f)
                player.ChangeState(new PlayerMoveState());
            else if (player.runPressed)
                player.ChangeState(new PlayerRunState());
            else
                player.ChangeState(new PlayerIdleState());
        }
    }

    public void Exit(PlayerStateMachine player) { }
}
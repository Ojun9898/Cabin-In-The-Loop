using UnityEngine;

public class PlayerRunState : IPlayerState
{
    public void Enter(PlayerStateMachine player)
    {
        player.animator.CrossFade("RUN", 0.1f, 0);
        player.currentMoveSpeed = player.moveSpeed * 2;
    }

    public void Execute(PlayerStateMachine player)
    {
        if (player.jumpPressed && player.controller.isGrounded)
        {
            player.jumpPressed = false;
            player.ChangeState(new PlayerJumpState());
            return;
        }

        if (player.attackPressed)
        {
            player.attackPressed = false;
            player.ChangeState(new PlayerAttackState());
            return;
        }

        Vector3 moveDir = player.GetCameraRelativeMoveDirection();

        if (moveDir.magnitude > 0.1f)
        {
            
            player.controller.Move(moveDir.normalized * (player.currentMoveSpeed * Time.deltaTime));
            player.RotateTowardsCameraDirection();
        }
        else
        {
            player.ChangeState(new PlayerIdleState());
        }
        
        if (!player.runPressed)
            player.ChangeState(new PlayerMoveState());
    }

    public void Exit(PlayerStateMachine player)
    {
        player.currentMoveSpeed = player.moveSpeed;
    }
}
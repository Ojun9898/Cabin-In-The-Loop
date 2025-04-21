using UnityEngine;

public class PlayerMoveState : IPlayerState
{
    public void Enter(PlayerStateMachine player)
    {
        player.animator.CrossFade("WALK", 0.1f, 0);
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
            if (player.runPressed)
            {
                player.ChangeState(new PlayerRunState());
                return;
            }
            
            player.RotateTowardsCameraDirection();
            player.controller.Move(moveDir.normalized * (player.currentMoveSpeed * Time.deltaTime));
        }
        else
        {
            player.ChangeState(new PlayerIdleState());
        }
    }


    public void Exit(PlayerStateMachine player) { }
}
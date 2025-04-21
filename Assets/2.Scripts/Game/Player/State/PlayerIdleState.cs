public class PlayerIdleState : IPlayerState
{
    public void Enter(PlayerStateMachine player)
    {
        player.animator.CrossFade("IDLE", 0.1f, 0);
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

        if (player.moveInput.magnitude > 0.1f)
        {
            player.ChangeState(new PlayerMoveState());
        }
    }

    public void Exit(PlayerStateMachine player) { }
}
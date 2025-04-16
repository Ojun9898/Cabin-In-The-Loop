using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttackState : IPlayerState
{
    private float _comboTimer;
    private int _comboStep;
    private const float ComboDelay = 0.5f;

    public void Enter(PlayerStateMachine player)
    {
        _comboStep = 1;
        _comboTimer = 0f;
        PlayCombo(player);
    }

    public void Execute(PlayerStateMachine player)
    {
        _comboTimer += Time.deltaTime;
        
        if (Input.GetMouseButtonDown(0))
        {
            _comboTimer = 0f;

            if (player.attackPressed && _comboTimer <= ComboDelay && _comboStep < 4)
            {
                _comboStep++;
                _comboTimer = 0f;
                PlayCombo(player);
            }
        }
        
        
        if (_comboTimer >= ComboDelay)
        {
            player.ChangeState(player.moveInput.magnitude > 0.1f ? new PlayerMoveState() : new PlayerIdleState());
        }
    }

    private void PlayCombo(PlayerStateMachine player)
    {
        player.RotateTowardsCameraDirection();
        player.animator.CrossFade($"ATTACK{_comboStep}", 0.1f, 2);
        player.attackPressed = false;
    }

    public void Exit(PlayerStateMachine player) { }
}

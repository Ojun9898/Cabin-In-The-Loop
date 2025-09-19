using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttackState : IPlayerState
{
    private float _comboTimer;
    private int _comboStep;
    private const int MaxCombo = 4;
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

        if (player.attackPressed && _comboTimer <= ComboDelay && _comboStep < MaxCombo)
        {
            _comboStep++;
            _comboTimer = 0f;
            PlayCombo(player);
        }

        if (_comboTimer > ComboDelay)
        {
            bool moving = player.moveInput.sqrMagnitude > 0.01f;
            player.ChangeState(moving ? new PlayerMoveState() : new PlayerIdleState());
        }
    }

    public void Exit(PlayerStateMachine player)
    {
        player.attackPressed = false;
    }

    private void PlayCombo(PlayerStateMachine player)
    {
        player.RotateTowardsCameraDirection();

        var weaponCtrl = player.GetComponent<WeaponController>();
        var type = weaponCtrl.currentWeaponType;
        var data = GameManager.Instance.GetWeaponData(type);

        switch (data.category)
        {
            case WeaponCategory.Melee:
                player.animator.CrossFade($"ATTACK{_comboStep}", 0.1f, 2);
                break;
            case WeaponCategory.Ranged:
            case WeaponCategory.SciFi:
                player.animator.CrossFade("RANGED_ATTACK", 0.1f, 2);
                break;
            case WeaponCategory.Throwable:
                player.animator.CrossFade("THROW_ATTACK", 0.1f, 2);
                break;
        }

        // ✅ 무기 컨트롤러 경로만 사용 (강화/크리 합산 보장)
        float duration = player.animator.GetCurrentAnimatorStateInfo(2).length;
        weaponCtrl.SpawnDamageField(duration);

        player.attackPressed = false;
    }
}
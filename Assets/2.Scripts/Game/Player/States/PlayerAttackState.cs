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

        // attackPressed 가 true 면 콤보 이어가기
        if (player.attackPressed && _comboTimer <= ComboDelay && _comboStep < MaxCombo)
        {
            _comboStep++;
            _comboTimer = 0f;
            PlayCombo(player);
        }

        // 딜레이 초과 시 이동/대기 상태로 복귀
        if (_comboTimer > ComboDelay)
        {
            bool moving = player.moveInput.sqrMagnitude > 0.01f;
            player.ChangeState(moving ? new PlayerMoveState() : new PlayerIdleState());
        }
    }

    public void Exit(PlayerStateMachine player)
    {
        // 상태 벗어날 때 반드시 초기화
        player.attackPressed = false;
    }

    private void PlayCombo(PlayerStateMachine player)
    {
        player.RotateTowardsCameraDirection();
    
        WeaponController weaponCtrl = player.GetComponent<WeaponController>();
        WeaponType type = weaponCtrl.currentWeaponType;
        WeaponData data = GameManager.Instance.GetWeaponData(type);
    
        switch (data.category)
        {
            case WeaponCategory.Melee:
                player.animator.CrossFade($"ATTACK{_comboStep}", 0.1f, 2);
                break;
    
            case WeaponCategory.Ranged:
            case WeaponCategory.SciFi:
                player.animator.CrossFade("RANGED_ATTACK", 0.1f, 1);
                // "RangedAttack" 트리거 발동
                player.animator.SetTrigger("RangedAttack");
                break;
    
            case WeaponCategory.Throwable:
                player.animator.CrossFade("THROW_ATTACK", 0.1f, 2);
                break;
        }
    
        SpawnDamageField(player);
        player.attackPressed = false;
    }
    
    

    private void SpawnDamageField(PlayerStateMachine player)
    {
        var weaponCtrl = player.GetComponent<WeaponController>();
        WeaponType type = weaponCtrl.currentWeaponType;
        WeaponData data = GameManager.Instance.GetWeaponData(type);

        GameObject field = GameManager.Instance.GetDamageField();
        // owner 로 player.gameObject 를 넘겨서 self-harm 방지
        field.GetComponent<DamageField>()
             .Initialize(player.gameObject, data.damage, data.range, player.animator.GetCurrentAnimatorStateInfo(2).length);

        // 필드 위치: 플레이어 앞(data.range/2)
        field.transform.position = player.transform.position + player.transform.forward * (data.range * 0.5f);
        field.SetActive(true);
    }
}
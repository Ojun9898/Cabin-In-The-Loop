// PlayerHealth.cs
using UnityEngine;

[RequireComponent(typeof(PlayerStateMachine))]
public class PlayerHealth : MonoBehaviour, IDamageable
{
    public float maxHealth = 100f;
    private float _currentHealth;
    private PlayerStateMachine _psm;

    void Awake()
    {
        _currentHealth = maxHealth;
        _psm = GetComponent<PlayerStateMachine>();
    }

    // 데미지 입으면 체력 차감 후 HitState 전이
    public void TakeDamage(float amount)
    {
        _currentHealth -= amount;
        Debug.Log($"Player took {amount} dmg, remaining {_currentHealth}");

        _psm.ChangeState(new PlayerHitState());

        if (_currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player Died");
        // 게임오버 처리, 리스폰 등
    }
}
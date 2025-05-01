using System;
using UnityEngine;

public class MonsterHealth
{
    private int currentHealth;
    private int maxHealth;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;

    public event Action<int> OnHealthChanged;
    public event Action OnDeath;

    public MonsterHealth(int maxHealth)
    {
        this.maxHealth = maxHealth;
        this.currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth = Mathf.Max(0, currentHealth - damage);
        OnHealthChanged?.Invoke(currentHealth);

        if (currentHealth <= 0)
        {
            OnDeath?.Invoke();
        }
    }
    
}

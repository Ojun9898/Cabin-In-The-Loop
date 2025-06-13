using UnityEngine;
using UnityEngine.SceneManagement;

public class HealthSystemForDummies : MonoBehaviour
{
    public bool IsAlive = true;
    public float CurrentHealth = 1000;
    public float MaximumHealth = 1000;

    public bool HasAnimationWhenHealthChanges = true;
    public float AnimationDuration = 0.1f;

    private EndingManager endingManager;

    public float CurrentHealthPercentage
    {
        get
        {
            return (CurrentHealth / MaximumHealth) * 100f;
        }
    }

    public OnCurrentHealthChanged OnCurrentHealthChanged = new OnCurrentHealthChanged();
    public OnIsAliveChanged OnIsAliveChanged = new OnIsAliveChanged();
    public OnMaximumHealthChanged OnMaximumHealthChanged = new OnMaximumHealthChanged();

    public GameObject HealthBarPrefabToSpawn;

    void Start()
    {
        SetHealthBarReference();

        endingManager = FindObjectOfType<EndingManager>();
        if (endingManager != null)
        {
            OnIsAliveChanged.AddListener(endingManager.UpdateIsAlive);
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SetHealthBarReference();
    }

    private void SetHealthBarReference()
    {
        if (this.CompareTag("Monster"))
        {
            // 현재 GameObject의 모든 자식(비활성 포함) 가져오기
            Transform[] children = GetComponentsInChildren<Transform>(true);

            foreach (Transform child in children)
            {
                if (child.name == "HealthBarCanvas")
                {
                    HealthBarPrefabToSpawn = child.gameObject;
                    break;
                }
            }
        }
    }

    public void AddToMaximumHealth(float value)
    {
        float cachedMaximumHealth = MaximumHealth;
        MaximumHealth += value;
        OnMaximumHealthChanged.Invoke(new MaximumHealth(cachedMaximumHealth, MaximumHealth));
    }

    public void AddToCurrentHealth(float value)
    {
        if (value == 0) return;

        float cachedCurrentHealth = CurrentHealth;

        if (value > 0)
        {
            GotHealedFor(value);
        }
        else
        {
            TakeDamage(Mathf.Abs(value));
        }

        OnCurrentHealthChanged.Invoke(new CurrentHealth(cachedCurrentHealth, CurrentHealth, CurrentHealthPercentage));
    }

    void GotHealedFor(float value)
    {
        CurrentHealth += value;

        if (CurrentHealth > MaximumHealth)
            CurrentHealth = MaximumHealth;

        if (!IsAlive)
            ReviveWithCustomHealth(CurrentHealth);
    }

    public void TakeDamage(float amount)
    {
        if (!IsAlive) return;

        float cachedCurrentHealth = CurrentHealth;

        CurrentHealth -= amount;
        Debug.Log($"Took {amount} damage, remaining health: {CurrentHealth}");

        if (CurrentHealth <= 0)
        {
            Die();
        }

        OnCurrentHealthChanged.Invoke(new CurrentHealth(cachedCurrentHealth, CurrentHealth, CurrentHealthPercentage));
    }

    private void Die()
    {
        if (!IsAlive) return;

        IsAlive = false;
        Debug.Log(this.name + "died");

        OnIsAliveChanged.Invoke(IsAlive);
    }

    public void ReviveWithMaximumHealth()
    {
        Revive(MaximumHealth);
    }

    public void ReviveWithCustomHealth(float healthWhenRevived)
    {
        Revive(healthWhenRevived);
    }

    public void ReviveWithCustomHealthPercentage(float healthPercentageWhenRevived)
    {
        Revive(MaximumHealth * (healthPercentageWhenRevived / 100f));
    }

    void Revive(float health)
    {
        float previousHealth = CurrentHealth;

        CurrentHealth = health;
        IsAlive = true;

        OnIsAliveChanged.Invoke(IsAlive);
        OnCurrentHealthChanged.Invoke(new CurrentHealth(previousHealth, CurrentHealth, CurrentHealthPercentage));
    }

    public void Kill()
    {
        float previousHealth = CurrentHealth;

        CurrentHealth = 0;
        IsAlive = false;

        OnIsAliveChanged.Invoke(IsAlive);
        OnCurrentHealthChanged.Invoke(new CurrentHealth(previousHealth, CurrentHealth, CurrentHealthPercentage));
    }
}

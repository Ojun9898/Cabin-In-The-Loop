using UnityEngine;
using UnityEngine.UI;

public class StatusUpgradeUI : MonoBehaviour
{
    public CharacterType selectedCharacter = CharacterType.Female;

    public void UpgradeHealth()
    {
        UpgradeStat(StatusType.Health, 10f);
    }

    public void UpgradeAttack()
    {
        UpgradeStat(StatusType.Attack, 2f);
    }

    public void UpgradeSpeed()
    {
        UpgradeStat(StatusType.Speed, 1f);
    }

    public void UpgradeCritChance()
    {
        UpgradeStat(StatusType.CritChance, 0.01f);
    }

    public void UpgradeCritMultiplier()
    {
        UpgradeStat(StatusType.CritMultiplier, 0.05f);
    }

    private void UpgradeStat(StatusType statType, float amount)
    {
        string key = $"{selectedCharacter}_{statType}_Base";
        float current = PlayerPrefs.GetFloat(key, 0f);
        PlayerPrefs.SetFloat(key, current + amount);
        PlayerPrefs.Save();

        Debug.Log($"{statType} 강화됨: {current} → {current + amount}");
    }
}
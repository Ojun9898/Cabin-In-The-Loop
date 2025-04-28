using UnityEngine;

public static class WeaponSaveSystem
{
    private const string WeaponKey = "Player_WeaponType";

    // 무기 저장
    public static void SaveWeapon(WeaponType weaponType)
    {
        PlayerPrefs.SetInt(WeaponKey, (int)weaponType);
        PlayerPrefs.Save();
    }

    // 무기 불러오기
    public static WeaponType LoadWeapon()
    {
        if (PlayerPrefs.HasKey(WeaponKey))
        {
            return (WeaponType)PlayerPrefs.GetInt(WeaponKey);
        }

        return WeaponType.Axe; // 기본 무기
    }

    // 저장 초기화 (디버그용)
    public static void Reset()
    {
        PlayerPrefs.DeleteKey(WeaponKey);
    }
}
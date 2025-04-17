using UnityEngine;

[CreateAssetMenu(menuName = "Weapons/WeaponData")]
public class WeaponData : ScriptableObject
{
    public WeaponType weaponType;
    public GameObject prefab;
    public string displayName;
    public float damage;
    public Sprite icon;
    // 확장 가능
}
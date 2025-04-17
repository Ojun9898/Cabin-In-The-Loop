using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("무기를 장착할 위치")]
    public Transform handSocket;

    private GameObject _currentWeapon;
    private WeaponType _currentWeaponType;

    // 무기 장착
    public void EquipWeapon(WeaponType weaponType)
    {
        // 기존 무기 제거
        UnEquipWeapon();

        // GameManager에서 무기 가져오기
        _currentWeapon = GameManager.Instance.GetWeapon(weaponType);

        if (_currentWeapon != null)
        {
            _currentWeaponType = weaponType;

            // 손에 장착
            _currentWeapon.transform.SetParent(handSocket);
            _currentWeapon.transform.localPosition = Vector3.zero;
            _currentWeapon.transform.localRotation = Quaternion.identity;
        }
    }

    // 무기 해제
    public void UnEquipWeapon()
    {
        if (_currentWeapon != null)
        {
            GameManager.Instance.ReturnWeapon(_currentWeaponType, _currentWeapon);
            _currentWeapon = null;
        }
    }

    // 현재 무기 타입 반환 (저장용)
    public WeaponType GetCurrentWeaponType()
    {
        return _currentWeapon != null ? _currentWeaponType : WeaponType.Axe; // 기본값
    }
}
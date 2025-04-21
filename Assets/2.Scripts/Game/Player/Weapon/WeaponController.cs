using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("무기를 장착할 위치")]
    public Transform handSocket;

    private GameObject _currentWeapon;
    public WeaponType currentWeaponType;

    /// <summary>
    /// 주어진 무기 타입으로 무기 장착
    /// </summary>
    public void EquipWeapon(WeaponType weaponType)
    {
        UnEquipWeapon();  // 기존 무기 해제

        // GameManager에서 풀링된 무기 가져오기
        _currentWeapon = GameManager.Instance.GetWeapon(weaponType);

        if (_currentWeapon != null)
        {
            currentWeaponType = weaponType;
            
            // 지정된 소켓(손)에 부착
            _currentWeapon.transform.SetParent(handSocket);
            _currentWeapon.transform.localPosition = Vector3.zero;
            _currentWeapon.transform.localRotation = Quaternion.identity;
        }
    }

    /// <summary>
    /// 현재 장착된 무기를 풀에 반환 및 해제
    /// </summary>
    public void UnEquipWeapon()
    {
        if (_currentWeapon != null)
        {
            GameManager.Instance.ReturnWeapon(currentWeaponType, _currentWeapon);
            _currentWeapon = null;
        }
    }

    /// <summary>
    /// 현재 장착된 무기 타입 반환 (없으면 기본 Axe)
    /// </summary>
    public WeaponType GetCurrentWeaponType()
    {
        return _currentWeapon != null ? currentWeaponType : WeaponType.Axe;
    }

    /// <summary>
    /// 오브젝트 풀링된 데미지 필드를 생성 및 초기화
    /// </summary>
    /// <param name="duration">데미지 필드 지속 시간 (애니메이션 길이 등)</param>
    public void SpawnDamageField(float duration)
    {
        // 1) 무기 데이터(데미지, 사거리) 가져오기
        WeaponData data = GameManager.Instance.GetWeaponData(currentWeaponType);

        // 2) 데미지 필드 오브젝트 꺼내기
        GameObject field = GameManager.Instance.GetDamageField();

        // 3) 생성 위치: 손 소켓 기준 전방으로 사거리의 절반 만큼 이동
        Vector3 spawnPos = handSocket.position + handSocket.forward * (data.range * 0.5f);
        field.transform.position = spawnPos;

        // 4) 활성화 및 초기화 (owner=자신, 피해량, 반경, 지속시간)
        field.SetActive(true);
        field.GetComponent<DamageField>()
             .Initialize(gameObject, data.damage, data.range, duration);
    }
}
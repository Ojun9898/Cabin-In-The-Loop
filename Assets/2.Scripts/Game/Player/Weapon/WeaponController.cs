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
        // 1) 무기 데이터 가져오기
        WeaponData data = GameManager.Instance.GetWeaponData(currentWeaponType);

        // 2) PlayerStatus의 CalculateDamage()로 최종 데미지 계산 (크리티컬 포함)
        float totalDamage = PlayerStatus.Instance.CalculateDamage(data);

        // 3) 풀에서 데미지 필드 꺼내기
        GameObject field = GameManager.Instance.GetDamageField();

        // 4) 생성 위치 계산 (손 소켓 기준 전방 절반 거리)
        Vector3 spawnPos = handSocket.position + handSocket.forward * (data.range * 0.5f);
        field.transform.position = spawnPos;

        // 5) 활성화 및 초기화 (owner, damage, radius, duration)
        field.SetActive(true);
        field.GetComponent<DamageField>()
            .Initialize(gameObject, totalDamage, data.range, duration);
    }
}
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

            if (PlayerStatus.Instance != null)
                PlayerStatus.Instance.SetCurrentWeaponType(weaponType);
            
            // 지정된 소켓(손)에 부착
            _currentWeapon.transform.SetParent(handSocket);
            _currentWeapon.transform.localPosition = Vector3.zero;
            _currentWeapon.transform.localRotation = Quaternion.identity;

            // 불필요: PlayerStatus에 무기 저장을 안 하므로 SaveData 의미 없음
            // PlayerStatus.Instance.SaveData();
        }
        else
        {
            Debug.LogWarning($"EquipWeapon: 풀에서 {weaponType} 무기를 가져오지 못했습니다.");
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
    public void SpawnDamageField(float duration)
    {
        // 무기 미장착 가드
        if (_currentWeapon == null)
        {
            Debug.LogWarning("SpawnDamageField: 장착된 무기가 없습니다.");
            return;
        }

        // PlayerStatus 존재 가드
        if (PlayerStatus.Instance == null)
        {
            Debug.LogWarning("SpawnDamageField: PlayerStatus 인스턴스를 찾을 수 없습니다.");
            return;
        }

        // 1) 무기 데이터 가져오기
        WeaponData data = GameManager.Instance.GetWeaponData(currentWeaponType);
        if (data == null)
        {
            Debug.LogWarning($"SpawnDamageField: {currentWeaponType}의 WeaponData를 가져오지 못했습니다.");
            return;
        }

        // 2) 최종 데미지 계산
        float totalDamage = PlayerStatus.Instance.CalculateDamage(data);

        // 3) 풀에서 데미지 필드 꺼내기
        GameObject field = GameManager.Instance.GetDamageField();
        if (field == null)
        {
            Debug.LogWarning("SpawnDamageField: DamageField를 풀에서 가져오지 못했습니다.");
            return;
        }

        // 4) 생성 위치 계산
        Vector3 spawnPos = handSocket.position + handSocket.forward * (data.range * 0.5f);
        field.transform.position = spawnPos;

        // 5) 활성화 및 초기화
        field.SetActive(true);
        field.GetComponent<DamageField>()
             .Initialize(gameObject, totalDamage, data.range, duration);
    }
}

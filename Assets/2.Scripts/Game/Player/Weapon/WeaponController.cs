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
        if (_currentWeapon == null)
        {
            Debug.LogWarning("SpawnDamageField: 장착된 무기가 없습니다.");
            return;
        }
        if (PlayerStatus.Instance == null)
        {
            Debug.LogWarning("SpawnDamageField: PlayerStatus 인스턴스를 찾을 수 없습니다.");
            return;
        }

        WeaponData data = GameManager.Instance.GetWeaponData(currentWeaponType);
        if (data == null)
        {
            Debug.LogWarning($"SpawnDamageField: {currentWeaponType}의 WeaponData를 가져오지 못했습니다.");
            return;
        }

        float totalDamage = PlayerStatus.Instance.CalculateDamage(data);

        // 1) 풀에서 루트 오브젝트 꺼내기 (보통 비활성 상태)
        GameObject root = GameManager.Instance.GetDamageField();
        if (root == null)
        {
            Debug.LogWarning("SpawnDamageField: DamageField를 풀에서 가져오지 못했습니다.");
            return;
        }

        // 2) DamageField 컴포넌트를 '자식 포함'해서 가져오기 (비활성 자식도 찾기 위해 true)
        DamageField df = root.GetComponentInChildren<DamageField>(true);
        if (df == null)
        {
            Debug.LogError("SpawnDamageField: DamageField 컴포넌트를 찾을 수 없습니다. 프리팹 구조를 확인하세요.");
            return;
        }

        // 3) 스폰 위치 지정 (원래 로직 유지)
        Vector3 spawnPos = handSocket.position + handSocket.forward * (data.range * 0.5f);
        root.transform.position = spawnPos;

        // 4) '컴포넌트가 붙은 바로 그 GameObject'를 먼저 활성화
        //    (자식이 꺼져 있던 케이스를 확실히 커버)
        if (!df.gameObject.activeSelf)
            df.gameObject.SetActive(true);

        // 5) 루트도 활성화 (둘 다 켜서 활성 계층 보장)
        if (!root.activeSelf)
            root.SetActive(true);

        // 6) Initialize는 '활성화 후' 호출 (StartCoroutine 보장)
        df.Initialize(gameObject, totalDamage, data.range, duration);
    }

}

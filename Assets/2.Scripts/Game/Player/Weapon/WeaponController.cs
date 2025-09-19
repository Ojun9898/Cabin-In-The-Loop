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
    
            
            var ps = PlayerStatus.Ensure();
            if (ps != null)
                ps?.SetCurrentWeaponType(weaponType);
            
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

        var ps = PlayerStatus.Ensure();
        if (ps == null)
        {
            Debug.LogWarning("SpawnDamageField: PlayerStatus 인스턴스를 확보하지 못했습니다.");
            return;
        }

        WeaponData data = GameManager.Instance.GetWeaponData(currentWeaponType);
        if (data == null)
        {
            Debug.LogWarning($"SpawnDamageField: {currentWeaponType}의 WeaponData를 가져오지 못했습니다.");
            return;
        }

        float atk = ps.GetTotalStat(StatusType.Attack);
        float totalDamage = ps.CalculateDamage(data);
        // Debug.Log($"[SpawnDamageField] char={ps.CharacterType} base={data.damage} atk={atk} total={totalDamage}");

        GameObject root = GameManager.Instance.GetDamageField();
        if (root == null)
        {
            Debug.LogWarning("SpawnDamageField: DamageField를 풀에서 가져오지 못했습니다.");
            return;
        }

        DamageField df = root.GetComponentInChildren<DamageField>(true);
        if (df == null)
        {
            Debug.LogError("SpawnDamageField: DamageField 컴포넌트를 찾을 수 없습니다. 프리팹 구조를 확인하세요.");
            return;
        }

        Vector3 spawnPos = handSocket.position + handSocket.forward * (data.range * 0.5f);
        root.transform.position = spawnPos;

        if (!df.gameObject.activeSelf) df.gameObject.SetActive(true);
        if (!root.activeSelf) root.SetActive(true);

        df.Initialize(gameObject, totalDamage, data.range, duration);
    }
}

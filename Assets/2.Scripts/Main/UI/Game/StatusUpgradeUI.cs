using UnityEngine;
using UnityEngine.UI;

public class StatusUpgradeUI : MonoBehaviour
{
    [Header("강화 대상 캐릭터")]
    public CharacterType selectedCharacter = CharacterType.Female;

    [Header("강화 수치 설정")]
    public float healthAmount = 10f;
    public float attackAmount = 5f;
    public float speedAmount  = 0.1f;

    private PlayerStatus _playerStatus;

    private void Start()
    {
        _playerStatus = PlayerStatus.Ensure(); // PlayerStatus가 없을 경우 이 한 줄로 해결

        if (_playerStatus.characterType != selectedCharacter)
            _playerStatus.SetCharacter(selectedCharacter);
    }

    // ========== 버튼에 직접 연결할 메서드 3개 ==========

    public void UpgradeHealth()
    {
        UpgradeStatInternal(StatusType.Health, healthAmount);

        // 참고: PlayerStatus.IncreaseBaseStat에
        // Health 강화 시 _maxHealth/_currentHealth 갱신 로직이 없다면,
        // 반드시 PlayerStatus 쪽에 반영 권장!
        // (UI에서 직접 만지는 건 비권장)
    }

    public void UpgradeAttack()
    {
        UpgradeStatInternal(StatusType.Attack, attackAmount);
    }

    public void UpgradeSpeed()
    {
        UpgradeStatInternal(StatusType.Speed, speedAmount);
    }

    // ========== 내부 공통 처리 ==========

    private void UpgradeStatInternal(StatusType statType, float amount)
    {
        if (_playerStatus == null)
        {
            Debug.LogError("PlayerStatus 인스턴스를 찾을 수 없습니다!");
            return;
        }

        _playerStatus.IncreaseBaseStat(statType, amount);

        float newValue = _playerStatus.GetTotalStat(statType);
        Debug.Log($"{statType} 강화됨 → 현재 값: {newValue}");
    }
}
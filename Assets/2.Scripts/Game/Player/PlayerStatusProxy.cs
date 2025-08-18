using UnityEngine;

public class PlayerStatusProxy : MonoBehaviour, IDamageable
{
    // 필요하면 외부가 전역 상태를 가져갈 때 사용할 수 있는 브리지
    public PlayerStatus Status => PlayerStatus.Ensure();

    // ==== IDamageable 위임 ====
    public void TakeDamage(float amount)
    {
        Status.TakeDamage(amount);
    }
    
    // ==== 선택: 경험치/강화 유틸 위임(픽업/트리거가 GetComponent로 접근하는 경우 대비) ====
    public void GainXp(float amount) => Status.GainXp(amount);

    public float GetStat(StatusType type) => Status.GetTotalStat(type);
}
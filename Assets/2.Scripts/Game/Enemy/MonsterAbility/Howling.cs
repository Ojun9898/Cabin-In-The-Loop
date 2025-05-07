using System.Collections.Generic;
using UnityEngine;

public class Howling : MonoBehaviour
{
    [SerializeField] private float howlDamage = 20f;
    private ParticleSystem howlParticle;
    private bool hasDealtDamage = false; // 데미지가 이미 적용되었는지 확인하는 플래그

    private void Awake()
    {
        howlParticle = GetComponent<ParticleSystem>();
        if (howlParticle == null)
        {
            Debug.LogError("ParticleSystem component is not found on Howling.");
        }
    }

    private void OnEnable()
    {
        hasDealtDamage = false; // 파티클이 활성화될 때 플래그 초기화
    }
    
    private void OnParticleTrigger()
    {
        // 이미 데미지를 주었으면 더 이상 처리하지 않음
        if (hasDealtDamage) return;

        // 충돌한 파티클 저장
        List<ParticleSystem.Particle> insideParticles = new List<ParticleSystem.Particle>();

        // 파티클 중 Trigger 내부에 있는 것 가져오기
        int count = howlParticle.GetTriggerParticles(ParticleSystemTriggerEventType.Inside, insideParticles);

        // 감지된 파티클이 있으면 데미지 적용
        if (count > 0)
        {
            DealHowlingDamageToPlayer();
            hasDealtDamage = true; // 데미지 적용 플래그 설정
        }
    }
    
    private void DealHowlingDamageToPlayer()
    {
        // 플레이어에게 데미지 적용
        PlayerStatus playerHealth = FindObjectOfType<PlayerStatus>();
        if (playerHealth != null)
        {
            Debug.Log($"Applying {howlDamage} damage to the player.");
            playerHealth.TakeDamage(howlDamage);
        }
    }
    
    public void SetHowlDamage(float damage)
    {
        howlDamage = damage; // 외부에서 데미지 설정 가능
    }
}
using UnityEngine;

public class WeaponEffectController : MonoBehaviour
{
    public TrailRenderer weaponTrail; 
    public GameObject slashParticlePrefab; 
    private GameObject currentSlashParticleInstance; 

    // 새로 추가할 변수
    [Header("오디오 설정")]
    [Tooltip("무기 휘두르는 소리 오디오 클립")]
    public AudioClip swingSound; // 휘두르는 소리 오디오 클립
    private AudioSource audioSource; // AudioSource 컴포넌트 참조

    void Awake() // Start() 대신 Awake()에서 초기화하는 것이 좋습니다.
    {
        // AudioSource 컴포넌트 참조 가져오기 또는 추가
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            // 필요하다면 여기서 AudioSource 기본 설정 (예: spatialBlend = 1f;)
        }

        // 시작 시 궤적과 파티클 비활성화 (이전 코드 유지)
        if (weaponTrail != null)
        {
            weaponTrail.emitting = false;
        }
        if (slashParticlePrefab != null)
        {
            // 파티클 프리팹가 씬에 직접 자식으로 있다면 비활성화
            // Instantiate로 생성하는 방식이라면 필요 없음
            // slashParticlePrefab.SetActive(false);
        }
    }

    public void OnAttackStart()
    {
        // (이전 코드 유지) Trail Renderer 활성화, 파티클 활성화/생성

        // 휘두르는 소리 재생
        if (swingSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(swingSound); // 한 번만 재생
        }
    }

    public void OnAttackEnd()
    {
        // (이전 코드 유지) Trail Renderer 비활성화, 파티클 비활성화
    }
}
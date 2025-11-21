using UnityEngine;

public enum PickupType { Gold, Potion, Material, Equipment }

public class ItemPickup : MonoBehaviour
{
    public PickupType type = PickupType.Gold;
    public int amount = 1;
    public float autoDestroyAfter = 120f; // 2분 뒤 정리 (옵션)

    [Header("픽업 사운드")]
    [SerializeField] private AudioClip pickupSfx;
    [Range(0f, 1f)]
    [SerializeField] private float baseVolume = 0.7f;

    [Tooltip("여러 아이템을 동시에 먹을 때, 사운드가 너무 많이 겹치지 않도록 하는 최소 간격(초)")]
    [SerializeField] private float minIntervalBetweenSfx = 0.06f;

    [Header("피치 랜덤 범위")]
    [SerializeField] private float minPitch = 0.95f;
    [SerializeField] private float maxPitch = 1.05f;

    // 여러 아이템이 한꺼번에 주워질 때, 사운드 난사 방지용
    private static float s_lastPlayTime = -999f;

    void Start()
    {
        if (autoDestroyAfter > 0)
            Destroy(gameObject, autoDestroyAfter);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // 1) 실제 효과 처리 (골드/포션/재료/장비 등)
        ApplyPickup(other.gameObject);

        // 2) 픽업 사운드 재생 (간격 + 피치 랜덤 적용)
        PlayPickupSound();

        // 3) 아이템 제거
        Destroy(gameObject);
    }

    private void ApplyPickup(GameObject player)
    {
        // 프로젝트 시스템에 맞게 처리
        switch (type)
        {
            case PickupType.Gold:
                PlayerStatus.Instance?.AddGold(amount);
                break;

            case PickupType.Potion:
                // 예: PlayerStatus.Instance.Heal(amount);
                break;

            case PickupType.Material:
                // 재료 인벤토리 추가 로직
                break;

            case PickupType.Equipment:
                // 장비 인벤토리 추가 로직
                break;
        }

        Debug.Log($"Picked: {type} x{amount}");
    }

    private void PlayPickupSound()
    {
        if (pickupSfx == null) return;

        float t = Time.time;

        // 너무 짧은 시간 안에 또 재생하려 하면 스킵
        if (t - s_lastPlayTime < minIntervalBetweenSfx)
            return;

        s_lastPlayTime = t;

        // 임시 오디오 오브젝트 생성해서 피치까지 조절 후 재생
        GameObject audioObj = new GameObject("PickupSFX");
        audioObj.transform.position = transform.position;

        AudioSource source = audioObj.AddComponent<AudioSource>();
        source.clip = pickupSfx;
        source.volume = baseVolume;
        source.spatialBlend = 1f; // 3D 사운드
        source.pitch = Random.Range(minPitch, maxPitch);

        source.Play();

        // 피치에 따라 실제 재생 길이가 달라지니, 그만큼 지난 뒤 오브젝트 제거
        float duration = pickupSfx.length / Mathf.Max(source.pitch, 0.01f);
        Destroy(audioObj, duration + 0.05f);
    }
}

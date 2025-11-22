using UnityEngine;

public class LootSpawner : MonoBehaviour
{
    [Header("몬스터 드랍 이벤트")]
    public MonsterDropEvent monsterDropEvent;

    [Min(0)] public int maxDifferentDrops = 2;
    [Tooltip("드랍 위치를 중심에서 얼마나 흩뿌릴지 반경")]
    public float scatterRadius = 1.5f;

    [Tooltip("드랍 시 처음 위로 튕기는 힘 세기")]
    public float upForce = 2f;

    [Tooltip("땅을 찾기 위한 레이캐스트 마스크")]
    public LayerMask groundMask = ~0;

    [Tooltip("몬스터 위치 기준으로 이 높이에서부터 아래로 레이캐스트")]
    public float groundRayHeight = 3f;

    [Header("드랍 이펙트")]
    public ParticleSystem spawnVfx;

    [Header("아이템 레이어 설정")]
    [SerializeField] private string itemLayerName = "Item";
    private int itemLayer = -1;

    private void Awake()
    {
        // Item 레이어 인덱스 가져오기
        itemLayer = LayerMask.NameToLayer(itemLayerName);
        if (itemLayer == -1)
        {
            Debug.LogWarning($"[LootSpawner] '{itemLayerName}' 레이어를 찾을 수 없습니다. " +
                             "Project Settings > Tags and Layers 에서 레이어를 추가해 주세요.");
        }
        else
        {
            // Item 레이어끼리의 충돌은 무시 (아이템들끼리 서로 떠받치는 것 방지)
            Physics.IgnoreLayerCollision(itemLayer, itemLayer, true);
        }
    }

    public void SpawnLoot(Vector3 deathPos)
    {
        if (monsterDropEvent == null) return;

        int spawned = 0;
        int safety = 20;

        while (spawned < maxDifferentDrops && safety-- > 0)
        {
            if (!monsterDropEvent.TryGetDrop(out var drop)) break;
            if (drop == null || drop.prefab == null) continue;

            int qty = Mathf.Clamp(
                Random.Range(drop.quantityRange.x, drop.quantityRange.y + 1),
                0, 999
            );
            if (qty <= 0) continue;

            spawned++;

            for (int i = 0; i < qty; i++)
            {
                Vector3 pos = GetDropPoint(deathPos);
                var go = Instantiate(drop.prefab, pos, Quaternion.identity);

                // 아이템 레이어로 강제 설정 (자식 포함)
                if (itemLayer != -1)
                    SetLayerRecursively(go, itemLayer);

                if (go.TryGetComponent<Rigidbody>(out var rb))
                {
                    // 수평 방향 + 약간 위쪽으로 튕기기 (너무 위로 안 날아가게)
                    Vector3 randomHorizontal = new Vector3(
                        Random.Range(-1f, 1f),
                        0f,
                        Random.Range(-1f, 1f)
                    );

                    Vector3 forceDir = (randomHorizontal.normalized + Vector3.up * 0.5f).normalized;
                    rb.AddForce(forceDir * upForce, ForceMode.VelocityChange);
                }
            }
        }

        if (spawned > 0)
        {
            if (spawnVfx) Instantiate(spawnVfx, deathPos, Quaternion.identity);
        }
    }

    /// <summary>
    /// 몬스터 중심에서 랜덤 오프셋 위치를 잡고, 그 위에서 아래로 레이캐스트해서
    /// 실제 지면 위의 드랍 포인트를 구함.
    /// </summary>
    Vector3 GetDropPoint(Vector3 center)
    {
        Vector2 offset = Random.insideUnitCircle * scatterRadius;
        Vector3 start = new Vector3(center.x + offset.x,
                                    center.y + groundRayHeight,
                                    center.z + offset.y);

        if (Physics.Raycast(start, Vector3.down, out var hit, groundRayHeight * 2f, groundMask))
        {
            // 살짝 위로 띄워서 콜라이더가 땅에 박히지 않도록
            return hit.point + Vector3.up * 0.05f;
        }

        // 혹시 레이캐스트가 실패하면 그냥 평면 기준으로 반환
        return center + new Vector3(offset.x, 0f, offset.y);
    }

    /// <summary>
    /// GameObject와 모든 자식 오브젝트의 레이어를 재귀적으로 변경
    /// </summary>
    private void SetLayerRecursively(GameObject obj, int layer)
    {
        if (obj == null) return;

        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            if (child == null) continue;
            child.gameObject.layer = layer;
        }
    }
}

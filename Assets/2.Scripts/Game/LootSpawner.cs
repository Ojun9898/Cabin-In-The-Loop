using UnityEngine;

public class LootSpawner : MonoBehaviour
{
    [Header("몬스터 드랍 이벤트")]
    public MonsterDropEvent monsterDropEvent;

    [Min(0)] public int maxDifferentDrops = 2;
    public float scatterRadius = 1.5f;
    public float upForce = 2f;
    public LayerMask groundMask = ~0;
    public float groundRayHeight = 3f;

    public ParticleSystem spawnVfx;
    public AudioClip spawnSfx;
    public float sfxVolume = 0.7f;

    public void SpawnLoot(Vector3 deathPos)
    {
        if (monsterDropEvent == null) return;

        int spawned = 0;
        int safety = 20;

        while (spawned < maxDifferentDrops && safety-- > 0)
        {
            if (!monsterDropEvent.TryGetDrop(out var drop)) break;
            if (drop == null || drop.prefab == null) continue;

            int qty = Mathf.Clamp(Random.Range(drop.quantityRange.x, drop.quantityRange.y + 1), 0, 999);
            if (qty <= 0) continue;

            spawned++;

            for (int i = 0; i < qty; i++)
            {
                Vector3 pos = GetDropPoint(deathPos);
                var go = Instantiate(drop.prefab, pos, Quaternion.identity);
                if (go.TryGetComponent<Rigidbody>(out var rb))
                    rb.AddForce(Random.insideUnitSphere + Vector3.up * upForce, ForceMode.VelocityChange);
            }
        }

        if (spawned > 0)
        {
            if (spawnVfx) Instantiate(spawnVfx, deathPos, Quaternion.identity);
            if (spawnSfx) AudioSource.PlayClipAtPoint(spawnSfx, deathPos, sfxVolume);
        }
    }

    Vector3 GetDropPoint(Vector3 center)
    {
        Vector2 offset = Random.insideUnitCircle * scatterRadius;
        Vector3 start = new Vector3(center.x + offset.x, center.y + groundRayHeight, center.z + offset.y);
        if (Physics.Raycast(start, Vector3.down, out var hit, groundRayHeight * 2f, groundMask))
            return hit.point + Vector3.up * 0.05f;

        return center + new Vector3(offset.x, 0f, offset.y);
    }
}

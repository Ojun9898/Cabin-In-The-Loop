using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject[] spawnPrefabs;
    
    public float maxTime = 3f;
    public float timer = 0f;

    public void CreateMonster()
    {
        timer += Time.deltaTime;

        if (timer >= maxTime)
        {
            timer = 0f;
            
            // int randomIndex = Random.Range(0, 3);
            
            // GameObject monsterObj = Instantiate(spawnPrefabs[0], this.transform);
            // GameObject monsterObj = CustomPoolManager.Instance.GetPool();
            GameObject monsterObj = GameManager.Instance.poolManager.GetPoolObject(spawnPrefabs[Random.Range(0, 3)]);
            
            monsterObj.transform.position = new Vector3(Random.Range(-3, 0), 0, -5);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    // 생성하려는 프리팹
    public GameObject[] spawnPrefabs;
    
    // 생성하는 주기
    public float maxTime = 3f;
    public float timer = 0f;

    void Start()
    {
        InvokeRepeating("CreateMonster", 0f, 3f);
    }

    //생성 기능
    void Update()
    {
        // 타이머
        timer += Time.deltaTime;

        if (timer >= maxTime)
        {
            timer = 0f;
            
            // 몬스터 생성
            // 랜덤하게 몬스터 생성
            int randomIndex = Random.Range(0, 3);
            
            GameObject monsterObj = Instantiate(spawnPrefabs[randomIndex]);
            
            monsterObj.transform.SetParent(this.transform);
            
            // 생성한 몬스터의 위치 = SpawnManager의 위치로 적용
            monsterObj.transform.position = this.transform.position;

        }
    }

    private void CreateMonster()
    {
        int randomIndex = Random.Range(0, 3);
            
        GameObject monsterObj = Instantiate(spawnPrefabs[randomIndex]);
            
        monsterObj.transform.SetParent(this.transform);
            
        // 생성한 몬스터의 위치 = SpawnManager의 위치로 적용
        monsterObj.transform.position = this.transform.position;
    }
}




































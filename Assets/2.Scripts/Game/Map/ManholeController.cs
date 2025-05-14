using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ManholeController : MonoBehaviour
{
    [SerializeField] private Animation anim;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private ManholeButtomController bc;
    private GameObject monster;

    private bool isPlayerInManhole = false;
    private bool isManholeOpen = false;
    private bool isPlayerInsideManhole = false;
    private bool isMonsterSpawned = false;

    /*void Start()
    {
        monster = GameObject.FindWithTag("Monster");
        if (monster == null)
        {
            Debug.Log("monster is null");
        }
    }*/
    
    void Update()
    {
        if (monster == null)
        {
            monster = GameObject.FindWithTag("Monster");
        }
        
        if (monster != null)
            isMonsterSpawned = monster.GetComponent<Monster>().isMonsterSpawned;
        else
            isMonsterSpawned = false;

        isPlayerInsideManhole = bc != null && bc.isPlayerInsideManhole;
        
        /*isMonsterSpawned = monster.GetComponent<Monster>().isMonsterSpawned;
        isPlayerInsideManhole = bc.isPlayerInsideManhole;*/
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInManhole = true;
            
            if (!isManholeOpen)
            {
                OpenManhole();
            }

            // 실험실 이동
            if (isPlayerInsideManhole)
            {
                SceneManager.LoadScene("(Bake)Laboratory");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInManhole = false;
        }
    }

    // 몬스터 스폰 후 e키를 누르면 애니메이션 재생
    private void OpenManhole()
    {
        // 나중에 isMonsterSpawned 조건 추가
        if (isPlayerInManhole && isMonsterSpawned &&
            Input.GetKeyDown(KeyCode.E) && !isManholeOpen)
        {
            isManholeOpen = true;

            anim.Play();
            audioSource.Play();
        }
    }
}
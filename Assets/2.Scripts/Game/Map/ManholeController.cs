using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ManholeController : MonoBehaviour
{
    [SerializeField] private Animation anim;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private ManholeButtomController bc;
    [SerializeField] private GameObject monster;

    [SerializeField]private bool isPlayerInManhole = false;
    public bool isManholeOpen = false;
    public bool isPlayerInsideManhole = false;
    [SerializeField]private bool isMonsterSpawned = false;
    [SerializeField]private bool isSpawn = false;

    void Start()
    {
        monster = GameObject.FindWithTag("Monster");
    }

    void Update()
    {
        if (monster == null)
        {
            monster = GameObject.FindWithTag("Monster");
            return; // 아직 monster가 없으면 이하 생략
        }

        if (monster.GetComponentInChildren<Monster>() != null)
        {
            isMonsterSpawned = monster.GetComponentInChildren<Monster>().isMonsterSpawned;
        }

        if (isMonsterSpawned)
        {
            isSpawn = true;
        }

        if (isPlayerInManhole && isSpawn && !isManholeOpen && Input.GetKeyDown(KeyCode.E))
        {
            OpenManhole();
        }

        isPlayerInsideManhole = bc.isPlayerInsideManhole;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInManhole = true;

            MessageManager.Instance.Message("[E]키: 맨홀 열기");
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInManhole = true;

            if (isManholeOpen && isPlayerInsideManhole)
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

    private void OpenManhole()
    {
        isManholeOpen = true;

        anim.Play();
        audioSource.Play();
    }
    
    public bool GetIsPlayerInManhole()
    {
        return isPlayerInManhole;
    }
}
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

    private bool isPlayerInManhole = false;
    public bool isManholeOpen = false;
    public bool isPlayerInsideManhole = false;
    private bool isMonsterSpawned = false;

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

        if (isPlayerInManhole && isMonsterSpawned && !isManholeOpen && Input.GetKeyDown(KeyCode.E))
        {
            OpenManhole();
        }

        isPlayerInsideManhole = bc.isPlayerInsideManhole;
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
}
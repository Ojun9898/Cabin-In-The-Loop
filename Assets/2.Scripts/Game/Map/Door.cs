using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class Door : MonoBehaviour
{
    [SerializeField] private Animation anim;
    [SerializeField] private AudioSource audioSource;
    // 기몽 추가
    [SerializeField] private NavMeshLink doorLink;

    [HideInInspector] public bool isDoorOpen = false;
    
    private bool isPlayerInRange = false;
    
    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E) && !isDoorOpen)
        {
            anim.Play();
            audioSource.Play(); // 문 열리는 사운드 재생
            isDoorOpen = true;
            // 기몽 추가 _문 열림 → 링크 활성화
            doorLink.enabled = true;
            
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            isPlayerInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            isPlayerInRange = false;
    }
}
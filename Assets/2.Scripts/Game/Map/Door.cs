using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private Animation anim;
    [SerializeField] private AudioSource audioSource;

    [HideInInspector] public bool isDoorOpen = false;
    
    private bool isPlayerInRange = false;
    
    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E) && !isDoorOpen)
        {
            anim.Play();
            audioSource.Play(); // 문 열리는 사운드 재생
            isDoorOpen = true;
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
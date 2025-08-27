using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class Door : MonoBehaviour
{
    [SerializeField] private Animation anim;
    [SerializeField] private AudioSource audioSource;
    
    [HideInInspector] public bool isDoorOpen = false;
    
    public bool isPlayerOutCavin = false;
    
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

    public bool GetIsPlayerInCavin()
    {
        return isDoorOpen;
    }

    public bool GetIsPlayerOutCavin()
    {
        return isPlayerOutCavin;
    }
}
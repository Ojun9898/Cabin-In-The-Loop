using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AltarDoor : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] audioClips;
    [SerializeField] private Animation doorAnim;
    [SerializeField] private Altar altar;

    private bool isPlayerInRange = false;
    public bool isFogEliminated = false;
    private bool isDoorOpen = false;
    public bool isDoorClose = false;
    private bool hasFogStarted = false;

    void Update()
    {
        isFogEliminated = MapManager.Instance.isFogEliminated;

        if (isPlayerInRange && !isDoorOpen && !isFogEliminated && Input.GetKeyDown(KeyCode.E))
        {
            OpenDoor();
        }
        else if (altar.isPlayerInAltar && isDoorOpen && !isFogEliminated)
        {
            CloseDoor();
        }

        if (isDoorClose && !isFogEliminated && altar.isPlayerInAltar && !hasFogStarted)
        {
            hasFogStarted = true; // 반복 방지
            MapManager.Instance.StartFog();
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

    public void OpenDoor()
    {
        if (doorAnim.isPlaying) return; // 애니메이션 끊김 방지

        doorAnim.Play("Double Wood Door Open");
        audioSource.PlayOneShot(audioClips[0]); // ElevatorClose 사운드 재생

        isDoorOpen = true;
    }

    public void CloseDoor()
    {
        if (doorAnim.isPlaying) return; // 애니메이션 끊김 방지

        doorAnim.Play("Double Wood Door Close");
        audioSource.PlayOneShot(audioClips[1]); // ElevatorClose 사운드 재생

        isDoorOpen = false;
        isDoorClose = true;
    }
}
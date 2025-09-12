using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorOutDoorController : MonoBehaviour
{
    [SerializeField] private int currentFloor;
    [SerializeField] private Animation thisFloorOutDoorAnim;
    [SerializeField] private Animation nextFloorOutDoorAnim;
    [SerializeField] private ElevatorController ec;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] audioClips;
<<<<<<< HEAD
<<<<<<< HEAD:Assets/2.Scripts/Game/Map/ElevatorOutdoorController.cs
    // (기몽) 하이어라키에 있는 SpawnManager를 에디터에서 드래그해서 할당
     [SerializeField] private SpawnManager spawnManager;
=======
>>>>>>> 1f949ed ([추가] 병합 및 씬 분리):Assets/2.Scripts/Game/ElevatorOutdoorController.cs
=======
>>>>>>> 4301af75291249a954534f393c172d24ac73c9c8

    [HideInInspector] public bool isDoorOpen = false;

    public void OpenDoor()
    {
        if (thisFloorOutDoorAnim.isPlaying) return; // 애니메이션 끊김 방지
        
        if (!ec.isArrived)
        {
            audioSource.PlayOneShot(audioClips[1]); // ElevatorDoorOpen 사운드 재생
            thisFloorOutDoorAnim.Play("OutDoorOpen");
        }

        else
        {
            audioSource.PlayOneShot(audioClips[0]); // ElevatorBell 사운드 재생
            nextFloorOutDoorAnim.Play("OutDoorOpen");
        }

        ec.OpenDoor(); // OutDoor 문과 엘레베이터 문을 동시에 열기
        isDoorOpen = true;
    }

    public void CloseDoor()
    {
        if (thisFloorOutDoorAnim.isPlaying) return; // 애니메이션 끊김 방지
        
        if (!ec.isArrived)
        {
            thisFloorOutDoorAnim.Play("OutDoorClose");
            audioSource.PlayOneShot(audioClips[2]); // ElevatorClose 사운드 재생
        }

        else
        {
            nextFloorOutDoorAnim.Play("OutDoorClose");
            audioSource.PlayOneShot(audioClips[2]); // ElevatorClose 사운드 재생
        }
        
        ec.CloseDoor(); // OutDoor 문과 엘레베이터 문을 동시에 닫기
        isDoorOpen = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (thisFloorOutDoorAnim.isPlaying) return; // 애니메이션 끊김 방지

        // Message: "엘레베이터를 사용하겠습니까? [E]"

        if (other.CompareTag("Player") && !ec.isElevatorMoving && !ec.isArrived)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (currentFloor == 6)
                {
                    audioSource.PlayOneShot(audioClips[3]); // ElevatorError 사운드 재생
                    
                    // Message: "현재 층에서는 엘레베이터를 사용할 수 없습니다."
                    
                    return;
                }
                // (기몽) 스테이지 클리어 하지 않은 경우
                 /*if (spawnManager.GetAliveMonsterCount() > 0) 
                 {
                     // Message: "몬스터를 모두 처치하세요." 
                     Debug.Log("몬스터를 모두 처치하세요.");
                     return;
                 }*/

                if (!isDoorOpen)
                {
                    OpenDoor();
                }
            }
            
            if (ec.isPlayerInElevator && !ec.isArrived && !ec.isElevatorMoving)
            {
                // Message: "다음 층으로 올라가겠습니까? [Y]"
                if (Input.GetKeyDown(KeyCode.Y))
                {
                    audioSource.PlayOneShot(audioClips[4]); // ElevatorBtn 사운드 재생
                    StartCoroutine(MoveElevator());
                }
            }
        }
    }

    private IEnumerator MoveElevator()
    {
        CloseDoor();
        
        // Message: 이동 중...
        yield return new WaitForSeconds(1);

        yield return StartCoroutine(ec.MoveElevator(currentFloor)); // 기다려야 함

        yield return StartCoroutine(ArriveElevator());
    }

    private IEnumerator ArriveElevator()
    {
        if (ec.isArrived)
        {
            audioSource.PlayOneShot(audioClips[1]); // ElevatorBell 사운드 재생
            
            // 메시지: 도착했습니다. 문이 열립니다.
            
            if (!isDoorOpen)
                OpenDoor();

            // 문이 완전히 열릴 때까지 대기
            yield return new WaitUntil(() => isDoorOpen);

            // 플레이어가 엘리베이터를 나갈 때까지 대기
            yield return new WaitUntil(() => !ec.isPlayerInElevator);

            // 문 닫기
            CloseDoor();

            // 문이 완전히 닫힐 때까지 대기
            yield return new WaitUntil(() => !isDoorOpen);

            // 도착 상태 초기화
            ec.isArrived = false;
        }
    }
}
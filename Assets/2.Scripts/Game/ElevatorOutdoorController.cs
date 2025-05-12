using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorOutDoorController : MonoBehaviour
{
    [SerializeField] private int currentFloor;
    [SerializeField] private Animation thisFloorOutDoorAnim;
    [SerializeField] private Animation nextFloorOutDoorAnim;
    [SerializeField] private ElevatorController ec;

    [HideInInspector] public bool isDoorOpen = false;

    public void OpenDoor()
    {
        if (!ec.isArrived)
        {
            thisFloorOutDoorAnim.Play("OutDoorOpen");
        }

        else
        {
            nextFloorOutDoorAnim.Play("OutDoorOpen");
        }

        ec.OpenDoor(); // OutDoor 문과 엘레베이터 문을 동시에 열기
        isDoorOpen = true;
    }

    public void CloseDoor()
    {
        if (!ec.isArrived)
        {
            thisFloorOutDoorAnim.Play("OutDoorClose");
        }

        else
        {
            nextFloorOutDoorAnim.Play("OutDoorClose");
        }
        
        ec.CloseDoor(); // OutDoor 문과 엘레베이터 문을 동시에 닫기
        isDoorOpen = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (thisFloorOutDoorAnim.isPlaying) return; // 애니메이션 끊김 방지

        // Message: "엘레베이터를 사용하겠습니까? [E]"

        if (other.CompareTag("Player") && !ec.isElevatorMoving)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (currentFloor == 6)
                {
                    // Message: "현재 층에서는 엘레베이터를 사용할 수 없습니다."
                    return;
                }

                // if () // 스테이지 클리어 하지 않은 경우
                // {
                //     // Message: "몬스터를 모두 처치하세요." 
                //     return;
                // }

                if (!isDoorOpen)
                {
                    OpenDoor();
                }
            }

            if (ec.isPlayerInElevator && !ec.isArrived)
            {
                // Message: "다음 층으로 올라가겠습니까? [Y]"
                if (Input.GetKeyDown(KeyCode.Y) && !ec.isElevatorMoving)
                {
                    CloseDoor();
                    StartCoroutine(MoveElevator());
                }
            }
        }
    }

    private IEnumerator MoveElevator()
    {
        CloseDoor(); // 애니메이션 시작
        yield return new WaitForSeconds(thisFloorOutDoorAnim["OutDoorClose"].length);

        yield return StartCoroutine(ec.MoveElevator(currentFloor)); // 기다려야 함

        yield return StartCoroutine(LeaveElevator()); // LeaveElevator도 코루틴이면
    }

    private IEnumerator LeaveElevator()
    {
        if (ec.isArrived)
        {
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
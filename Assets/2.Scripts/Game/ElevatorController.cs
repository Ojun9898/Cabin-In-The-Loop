using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorController : MonoBehaviour
{
    private int ElevatorFloor;
    private float elevatorPos;
    private float outDoorPos;
    private ElevatorOutdoorController eoc = null;
    private ElevatorOutdoorController[] allOutdoors;

    [HideInInspector] public bool isElevatorOpen = false; // Elevator 문 열림 여부
    [HideInInspector] public bool isOutDoorOpen = false; // Outdoor 문 열림 여부

    [HideInInspector] public int currentElevatorFloor; // 엘레베이터 위치
    [HideInInspector] public int currentOutDoorFloor; // 엘레베이터를 호출하려는 층
    [HideInInspector] public bool isElevatorMoving = false; // 엘레베이터가 움직이고 있는지 확인하는 변수
    [HideInInspector] public Animation ElevatorAnim;
    [HideInInspector] public Animation OutDoorAnim; // 엘레베이터 문 애니메이션

    void Start()
    {
        allOutdoors = FindObjectsOfType<ElevatorOutdoorController>(); // 씬에 있는 모든 outdoor 오브젝트 가져오기
        ElevatorAnim = GetComponent<Animation>(); // Elevator 애니메이션 가져오기
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) // E키를 눌렀을 때
        {
            CallElevator();
        }

        // 플레이어가 엘레베이터 
    }

    // <summary>
    // 엘레베이터 위치 설정
    // 1. 항상 다음 층으로 이동 ( 1층 -> 2층 O / 1층 -> 3층 X )
    // 2. 엘레베이터는 다른 층에 있을 경우 플레이어가 호출한 층으로 이동
    public void CallElevator()
    {
        foreach (var outdoor in allOutdoors) // 플레이어가 선택한 eoc 찾기
        {
            if (outdoor.GetIsPlayerInTrigger())
            {
                eoc = outdoor;
                break;
            }
        }

        if (eoc == null) return;

        OutDoorAnim = eoc.GetComponent<Animation>(); // OutDoor 애니메이션 가져오기

        Debug.Log("엘레베이터 호출");

        // 현재 OutDoorFloor 층 불러오기 (플레이어가 키를 누른 층)
        if (eoc.GetOutDoorFloor() == 0) // 조건에 맞지 않을 경우
        {
            return;
        }

        currentOutDoorFloor = eoc.GetOutDoorFloor();

        // 현재 엘레베이터 층 불러오기
        if (GetElevatorFloor() == -1) // null
        {
            return;
        }

        currentElevatorFloor = GetElevatorFloor();

        if (currentElevatorFloor != currentOutDoorFloor) // 엘레베이터가 다른 층에 있을 때
        {
            outDoorPos = eoc.GetOutDoorPos(); // 호출한 층의 위치를 가져옴
            MoveElevator(outDoorPos); // 엘레베이터 이동
        }

        // 도착 후 문 열림 - 애니메이션 재생

        ElevatorOpen(); // 엘레베이터 문 열림
        OutDoorOpen(); // Outdoor 문 열림

        isElevatorOpen = true;
        isOutDoorOpen = true;
    }

    // <summary>
    // 엘레베이터를 타겟 pos로 이동
    public void MoveElevator(float pos)
    {
        ElevatorClose(); // 엘레베이터 문 닫힘
        OutDoorClose(); // Outdoor 문 닫힘

        Vector3 targetPos = new Vector3(this.transform.position.x, pos, this.transform.position.z); // 엘레베이터 위치 설정

        isElevatorMoving = true; // 엘레베이터가 움직이고 있는 상태로 변경
        Debug.Log("엘레베이터 이동 시작");

        StartCoroutine(MoveElevatorCoroutine(targetPos, 2f)); // 엘레베이터 이동 코루틴 시작
    }

    // 자연스러운 움직임을 위한 코루틴
    private IEnumerator MoveElevatorCoroutine(Vector3 target, float duration)
    {
        Vector3 start = transform.position;
        float time = 0f;

        Debug.Log("엘레베이터 이동 중");

        while (time < duration)
        {
            transform.position = Vector3.Lerp(start, target, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        transform.position = target; // 마지막 위치 정확하게 설정

        Debug.Log("엘레베이터 이동 완료");

        isElevatorMoving = false; // 이동 완료 후 상태 변경
    }

    public int GetElevatorFloor()
    {
        elevatorPos = GetElevatorPos();

        foreach (ElevatorOutdoorController outdoor in allOutdoors)
        {
            if (Mathf.Approximately(elevatorPos, outdoor.GetOutDoorPos()))
            {
                ElevatorFloor = outdoor.GetOutDoorFloor();
                Debug.Log("ElevatorFloor: " + ElevatorFloor);
                return ElevatorFloor;
            }
        }

        return -1; // 못 찾았을 경우
    }

    public float GetElevatorPos()
    {
        elevatorPos = transform.position.y;
        Debug.Log("ElevatorPos: " + elevatorPos);
        return elevatorPos;
    }

    // Elevator, OutDoor 문 열고닫는 애니메이션 재생
    public void ElevatorOpen()
    {
        if (isElevatorOpen)
        {
            ElevatorAnim.Play();
        }

        isElevatorOpen = true;
    }

    public void ElevatorClose()
    {
        // ElevatorOpen 애니메이션 역재생
        if (!isElevatorOpen)
        {
            ElevatorAnim["ElevatorOpen"].time = ElevatorAnim["ElevatorOpen"].length;  // 끝에서 시작
            ElevatorAnim["ElevatorOpen"].speed = -1; // 역재생
            ElevatorAnim.Play();
        }
        isElevatorOpen = false;
    }

    public void OutDoorOpen()
    {
        if (isOutDoorOpen)
        {
            OutDoorAnim.Play();
        }

        isOutDoorOpen = true;
    }

    public void OutDoorClose()
    {
        // OutDoorOpen 애니메이션 역재생
        if (!isOutDoorOpen)
        {
            OutDoorAnim["OutDoorOpen"].time = OutDoorAnim["OutDoorOpen"].length;  // 끝에서 시작
            OutDoorAnim["OutDoorOpen"].speed = -1; // 역재생
            OutDoorAnim.Play();
        }

        isOutDoorOpen = false;
    }

    public void OnElevatorOpenEnd()
    {
        //  엘레베이터 내부 콜라이더에 플레이어가 인식되지 않으면 일정시간 이후 문을 닫음
        isElevatorOpen = false; // 문이 닫힘
        // 6층 제외 윗층으로 올라감
        if (currentOutDoorFloor == 6)
        {
            Debug.Log("6층에서는 엘레베이터를 사용할 수 없습니다.");
            return;
        }

        MoveElevatorToNextFloor();
    }

    public void OnElevatorCloseEnd()
    {

    }

    public void MoveElevatorToNextFloor()
    {

        // 다음 층 안내 메세지
        Debug.Log("다음 층으로 이동하려면 E키를 눌러주세요.");

        if (Input.GetKeyDown(KeyCode.E)) // E키를 눌렀을 때
        {
            Debug.Log("엘레베이터 이동 시작");

            ElevatorClose(); // 엘레베이터 문 닫힘
            OutDoorClose(); // Outdoor 문 닫힘

            outDoorPos = eoc.GetOutDoorPos(); // 호출한 층의 위치를 가져옴
        }

        MoveElevator(outDoorPos + 6); // 엘레베이터를 다음 층으로 이동

        Debug.Log("현재 " + currentElevatorFloor + "층 입니다");

        ElevatorOpen(); // 엘레베이터 문 열림
        OutDoorOpen(); // Outdoor 문 열림
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && isElevatorOpen == true)
        {
            MoveElevatorToNextFloor(); // 엘레베이터에 탑승한 경우
        }
    }
}


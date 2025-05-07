using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class ElevatorOutdoorController : MonoBehaviour
{
    [SerializeField] private int outDoorFloor; // outdoor의 층 입력

    private ElevatorController ec;
    private float outDoorFloorPos; // outdoor의 위치
    private bool isPlayerInTrigger = false; // 플레이어가 트리거에 들어왔는지 확인하는 변수
    private bool isDoorOpen = false; // 문이 열려있는지 확인하는 변수

    void Update()
    {
        GetOutDoorFloor();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // 플레이어가 트리거에 들어왔을 때
        {
            isPlayerInTrigger = true;
            Debug.Log("Player is in outdoor.");
        }
    }

    public int GetOutDoorFloor()
    {
        ec = FindObjectOfType<ElevatorController>();

        if (ec.isElevatorMoving == false && isDoorOpen == false) // outdoor트리거에 플레이어가 들어왔고 E키를 눌렀을 때 / 엘레베이터가 움직이지 않고 문이 닫혀있을 때
        {
            return outDoorFloor;
        }
        
        return 0;
    }

    public float GetOutDoorPos()
    {    
        outDoorFloorPos = transform.position.y; // 현재 오브젝트의 y좌표
        return outDoorFloorPos;
    }

    public bool GetIsPlayerInTrigger()
    {
        return isPlayerInTrigger;
    }

    public void OnOutDoorOpenEnd()
    {
        // 엘레베이터 내부 콜라이더에 플레이어가 인식되지 않으면 일정시간 이후 문을 닫음
        isDoorOpen = false; // 문이 닫힘

        // 6층 제외 윗층으로 올라감
        if (outDoorFloor == 6)
        {
            Debug.Log("6층에서는 엘레베이터를 사용할 수 없습니다.");
            return;
        }

        ec.MoveElevatorToNextFloor();
    }

    public void OnOutDoorCloseEnd()
    {

    }
}

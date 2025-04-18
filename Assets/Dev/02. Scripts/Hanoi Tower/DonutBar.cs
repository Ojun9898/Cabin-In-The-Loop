using System.Collections.Generic;
using System.Linq;
using Hanoi_Tower;
using UnityEngine;

public class DonutBar : MonoBehaviour
{
    public enum BarType { LEFT = -3, CENTER = 0, RIGHT = 3 }
    public BarType e_BarType;
    
    public GameManager gameManager;

    public Stack<GameObject> stack = new Stack<GameObject>();


    // void OnMouseDown()
    // {
    //     if (!gameManager.isSelected) // 도넛을 가져올 기둥을 선택
    //     {
    //         gameManager.selectedDonut = PopDonut();
    //     }
    //     else if (gameManager.isSelected) // 도넛을 놓을 기둥을 선택
    //     {
    //         PushDonut(gameManager.selectedDonut);
    //     }
    // }
    //
    // private bool CheckDonutNumber(GameObject pushDonut)
    // {
    //     bool result = true; // 처음에 도넛을 Push해야하기 때문에 디폴트 값을 true 설정
    //     if (stack.Count > 0)
    //     {
    //         int pushNumber = pushDonut.GetComponent<Donut>().donutNumber;
    //         int peekNumber = stack.Peek().GetComponent<Donut>().donutNumber;
    //
    //         result = pushNumber < peekNumber; // 도넛이 하노이 로직에 맞는지 확인
    //
    //         if (!result)
    //             Debug.Log($"놓으려는 도넛은 {pushNumber}이고, 해당 기둥의 도넛은 {peekNumber}입니다.");
    //     }
    //
    //     return result;
    // }
    //
    // public void PushDonut(GameObject pushDonut)
    // {
    //     if (!CheckDonutNumber(pushDonut)) // 도넛 넘버가 하노이 로직에 맞지 않으면 return으로 기능 종료
    //         return;
    //
    //     gameManager.isSelected = false;
    //     gameManager.selectedDonut = null;
    //
    //     stack.Push(pushDonut);
    //     pushDonut.transform.SetPositionAndRotation(new Vector3((int)e_BarType, 3.5f, 0f), Quaternion.identity);
    //
    //     switch (e_BarType)
    //     {
    //         case BarType.LEFT:
    //             gameManager.leftBar = stack.ToList();
    //             break;
    //         case BarType.CENTER:
    //             gameManager.centerBar = stack.ToList();
    //             break;
    //         case BarType.RIGHT:
    //             gameManager.rightBar = stack.ToList();
    //             break;
    //     }
    // }
    //
    // public GameObject PopDonut()
    // {
    //     GameObject obj = null;
    //     if (stack.Count > 0)
    //     {
    //         obj = stack.Pop();
    //         gameManager.isSelected = true;
    //         
    //         switch (e_BarType)
    //         {
    //             case BarType.LEFT:
    //                 gameManager.leftBar = stack.ToList();
    //                 break;
    //             case BarType.CENTER:
    //                 gameManager.centerBar = stack.ToList();
    //                 break;
    //             case BarType.RIGHT:
    //                 gameManager.rightBar = stack.ToList();
    //                 break;
    //         }
    //     }
    //
    //     return obj;
    // }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanoi_Tower
{
    public class GameManager : MonoBehaviour
    {
        // public enum HanoiLevel { LV1 = 3, LV2, LV3 }
        // public HanoiLevel e_HanoiLevel;
        //
        // // 원반 생성
        // public GameObject donutPrefab;
        //
        // public DonutBar[] donutBars;
        //
        // public List<GameObject> leftBar = new List<GameObject>();
        // public List<GameObject> centerBar = new List<GameObject>();
        // public List<GameObject> rightBar = new List<GameObject>();
        //
        // public GameObject selectedDonut; 
        //
        // public bool isSelected = false;
        //
        // IEnumerator Start()
        // {
        //     for (int i = (int)e_HanoiLevel; i >= 1; i--)
        //     {
        //         donutBars[0].PushDonut(CreateDonut(donutPrefab, i));
        //
        //         yield return new WaitForSeconds(1f); // 1초 대기
        //     }
        // }
        //
        // private GameObject CreateDonut(GameObject prefab, int i)
        // {
        //     GameObject obj = Instantiate(prefab); // 도넛 생성
        //     obj.transform.SetPositionAndRotation(new Vector3((int)DonutBar.BarType.LEFT, 3.5f, 0f), Quaternion.identity); // 도넛 위치, 회전 설정
        //     
        //     obj.name = "Donut_" + i; // 도넛 이름 설정
        //     obj.GetComponent<Donut>().donutNumber = i; // 생성한 도넛의 넘버링 적용
        //     obj.transform.localScale = Vector3.one * (1f + i * 0.3f); // 도넛 크기 설정
        //
        //     return obj;
        // }
        //
        // public void OnShowAnswer()
        // {
        //     ShowAnswer((int)e_HanoiLevel, 0, 1, 2);
        // }
        //
        // private void ShowAnswer(int count, int from, int temp, int to)
        // {
        //     if (count == 0) return;
        //
        //     if (count == 1)
        //         Debug.Log($"{count}번 도넛을 {from}에서 {to}로 이동");
        //     else
        //     {
        //         ShowAnswer(count - 1, from, to, temp);
        //         
        //         Debug.Log($"{count}번 도넛을 {from}에서 {to}로 이동");
        //         
        //         ShowAnswer(count - 1, temp, from, to);
        //     }
        // }
    }
}
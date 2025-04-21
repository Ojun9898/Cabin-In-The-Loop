using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class DataClass
{
    public int GetData()
    {
        return 0;
    }
}

public class LottoCreator : MonoBehaviour
{
    public int[] numbers = new int[45]; // 45자리의 숫자 배열

    public LottoBall[] lottoBalls; // 7개의 로또볼
    
    private int shakeCount = 1000; // 섞는 횟수

    void Start()
    {
        for (int i = 0; i < 45; i++) // 섞을 Number 셋팅
            numbers[i] = i + 1;

        // OnCreateLotto();
    }
    
    public void OnCreateLotto()
    {
        for (int i = 0; i < shakeCount; i++) // 셔플 기능
        {
            var ranInt1 = Random.Range(0, 45);
            var ranInt2 = Random.Range(0, 45);
            // int temp = 0;
            
            var temp = numbers[ranInt1];
            numbers[ranInt1] = numbers[ranInt2];
            numbers[ranInt2] = temp;
        }
        
        // 오름차순 정렬
        int[] sortArray = new int[7];
        for (int i = 0; i < 7; i++)
            sortArray[i] = numbers[i];
        
        Array.Sort(sortArray);

        for (int i = 0; i < 7; i++) // 섞은 숫자 배열의 앞 7개를 로또볼에 적용
            lottoBalls[i].textNumber.text = sortArray[i].ToString();

        StartCoroutine(ShowBall());
    }

    IEnumerator ShowBall()
    {
        foreach (LottoBall ball in lottoBalls) // 순차적으로 보여주는 기능
        {
            ball.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.5f);
        }
    }
}
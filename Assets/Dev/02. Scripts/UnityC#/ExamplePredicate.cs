using System;
using UnityEngine;

public class ExamplePredicate : MonoBehaviour
{
    public int level = 10;

    public Predicate<int> myPredicate;

    void Start()
    {
        myPredicate = n => n <= 10;

        // [삼항 연산자] 결과 = 조건 ? 참일 경우 : 거짓일 경우
        string msg = myPredicate(level) ? "초보자 사냥터 이용 가능" : "초보자 사냥터 이용 불가능";
        
        
        
        Debug.Log(msg);
    }
    
    public void LevelCheck(int level)
    {
        if (level <= 10)
        {
            Debug.Log("초보자 사냥터 이용 가능");
        }
        else if (level > 10)
        {
            Debug.Log("초보자 사냥터 이용 불가능");
        }
    }
}
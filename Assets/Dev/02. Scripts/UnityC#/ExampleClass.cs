using UnityEngine;

public class ExampleClass : MonoBehaviour
{
    void Start()
    {
        // ExampleDelegate.myDelegate += OtherEvent;
    }
    
    public void OtherEvent()
    {
        // 키가 눌렸을 때 추가로 실행될 기능
        Debug.Log("Other Event");
    }
}
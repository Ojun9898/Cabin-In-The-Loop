using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleCallback : MonoBehaviour
{
    private Animator anim;

    public Action swingStart;
    public Action swingEnd;
    
    void Start()
    {
        swingStart += SwingStart;
        swingEnd += SwingEnd;
        
        anim = this.GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(SwingRoutine(swingStart, swingEnd));
        }
    }

    IEnumerator SwingRoutine(Action startAction, Action endAction)
    {
        startAction?.Invoke();
        anim.SetTrigger("Swing");

        float animTime = anim.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animTime);
        
        endAction?.Invoke();
    }

    public void SwingStart()
    {
        Debug.Log("스윙을 하기 전 이벤트");
    }

    public void SwingEnd()
    {
        Debug.Log("스윙을 한 후의 이벤트");
    }
}
using System;
using UnityEngine;

public class ExampleTimer : MonoBehaviour
{
    public delegate void GameStart();
    public static GameStart onGameStart;
    
    public delegate void GameOver();
    public static GameOver onGameOver;

    public float timer = 5f;
    private bool isTimer = true;

    void OnEnable()
    {
        onGameStart += StartEvent;
        onGameOver += EndEvent;
    }

    void OnDisable()
    {
        onGameStart -= StartEvent;
        onGameOver -= EndEvent;
    }

    void Start()
    {
        onGameStart?.Invoke();
    }

    void Update()
    {
        if (!isTimer) return;

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            isTimer = false;
            onGameOver?.Invoke();
        }
    }

    private void StartEvent()
    {
        Debug.Log("타이머 게임 시작");
    }

    private void EndEvent()
    {
        Debug.Log("타이머 게임 종료");
    }
}
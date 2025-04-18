using System;
using UnityEngine;

public class ScoreManager : MonoBehaviour, IObserver
{
    public int score;
    
    void OnEnable()
    {
        Monster.onMonsterDead += OnNotify;
    }

    void OnDisable()
    {
        Monster.onMonsterDead += OnNotify;
    }

    public void OnNotify(Monster monster)
    {
        switch (monster)
        {
            case Goblin:
                score += 10;
                break;
            case HobGoblin:
                score += 20;
                break;
            case Troll:
                score += 30;
                break;
        }
    }
}
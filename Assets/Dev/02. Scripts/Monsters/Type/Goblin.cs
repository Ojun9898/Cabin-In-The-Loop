using UnityEngine;

public class Goblin : Monster
{
    protected override void Init()
    {
        base.Init();
        
        hp = 3f;
        speed = 3f;
    }
}
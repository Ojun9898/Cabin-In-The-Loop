using UnityEngine;

public class Troll : Monster
{
    protected override void Init()
    {
        base.Init();

        hp = 10f;
        speed = 1f;
    }
}
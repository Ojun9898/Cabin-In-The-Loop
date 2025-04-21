using UnityEngine;

public class HobGoblin : Monster
{
    protected override void Init()
    {
        base.Init();

        hp = 6f;
        speed = 2f;
    }
}
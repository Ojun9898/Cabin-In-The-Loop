using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// EState, EMonsterType 정의
public enum EMonsterType { Ripper, Vendigo /*, Type3, Type4, Type5 */ }

// Inspector 창에서 편집가능
[System.Serializable]
public class MonsterSFXProfile
{
    public EMonsterType monsterType;
    public AudioClip chaseClip;
    public AudioClip hitClip;
    public AudioClip deathClip;
    public AudioClip attackClip;
    /* public AudioClip 하울링Clip;*/

    // State 스크립트에 정의된 EState 를 그대로 사용
    public AudioClip GetClip(EState state)
    {
        switch (state)
        {
            case EState.Chase:  return chaseClip;
            case EState.Hit:    return hitClip;
            case EState.Death:  return deathClip;
            case EState.Attack: return attackClip;
            /* public AudioClip 하울링Clip;*/
            default:            return null;
        }
    }
}

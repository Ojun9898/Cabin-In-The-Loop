using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// EState, EMonsterType 정의
public enum EMonsterType { Ripper, Vendigo, Zombie, Insect, Beast }

// Inspector 창에서 편집가능
[System.Serializable]
public class MonsterSFXProfile
{
    public EMonsterType monsterType;
    public AudioClip chaseClip;
    public AudioClip hitClip;
    public AudioClip deathClip;
    public AudioClip attackClip;
    // 비스트 하울링
    public AudioClip rattackClip;

    // State 스크립트에 정의된 EState 를 그대로 사용
    public AudioClip GetClip(EState state)
    {
        switch (state)
        {
            case EState.Chase:  return chaseClip;
            case EState.Hit:    return hitClip;
            case EState.Death:  return deathClip;
            case EState.Attack: return attackClip;
            case EState.Rattack: return rattackClip;
            default:            return null;
        }
    }
}

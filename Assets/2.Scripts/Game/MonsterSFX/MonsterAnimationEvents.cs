using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAnimationEvents : MonoBehaviour
{
    // 몬스터 타입 지정
    [SerializeField] private EMonsterType monsterType;

    // 애니메이션 시작시 호출
    public void OnAttackStart()
    {
        MonsterSFXManager.Instance.RequestPlay(
            EState.Attack,
            monsterType,
            transform
        );
    }
    
    // 애니메이션 종료시점시에 호출
    public void OnAttackEnd()
    {
        // 해당 몬스터의 모든 재생 중인 SFX를 중단
        MonsterSFXManager.Instance.StopAudio(transform.GetInstanceID());
    }
    
    public void OnHitStart()
    {
        MonsterSFXManager.Instance.RequestPlay(
            EState.Hit,
            monsterType,
            transform
        );
    }

    public void OnHitEnd()
    {
        MonsterSFXManager.Instance.StopAudio(transform.GetInstanceID());
    }

    public void OnDeathStart()
    {
        MonsterSFXManager.Instance.RequestPlay(
            EState.Death,
            monsterType,
            transform
        );
    }
    
    public void OnDeathEnd()
    {
        // 해당 몬스터의 모든 재생 중인 SFX를 중단
        MonsterSFXManager.Instance.StopAudio(transform.GetInstanceID());
    }

    public void OnRAttackStart()
    {
        MonsterSFXManager.Instance.RequestPlay(
            EState.Rattack,
            monsterType,
            transform
        );
    }
    
    public void OnRAttackEnd()
    {
        // 해당 몬스터의 모든 재생 중인 SFX를 중단
        MonsterSFXManager.Instance.StopAudio(transform.GetInstanceID());
    }
}

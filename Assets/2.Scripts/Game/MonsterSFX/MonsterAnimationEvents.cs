using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAnimationEvents : MonoBehaviour
{
    // 몬스터 타입 지정
    [SerializeField] private EMonsterType monsterType;

    // 애니메이션 시작(루프의 맨 앞) 시 호출
    public void OnAttackStart()
    {
        MonsterSFXManager.Instance.RequestPlay(
            EState.Attack,
            monsterType,
            transform
        );
    }
    public void OnAttackEnd()
    {
        // 해당 몬스터의 모든 재생 중인 SFX를 중단
        MonsterSFXManager.Instance.StopAllAudio(transform.GetInstanceID());
    }
<<<<<<< HEAD
<<<<<<< HEAD
    
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
        MonsterSFXManager.Instance.StopAllAudio(transform.GetInstanceID());
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
        MonsterSFXManager.Instance.StopAllAudio(transform.GetInstanceID());
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
        MonsterSFXManager.Instance.StopAllAudio(transform.GetInstanceID());
    }
=======
>>>>>>> 1f949ed ([추가] 병합 및 씬 분리)
=======
>>>>>>> 4301af75291249a954534f393c172d24ac73c9c8
}

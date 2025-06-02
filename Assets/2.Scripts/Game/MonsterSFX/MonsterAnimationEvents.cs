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
}

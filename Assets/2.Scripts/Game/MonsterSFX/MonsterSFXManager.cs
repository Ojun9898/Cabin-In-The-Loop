using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MonsterSFXManager : MonoBehaviour
{
    public static MonsterSFXManager Instance { get; private set; }
    
    [Header("몬스터 타입별 SFX 프로필을 설정")]
    public List<MonsterSFXProfile> profiles;
    
    [Header("AudioSource Pool 설정")]
    [Tooltip("동시 재생할 AudioSource 최대 개수")]
    public int maxAudioSources = 30;
    public int maxSimultaneousPlaysPerClip = 3;
    
    // EMonsterType → MonsterSFXProfile 맵
    private Dictionary<EMonsterType, MonsterSFXProfile> monsterProfile
        = new Dictionary<EMonsterType, MonsterSFXProfile>();

    // 몬스터(instanceID) → 마지막으로 재생한 상태
    private Dictionary<int, EState> lastState = new Dictionary<int, EState>();

    // 풀링된 AudioSource 래퍼
    private class PooledSource
    {
        public AudioSource ms;
        public int monsterID;
        public EState state;
    }

    // 게임 시작시 쵀대개수 만큼 미리 풀을 생성해둠
    private List<PooledSource> audioPool;

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        // 싱글톤 세팅
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // monsterProfile 초기화
        foreach (var p in profiles)
            monsterProfile[p.monsterType] = p;

        // AudioSource 풀 생성
        audioPool = new List<PooledSource>(maxAudioSources);
        for (int i = 0; i < maxAudioSources; i++)
        {
            var go = new GameObject($"PooledAudioSource_{i}");
            go.transform.SetParent(transform);
            var ms = go.AddComponent<AudioSource>();
            ms.playOnAwake = false;
            // 완전 3D 사운드 (거리 기반 볼륨/팬 조절)
            ms.spatialBlend = 1f;
            // 더 자연스럽게 거리감을 줌
            ms.rolloffMode = AudioRolloffMode.Logarithmic;
            // pool에 새로운 AudioSource 등록
            audioPool.Add(new PooledSource {ms = ms, monsterID = -1, state = default});
            
        }
    }

    // 풀에서 놀고있는(비어있는) 슬롯을 꺼내기.
    private PooledSource GetFreeSource()
    {
        foreach (var entry in audioPool)
            if (!entry.ms.isPlaying)
                return entry;
        
        return null;
    }
    
    // 상태 변경시 이전 상태의 사운드를 즉시 중단
    public void StopAllAudio(int monsterId)
    {
        foreach (var entry in audioPool)
        {
            if (entry.monsterID == monsterId && entry.ms.isPlaying)
                entry.ms.Stop();
        }
    }
    
    public void StopAllSounds()
    {
        // 1) 모든 AudioSource 중단
        foreach (var entry in audioPool)
        {
            if (entry.ms.isPlaying)
                entry.ms.Stop();
            // 2) monsterID를 초기화해서 재사용 가능 상태로
            entry.monsterID = -1;
        }

        // 3) 상태 캐시도 클리어해서, 다음에 같은 몬스터가 소환돼도 
        lastState.Clear();
    }

    
    // 상태 진입 시 호출
    public void RequestPlay(EState state, EMonsterType type, Transform monsterTransform)
    {
        int id = monsterTransform.GetInstanceID();

        // 이전에 등록된 id 가 없거나, 아니면 새로운 state가 이전 prev 상태랑 다르다면
        if (!lastState.TryGetValue(id, out var prev) || prev != state)
        {
            StopAllAudio(id);
            lastState[id] = state;
        }

        // 프로필에서 클립들 가져오기
        if (!monsterProfile.TryGetValue(type, out var prof))
            return;
        
        // 가져온 클립들중에서 현재 상태와 맞는 클립만 골라서 할당
         AudioClip clip = prof.GetClip(state);
        if (clip == null)
            return;
        
        // 동시 재생 제한 체크
        int playingCount = 0;
        foreach (var entry in audioPool)
            if (entry.ms.isPlaying && entry.ms.clip == clip)
                playingCount++;
        if (playingCount >= maxSimultaneousPlaysPerClip)
            return;

        // 풀에서 AudioSource 꺼내 재생
        var freeEntry = GetFreeSource();
        if (freeEntry != null)
        {
            freeEntry.monsterID = id;
            freeEntry.state   = state;
            freeEntry.ms.transform.position = monsterTransform.position;
            freeEntry.ms.clip   = clip;
            // 최대 볼륨
            freeEntry.ms.volume = 1f;
            freeEntry.ms.Play();
        }
    }
}


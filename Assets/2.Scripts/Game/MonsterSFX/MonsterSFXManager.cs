using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


public class MonsterSFXManager : MonoBehaviour
{
    public static MonsterSFXManager Instance { get; private set; }
    
    [Header("몬스터 타입별 SFX 프로필을 설정")]
    public List<MonsterSFXProfile> profiles;
    
    [Header("AudioSource Pool 설정")]
    public int maxAudioSources = 40;
    // 같은 타입의 몬스터가, 같은 동작 오디오를 동시에 재생할수 있는 수
    public int maxPlaysPerClip = 4;
    
    // EMonsterType → MonsterSFXProfile 맵
    private Dictionary<EMonsterType, MonsterSFXProfile> monsterProfile
        = new Dictionary<EMonsterType, MonsterSFXProfile>();

    // 몬스터(instanceID) → 마지막으로 재생한 상태
    private Dictionary<int, EState> lastState = new Dictionary<int, EState>();

    // 풀링된 AudioSource 래퍼
    private class PooledSource
    {
        public AudioSource monsterSource;
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
            var monsterSFX = new GameObject($"MonsterSFX_{i}");
            // 새로 생성된 monsterSFX 오브젝트를 MonsterSFXManager 오브젝트의 아래에 생성하게 설정
            monsterSFX.transform.SetParent(transform);
            var ms = monsterSFX.AddComponent<AudioSource>();
            ms.playOnAwake = false;
            // 완전 3D 사운드 (거리 기반 볼륨/팬 조절)
            ms.spatialBlend = 1f;
            // 더 자연스럽게 거리감을 줌
            ms.rolloffMode = AudioRolloffMode.Logarithmic;
            // pool에 새로운 AudioSource 등록
            audioPool.Add(new PooledSource {monsterSource = ms, monsterID = -1, state = default});
            
        }
    }

    // 풀에서 놀고있는(비어있는) 슬롯을 꺼내기.
    private PooledSource GetFreeSource()
    {
        foreach (var entry in audioPool)
            if (!entry.monsterSource.isPlaying)
                return entry;
        
        // 전부 사용중이라면 null값 반환
        return null;
    }
    
    // 상태 변경시 이전 상태의 사운드를 즉시 중단
    public void StopAudio(int monsterId)
    {
        foreach (var entry in audioPool)
        {
            if (entry.monsterID == monsterId && entry.monsterSource.isPlaying)
                entry.monsterSource.Stop();
        }
    }
    
    public void StopAllSounds()
    {
        // 1) 모든 AudioSource 중단
        foreach (var entry in audioPool)
        {
            if (entry.monsterSource.isPlaying)
                entry.monsterSource.Stop();
            // 2) monsterID를 초기화해서 재사용 가능 상태로
            entry.monsterID = -1;
        }

        // 3) 상태 캐시도 클리어
        lastState.Clear();
    }

    
    // 상태 진입 시 호출
    public void RequestPlay(EState state, EMonsterType type, Transform monsterTransform)
    {
        // 몬스터 식별자 생성
        int id = monsterTransform.GetInstanceID();

        // 이전에 등록된 id 가 없거나, 아니면 새로운 state가 이전 prev 상태랑 다르다면
        if (!lastState.TryGetValue(id, out var prev) || prev != state)
        {
            // 그 AudioSource 종료
            StopAudio(id);
            // lastState 딕셔너리(“이 몬스터가 마지막으로 낸 상태 사운드")의 키로 사용
            lastState[id] = state;
        }

        // 프로필에서 클립들 가져오기
        if (!monsterProfile.TryGetValue(type, out var sfxProfile))
            return;
        
        // 가져온 클립들중에서 현재 상태와 맞는 클립만 골라서 할당
         AudioClip clip = sfxProfile.GetClip(state);
        if (clip == null)
            return;
        
        // 동시 재생 제한 체크
        int playingCount = 0;
        foreach (var sfx in audioPool)
            if (sfx.monsterSource.isPlaying && sfx.monsterSource.clip == clip)
                playingCount++;
        // 중복된 오디오가 4개 이상이라면 함수 종료
        if (playingCount >= maxPlaysPerClip)
            return;

        // 풀에서 AudioSource 꺼내 재생
        var freeEntry = GetFreeSource();
        if (freeEntry != null)
        {
            // 이곳에서 처음으로 오디오소스 컴포넌트에 id를 부여
            freeEntry.monsterID = id;
            freeEntry.state   = state;
            freeEntry.monsterSource.transform.position = monsterTransform.position;
            freeEntry.monsterSource.clip   = clip;
            // 최대 볼륨
            freeEntry.monsterSource.volume = 1f;
            freeEntry.monsterSource.Play();
        }
    }
}


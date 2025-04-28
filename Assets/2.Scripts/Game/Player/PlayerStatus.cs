using UnityEngine;
using System;
using System.Collections.Generic;

// 캐릭터 종류 정의
public enum CharacterType
{
    Male,
    Female
}

// 스테이터스 타입 정의 (Defense 제거)
public enum StatusType
{
    Health,
    Attack,
    Speed
}

[RequireComponent(typeof(PlayerStateMachine))]
public class PlayerStatus : Singleton<PlayerStatus>, IDamageable
{
    [Header("Character Selection")]
    [Tooltip("선택된 캐릭터 타입에 따라 기본 스탯과 기본 무기가 달라집니다.")]
    [SerializeField] private CharacterType characterType;

    [Header("Default Weapons by Character")]
    [Tooltip("남성 캐릭터 기본 무기 타입")]
    [SerializeField] private WeaponType defaultMaleWeapon;
    [Tooltip("여성 캐릭터 기본 무기 타입")]
    [SerializeField] private WeaponType defaultFemaleWeapon;

    // 기본 스탯과 버프 스탯
    private Dictionary<StatusType, float> baseStats = new Dictionary<StatusType, float>();
    private Dictionary<StatusType, float> buffStats = new Dictionary<StatusType, float>();

    private float currentHealth;
    private PlayerStateMachine _psm;

    protected override void Awake()
    {
        base.Awake();                // Singleton 초기화
        DontDestroyOnLoad(gameObject);

        _psm = GetComponent<PlayerStateMachine>();
        InitializeStats();
        EquipDefaultWeapon();
    }

    private void Start()
    {
        // 전투 시작 시 현재 체력 초기화
        currentHealth = GetTotalStat(StatusType.Health);
    }

    // 스탯 초기화 및 저장된 값 로드
    private void InitializeStats()
    {
        // 버프 초기화
        foreach (StatusType st in Enum.GetValues(typeof(StatusType)))
            buffStats[st] = 0f;

        // 기본 스탯 설정 및 PlayerPrefs에서 로드
        foreach (StatusType st in Enum.GetValues(typeof(StatusType)))
        {
            string key = $"{characterType}_{st}_Base";
            float saved = PlayerPrefs.GetFloat(key, float.NaN);
            baseStats[st] = float.IsNaN(saved) ? GetDefaultValue(st) : saved;
        }
    }

    // 캐릭터별 기본 스탯 반환
    private float GetDefaultValue(StatusType type)
    {
        switch (characterType)
        {
            case CharacterType.Male:
                return type switch
                {
                    StatusType.Health  => 120f,
                    StatusType.Attack  => 15f,
                    StatusType.Speed   => 3f,
                    _ => 0f
                };
            case CharacterType.Female:
                return type switch
                {
                    StatusType.Health  => 100f,
                    StatusType.Attack  => 18f,
                    StatusType.Speed   => 3.5f,
                    _ => 0f
                };
            default: return 0f;
        }
    }

    // PlayerPrefs에 기본 스탯 저장
    public void SaveBaseStats()
    {
        foreach (var kv in baseStats)
        {
            string key = $"{characterType}_{kv.Key}_Base";
            PlayerPrefs.SetFloat(key, kv.Value);
        }
        PlayerPrefs.Save();
    }

    // 메인 메뉴 등에서 영구 업그레이드
    public void IncreaseBaseStat(StatusType type, float amount)
    {
        baseStats[type] += amount;
        SaveBaseStats();
    }

    // 인게임 버프 추가
    public void AddBuff(StatusType type, float amount)
    {
        buffStats[type] += amount;
        if (type == StatusType.Health)
            currentHealth += amount;
    }

    // 최종 스탯 계산 (Base + Buff)
    public float GetTotalStat(StatusType type)
    {
        return baseStats[type] + buffStats[type];
    }

    // 기본 무기 장착: WeaponController 활용
    private void EquipDefaultWeapon()
    {
        var weaponCtrl = GetComponent<WeaponController>();
        WeaponType defaultWeapon = characterType == CharacterType.Male
            ? defaultMaleWeapon
            : defaultFemaleWeapon;
        weaponCtrl.EquipWeapon(defaultWeapon);
    }

    // IDamageable 구현
    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        Debug.Log($"Player took {amount} dmg, remaining {currentHealth}");

        _psm.ChangeState(new PlayerHitState());

        if (currentHealth <= 0f)
            Die();
    }

    private void Die()
    {
        Debug.Log("Player Died");
        // 게임오버, 리스폰 처리
    }

    // 디버그: 모든 스탯 출력
    [ContextMenu("Print Stats")]
    private void PrintAllStats()
    {
        foreach (var st in baseStats.Keys)
        {
            Debug.Log($"{st} => Base: {baseStats[st]}, Buff: {buffStats[st]}, Total: {GetTotalStat(st)}");
        }
    }
}

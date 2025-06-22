using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Events;

// 캐릭터 종류 정의
public enum CharacterType
{
    Male,
    Female
}

// 스탯 타입 정의 
public enum StatusType
{
    Health,
    Attack,
    Speed,
    CritChance,      // 치명타 확률
    CritMultiplier   // 치명타 배율
}

[RequireComponent(typeof(PlayerStateMachine))]
public class PlayerStatus : Singleton<PlayerStatus>, IDamageable
{
    [Header("캐릭터 선택")]
    [SerializeField] private CharacterType characterType;

    [Header("기본 무기 설정")]
    [SerializeField] private WeaponType defaultMaleWeapon;
    [SerializeField] private WeaponType defaultFemaleWeapon;

    [Header("레벨 업 설정")]
    [SerializeField] private int startingLevel = 1;
    [SerializeField] private float startingXpToLevel = 100f;
    [SerializeField] private float xpGrowthFactor = 1.2f;

    // 레벨업 이벤트 (코드 구독용, 인스펙터용)
    public event Action<int> OnLevelUp;
    public UnityEvent<float> onLevelUpEvent;

    // 기본 스탯 / 버프 스탯 저장소
    private Dictionary<StatusType, float> _baseStats = new Dictionary<StatusType, float>();
    private Dictionary<StatusType, float> _buffStats = new Dictionary<StatusType, float>();

    // 레벨 & 경험치
    private int _currentLevel;
    private float _currentXp;
    private float _xpToNextLevel;

    // 체력
    private float _currentHealth;
    public event Action<float> onHealthChanged;
    
    private PlayerStateMachine _psm;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);

        _psm = GetComponent<PlayerStateMachine>();

        // 항상 1레벨, 0XP로 시작
        _currentLevel = startingLevel;
        _currentXp = 0f;
        SaveLevelData();

        InitializeStats();
        EquipDefaultWeapon();
    }

    private void Start()
    {
        // 씬 시작 시 체력 및 다음 레벨 XP 목표 계산
        _currentHealth = GetTotalStat(StatusType.Health);
        _xpToNextLevel = CalculateXpForLevel(_currentLevel);
    }

    #region 스탯 초기화 & 저장
    private void InitializeStats()
    {
        // 버프 스탯 초기화
        foreach (StatusType st in Enum.GetValues(typeof(StatusType)))
            _buffStats[st] = 0f;

        // 기본 스탯 로드 or 디폴트 값 설정
        foreach (StatusType st in Enum.GetValues(typeof(StatusType)))
        {
            string key = $"{characterType}_{st}_Base";
            float saved = PlayerPrefs.GetFloat(key, float.NaN);
            _baseStats[st] = float.IsNaN(saved) ? GetDefaultValue(st) : saved;
        }
    }

    private void SaveLevelData()
    {
        string keyLvl = $"{characterType}_Level";
        string keyXp  = $"{characterType}_Xp";
        PlayerPrefs.SetInt(keyLvl, _currentLevel);
        PlayerPrefs.SetFloat(keyXp, _currentXp);
        PlayerPrefs.Save();
    }
    #endregion

    #region 기본값 & 경험치 계산
    private float GetDefaultValue(StatusType type)
    {
        switch (characterType)
        {
            case CharacterType.Male:
                return type switch
                {
                    StatusType.Health       => 120f,
                    StatusType.Attack       => 15f,
                    StatusType.Speed        => 3f,
                    StatusType.CritChance   => 0.1f,   // 10% 기본
                    StatusType.CritMultiplier => 1.5f, // 150% 배율
                    _ => 0f
                };
            case CharacterType.Female:
                return type switch
                {
                    StatusType.Health       => 100f,
                    StatusType.Attack       => 18f,
                    StatusType.Speed        => 3.5f,
                    StatusType.CritChance   => 0.15f,  // 15% 기본
                    StatusType.CritMultiplier => 1.75f,// 175% 배율
                    _ => 0f
                };
            default:
                return 0f;
        }
    }

    private float CalculateXpForLevel(int level)
    {
        return startingXpToLevel * Mathf.Pow(xpGrowthFactor, level - startingLevel);
    }
    #endregion

    #region 퍼블릭 API
    public void IncreaseBaseStat(StatusType type, float amount)
    {
        _baseStats[type] += amount;
        PlayerPrefs.SetFloat($"{characterType}_{type}_Base", _baseStats[type]);
        PlayerPrefs.Save();
    }

    public void AddBuff(StatusType type, float amount)
    {
        _buffStats[type] += amount;
        if (type == StatusType.Health)
        {
            _currentHealth += amount;
            onHealthChanged?.Invoke(_currentHealth);
        }
    }

    public float GetTotalStat(StatusType type)
    {
        return _baseStats[type] + _buffStats[type];
    }

    /// <summary>
    /// 경험치 획득 메서드
    /// </summary>
    public void GainXp(float amount)
    {
        _currentXp += amount;
        while (_currentXp >= _xpToNextLevel)
        {
            _currentXp -= _xpToNextLevel;
            LevelUp();
        }
        SaveLevelData();
    }

    /// <summary>
    /// 데미지 계산 + 크리티컬 처리 헬퍼
    /// </summary>
    public float CalculateDamage(WeaponData data)
    {
        // 기본 데미지 = 무기 베이스 + 플레이어 공격력
        float baseDamage = data.damage + GetTotalStat(StatusType.Attack);

        // 크리티컬 확률과 배율 적용
        float critChance    = GetTotalStat(StatusType.CritChance);
        float critMultiplier= GetTotalStat(StatusType.CritMultiplier);
        bool isCrit = UnityEngine.Random.value < critChance;

        // 최종 데미지
        float finalDamage = isCrit
            ? baseDamage * critMultiplier
            : baseDamage;

        // 크리티컬 발생 이벤트 호출 (UI/사운드용)
        if (isCrit)
            onLevelUpEvent?.Invoke(finalDamage); // 필요 시 별도 이벤트로 분리하세요

        return finalDamage;
    }
    #endregion

    private void LevelUp()
    {
        _currentLevel++;
        _xpToNextLevel = CalculateXpForLevel(_currentLevel);

        OnLevelUp?.Invoke(_currentLevel);
        onLevelUpEvent?.Invoke(_currentLevel);
        
        // 레벨업 시 무기 선택 UI 띄우기
        GameManager.Instance.ShowWeaponSelection();
        
        Debug.Log($"Level Up! New Level: {_currentLevel}");
    }

    private void EquipDefaultWeapon()
    {
        var weaponCtrl = GetComponent<WeaponController>();
        WeaponType defaultWeapon = characterType == CharacterType.Male
            ? defaultMaleWeapon
            : defaultFemaleWeapon;
        weaponCtrl.EquipWeapon(defaultWeapon);
    }

    #region IDamageable 구현
    public void TakeDamage(float amount)
    {
        _currentHealth -= amount;
        onHealthChanged?.Invoke(_currentHealth);
        
        Debug.Log($"Player took {amount} dmg, remaining {_currentHealth}");
        
        _psm.ChangeState(new PlayerHitState());
        if (_currentHealth <= 0f) Die();
    }

    private void Die()
    {
        EndingManager.Instance.ShowDeadEnding();
    }
    #endregion

    #region 디버그용
    [ContextMenu("Print Stats")]
    private void PrintAllStats()
    {
        Debug.Log($"Level: {_currentLevel}, XP: {_currentXp}/{_xpToNextLevel}");
        foreach (var st in _baseStats.Keys)
            Debug.Log($"{st}: Base={_baseStats[st]}, Buff={_buffStats[st]}, Total={GetTotalStat(st)}");
    }
    #endregion
}

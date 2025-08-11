using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;

// 캐릭터 종류
public enum CharacterType
{
    Male,
    Female
}

// 스탯 타입
public enum StatusType
{
    Health,
    Attack,
    Speed,
    CritChance,
    CritMultiplier
}

// 저장할 데이터 구조
[Serializable]
public class PlayerData
{
    public CharacterType characterType;
    public int level;
    public float xp;
    public Dictionary<StatusType, float> baseStats = new Dictionary<StatusType, float>();
    public WeaponType currentWeaponType;
}

public class PlayerStatus : Singleton<PlayerStatus>, IDamageable
{
    [Header("캐릭터 선택")]
    public CharacterType characterType;

    [Header("레벨 업 설정")]
    [SerializeField] private int startingLevel = 1;
    [SerializeField] private float startingXpToLevel = 100f;
    [SerializeField] private float xpGrowthFactor = 1.2f;

    private PlayerData _data;
    private Dictionary<StatusType, float> _buffStats = new Dictionary<StatusType, float>();
    private float _currentHealth;
    private float _xpToNextLevel;

    private string SavePath => Path.Combine(Application.persistentDataPath, $"{characterType}_data.json");
    
    // 추가: 크리티컬 발생 시 호출하는 이벤트 (UI나 사운드용)
    public event Action<float> onCriticalHit;
    public event Action<float> onHealthChanged;
    
    public CharacterType CharacterType => _data.characterType;
    public float _maxHealth;
    public WeaponType CurrentWeaponType => _data.currentWeaponType;

    // 추가: 메인 씬에 없을 때 런타임 생성 보장
    public static PlayerStatus Ensure()
    {
        // 1) 씬에 이미 있나? (게터를 건드리지 않음)
        var existing = UnityEngine.Object.FindObjectOfType<PlayerStatus>();
        if (existing != null)
            return existing;

        // 2) 없으면 런타임 생성
        var go = new GameObject("PlayerStatus(Auto)");
        var ps = go.AddComponent<PlayerStatus>(); // AddComponent 시 Awake가 호출되어 Singleton 셋업됨
        UnityEngine.Object.DontDestroyOnLoad(go);
        return ps;
    }
    protected override void Awake()
    {
        base.Awake();

        LoadData();
        InitializeBuffs();

        _maxHealth = GetTotalStat(StatusType.Health);
        _currentHealth = _maxHealth;
        _xpToNextLevel = CalculateXpForLevel(_data.level);

        // 추가: UI/리스너에 현재 체력 알림
        onHealthChanged?.Invoke(_currentHealth);
    }

    private void InitializeBuffs()
    {
        foreach (StatusType st in Enum.GetValues(typeof(StatusType)))
        {
            if (!_buffStats.ContainsKey(st))
                _buffStats[st] = 0f;
        }
    }

    private void LoadData()
    {
        if (File.Exists(SavePath))
        {
            string json = File.ReadAllText(SavePath);
            _data = JsonUtility.FromJson<PlayerDataWrapper>(json).ToPlayerData();
        }
        else
        {
            _data = new PlayerData
            {
                characterType = characterType,
                level = startingLevel,
                xp = 0,
                baseStats = GetDefaultStats(characterType),
                currentWeaponType = WeaponType.Crowbar
            };
            SaveData();
        }
    }

    public void SaveData()
    {
        string json = JsonUtility.ToJson(new PlayerDataWrapper(_data), true);
        File.WriteAllText(SavePath, json);
    }
    
    public void SetCurrentWeaponType(WeaponType type)
    {
        _data.currentWeaponType = type;
        SaveData();
    }

    private Dictionary<StatusType, float> GetDefaultStats(CharacterType type)
    {
        var dict = new Dictionary<StatusType, float>();
        switch (type)
        {
            case CharacterType.Male:
                dict[StatusType.Health] = 120f;
                dict[StatusType.Attack] = 15f;
                dict[StatusType.Speed] = 3f;
                dict[StatusType.CritChance] = 0.1f;
                dict[StatusType.CritMultiplier] = 1.5f;
                break;
            case CharacterType.Female:
                dict[StatusType.Health] = 100f;
                dict[StatusType.Attack] = 18f;
                dict[StatusType.Speed] = 3.5f;
                dict[StatusType.CritChance] = 0.15f;
                dict[StatusType.CritMultiplier] = 1.75f;
                break;
        }
        return dict;
    }

    private float CalculateXpForLevel(int level)
    {
        return startingXpToLevel * Mathf.Pow(xpGrowthFactor, level - startingLevel);
    }

    public void IncreaseBaseStat(StatusType type, float amount)
    {
        if (!_data.baseStats.ContainsKey(type))
            _data.baseStats[type] = 0f;

        _data.baseStats[type] += amount;

        // Health를 올렸다면 최대/현재 체력도 함께 반영(선택)
        if (type == StatusType.Health)
        {
            _maxHealth = GetTotalStat(StatusType.Health);
            _currentHealth = Mathf.Min(_currentHealth, _maxHealth);
            onHealthChanged?.Invoke(_currentHealth);
        }

        SaveData();
    }

    public float GetTotalStat(StatusType type)
    {
        float baseV = 0f;
        _data.baseStats.TryGetValue(type, out baseV);

        float buffV = 0f;
        _buffStats.TryGetValue(type, out buffV); // 안전 접근

        return baseV + buffV;
    }

    public void GainXp(float amount)
    {
        _data.xp += amount;
        while (_data.xp >= _xpToNextLevel)
        {
            _data.xp -= _xpToNextLevel;
            _data.level++;
            _xpToNextLevel = CalculateXpForLevel(_data.level);
        }
        SaveData();
    }
    
    public void SetCharacter(CharacterType type)
    {
        if (_data.characterType != type)
        {
            characterType = type;
            _data.characterType = type;
            LoadData(); // 해당 캐릭터 데이터 로드
            
            _maxHealth = GetTotalStat(StatusType.Health);
            _currentHealth = Mathf.Clamp(_currentHealth, 0f, _maxHealth);
            _xpToNextLevel = CalculateXpForLevel(_data.level);
            onHealthChanged?.Invoke(_currentHealth);
        }
    }
    
    /// <summary>
    /// 데미지 계산 + 크리티컬 처리 헬퍼
    /// </summary>
    public float CalculateDamage(WeaponData weapon)
    {
        // 기본 데미지 = 무기 공격력 + 플레이어 공격력
        float baseDamage = weapon.damage + GetTotalStat(StatusType.Attack);

        // 크리티컬 확률과 배율 적용
        float critChance = GetTotalStat(StatusType.CritChance);
        float critMultiplier = GetTotalStat(StatusType.CritMultiplier);
        bool isCrit = UnityEngine.Random.value < critChance;

        // 최종 데미지 계산
        float finalDamage = isCrit ? baseDamage * critMultiplier : baseDamage;

        // 크리티컬 발생 이벤트 호출
        if (isCrit)
            onCriticalHit?.Invoke(finalDamage);

        return finalDamage;
    }

    // IDamageable 구현
    public void TakeDamage(float amount)
    {
        _currentHealth -= amount;
        if (_currentHealth < 0) _currentHealth = 0;

        // 체력 변경 이벤트 호출
        onHealthChanged?.Invoke(_currentHealth);

        if (_currentHealth <= 0)
        {
            Debug.Log("Player Dead");
        }
    }
}

// JsonUtility가 Dictionary를 지원하지 않으므로 변환용 Wrapper
[Serializable]
public class PlayerDataWrapper
{
    public CharacterType characterType;
    public int level;
    public float xp;
    public List<StatusEntry> baseStats = new List<StatusEntry>();
    public WeaponType currentWeaponType;

    [Serializable]
    public struct StatusEntry
    {
        public StatusType key;
        public float value;
    }

    public PlayerDataWrapper(PlayerData data)
    {
        characterType = data.characterType;
        level = data.level;
        xp = data.xp;
        currentWeaponType = data.currentWeaponType;
        
        foreach (var kv in data.baseStats)
        {
            baseStats.Add(new StatusEntry { key = kv.Key, value = kv.Value });
        }
    }

    public PlayerData ToPlayerData()
    {
        var pd = new PlayerData
        {
            characterType = characterType,
            level = level,
            xp = xp,
            baseStats = new Dictionary<StatusType, float>(),
            currentWeaponType = currentWeaponType
        };
        foreach (var entry in baseStats)
        {
            pd.baseStats[entry.key] = entry.value;
        }
        return pd;
    }
}
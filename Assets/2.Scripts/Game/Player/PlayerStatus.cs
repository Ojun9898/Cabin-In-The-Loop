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
    public CharacterType characterType = CharacterType.Female;

    [Header("레벨 업 설정")]
    [SerializeField] private int startingLevel = 1;
    [SerializeField] private float startingXpToLevel = 100f;
    [SerializeField] private float xpGrowthFactor = 1.2f;

    private PlayerData _data;
    private Dictionary<StatusType, float> _buffStats = new Dictionary<StatusType, float>();
    public float _currentHealth;
    public float _xpToNextLevel;

    public float CurrentHealth => _currentHealth;
    public float XpToNextLevel => _xpToNextLevel;
    public int Level => _data.level;
    public float CurrentXp => _data.xp;

    // ★ 현재 파일은 항상 Female로 고정 사용 (요청 반영)
    private string SavePath =>
        Path.Combine(Application.persistentDataPath, $"Female_data.json");

    // 이벤트
    public event Action<float> onCriticalHit;
    public event Action<float> onHealthChanged;

    public CharacterType CharacterType => _data.characterType;
    public float _maxHealth;
    public WeaponType CurrentWeaponType => _data.currentWeaponType;

    bool isDead = false;

    // 메인 씬에 없을 때 런타임 생성 보장
    public static PlayerStatus Ensure()
    {
        var existing = UnityEngine.Object.FindObjectOfType<PlayerStatus>();
        if (existing != null)
            return existing;

        var go = new GameObject("PlayerStatus(Auto)");
        var ps = go.AddComponent<PlayerStatus>();
        UnityEngine.Object.DontDestroyOnLoad(go);
        return ps;
    }

    protected override void Awake()
    {
        base.Awake();

        LoadData();
        InitializeBuffs();
        EnsureAllStatKeys(); // ★ 누락 키/버프 키 보정

        _maxHealth = GetTotalStat(StatusType.Health);
        _currentHealth = _maxHealth;
        _xpToNextLevel = CalculateXpForLevel(_data.level);

        // UI/리스너에 현재 체력 알림
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
            // 기본을 Female로 시작
            characterType = CharacterType.Female;

            _data = new PlayerData
            {
                characterType = characterType,
                level = startingLevel,
                xp = 0,
                baseStats = GetDefaultStats(characterType),
                currentWeaponType = default   // 필요 시 기본 무기 지정
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

    // ★ 스탯/버프 키 보정 (파일/버전 호환)
    private void EnsureAllStatKeys()
    {
        var defaults = GetDefaultStats(_data.characterType);
        foreach (StatusType st in Enum.GetValues(typeof(StatusType)))
        {
            if (!_data.baseStats.ContainsKey(st))
                _data.baseStats[st] = defaults[st];
            if (!_buffStats.ContainsKey(st))
                _buffStats[st] = 0f;
        }
    }

    public void IncreaseBaseStat(StatusType type, float amount)
    {
        if (!_data.baseStats.ContainsKey(type))
            _data.baseStats[type] = 0f;

        _data.baseStats[type] += amount;

        // Health를 올렸다면 최대/현재 체력도 함께 반영
        if (type == StatusType.Health)
        {
            _maxHealth = GetTotalStat(StatusType.Health);
            _currentHealth = Mathf.Min(_currentHealth, _maxHealth);
            onHealthChanged?.Invoke(_currentHealth);
        }

        SaveData();
    }

    // ★ 안전한 합계 스탯(키 없으면 0)
    public float GetTotalStat(StatusType type)
    {
        float baseV = 0f;
        if (_data.baseStats != null)
            _data.baseStats.TryGetValue(type, out baseV);

        float buffV = 0f;
        _buffStats.TryGetValue(type, out buffV);

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

    // ★ 새 게임 리셋: LV1 / XP0, HP 풀, HUD 갱신
    public void ResetProgressForNewGame()
    {
        _data.level = startingLevel;
        _data.xp = 0f;
        _xpToNextLevel = CalculateXpForLevel(_data.level);

        _maxHealth = GetTotalStat(StatusType.Health);
        _currentHealth = _maxHealth;
        onHealthChanged?.Invoke(_currentHealth);

        SaveData();
    }

    public void SetCharacter(CharacterType type)
    {
        if (_data.characterType != type)
        {
            characterType = type;
            _data.characterType = type;

            // 저장 경로는 Female 고정이지만, 내부 캐릭값 변경 후에도 기본 스탯 보정
            LoadData();            // 파일 재로드(Female_data.json)
            EnsureAllStatKeys();   // 키/버프 보정

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
        float atk = GetTotalStat(StatusType.Attack);
        float baseDamage = weapon.damage + atk;

        // 크리티컬 확률과 배율 적용 (안전 보정)
        float critChance = Mathf.Clamp01(GetTotalStat(StatusType.CritChance));
        float critMultiplier = Mathf.Max(1f, GetTotalStat(StatusType.CritMultiplier));
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

        onHealthChanged?.Invoke(_currentHealth);

        if (_currentHealth <= 0 && isDead == false)
        {
            isDead = true;
            EndingManager.Instance.ShowDeadEnding();
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
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HUDManager : Singleton<HUDManager>
{
    // 스탯 (hp, xp, level)

    [SerializeField] private float hpMax; // 최대 체력
    [SerializeField] private float xpMax; // 최대 XP
    
    private float xp;
    private int level;

    private RectTransform hpFillRect;
    private TextMeshProUGUI hpText;

    private RectTransform xpFillRect;
    private TextMeshProUGUI xpText;

    private TextMeshProUGUI levelText;

    private GameObject Player;
    private PlayerStatus ps; // 이벤트 구독

    // 스테이지
    
    private TextMeshProUGUI stageText;

    // 퀘스트
    
    private GameObject quest;
    private TextMeshProUGUI questText;


    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }
    void OnEnable()
    {
        StartCoroutine(InitHUD());
    }

    IEnumerator InitHUD()
    {
        yield return null; // 씬 로드 직후 한 프레임 기다림

        Player = GameObject.FindGameObjectWithTag("Player");

        if (Player == null)
        {
            Debug.LogWarning("HUDManager: Player not found.");
            yield break;
        }

        // 스탯 바인딩
        hpFillRect = transform.Find("Stats/HP/Fill")?.GetComponent<RectTransform>();
        hpText = transform.Find("Stats/HP/Text")?.GetComponent<TextMeshProUGUI>();

        xpFillRect = transform.Find("Stats/XP/Fill")?.GetComponent<RectTransform>();
        xpText = transform.Find("Stats/XP/Text")?.GetComponent<TextMeshProUGUI>();

        levelText = transform.Find("Stats/Level")?.GetComponent<TextMeshProUGUI>();

        stageText = transform.Find("Stage/Text")?.GetComponent<TextMeshProUGUI>();

        ps = Player.GetComponent<PlayerStatus>();

        if (ps != null)
        {
            ps.onHealthChanged += SetHPUI;

            hpMax = ps._maxHealth;
            xpMax = 100f;

            SetHPUI(ps._maxHealth);
        }
    }
    
    void Update()
    {
        // 스탯 (XP, Level) 갱신
        xp = PlayerPrefs.GetFloat("_currentXp");
        level = PlayerPrefs.GetInt("_currentLevel");

        SetXPUI(xp);
        SetLevelUI(level);
    }

    private void SetHPUI(float hp)
    {
        // HP 음수 방지
        if (hp <= 0)
            hp = 0;

        float percent = Mathf.Clamp01(hp / 100f);
        float rightValue = Mathf.Lerp(438f, 0f, percent);
        hpFillRect.offsetMax = new Vector2(-rightValue, hpFillRect.offsetMax.y);
        hpText.text = $"{Mathf.RoundToInt(hp)} / {hpMax}";
    }

    private void SetXPUI(float xp)
    {
        if (xpFillRect == null || xpText == null)
            return;
        
        // XP 음수 방지
        if (xp <= 0)
        {
            xpText.text = "0";
        }

        float percent = Mathf.Clamp01(xp / 100f);
        float rightValue = Mathf.Lerp(438f, 0f, percent);
        xpFillRect.offsetMax = new Vector2(-rightValue, xpFillRect.offsetMax.y);
        xpText.text = $"{Mathf.RoundToInt(xp)} / {xpMax}";
    }

    private void SetLevelUI(int level)
    {
        if (levelText == null)
            return;
        
        levelText.text = $"LV.{level}";
    }

    public void SetStageUI(int round)
    {
        string stage;

        if (round == 0)
        {
            stage = "오두막";
        }

        else if (round == 7)
        {
            stage = "복도";
        }

        else if (round == 8)
        {
            stage = "제단";
        }

        else
        {
            stage = "실험실 " + round + "층";
        }

        stageText.text = stage;
    }

    /*public void SetQuestUI(string questCode)
    {
        string takenQuest;

        //퀘스트 json에서 questText 가져오는 함수
        takenQuest = TakeQuestText(questCode);

        questText.text = takenQuest;
    }*/

    /*public string TakeQuestText(string questCode)
    {
        string takenQuest;

        takenQuest = questCode;

        return takenQuest;
    }*/
}
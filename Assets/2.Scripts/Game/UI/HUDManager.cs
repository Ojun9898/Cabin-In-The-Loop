using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HUDManager : Singleton<HUDManager>
{
    [System.Serializable]
    public class QuestData
    {
        public string Scene;
        public string QuestType;
        public string QuestText;
        public string MessageText;
    }

    [SerializeField] private GameObject questPrefab;
    [SerializeField] private Transform questParent;

    [Header("스탯 최대치")] [SerializeField] private float hpMax;
    [SerializeField] private float xpMax;

    private Dictionary<string, QuestData> questDict = new Dictionary<string, QuestData>();
    private Dictionary<string, GameObject> activeQuests = new Dictionary<string, GameObject>(); // 현재 화면에 있는 퀘스트 오브젝트 저장

    private RectTransform hpFillRect;
    private TextMeshProUGUI hpText;

    private RectTransform xpFillRect;
    private TextMeshProUGUI xpText;

    private TextMeshProUGUI levelText;
    private TextMeshProUGUI stageText;

    private GameObject Player;
    private PlayerStatus ps;

    private float xp;
    private int level;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
        LoadQuestData();

        // 씬 로드 이벤트 등록
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // 씬 로드 시 호출되는 함수
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // UI 오브젝트 다시 잡기
        if (questParent == null)
            questParent = GameObject.Find("Quests")?.transform;

        if (stageText == null)
            stageText = GameObject.Find("Stage/Text")?.GetComponent<TextMeshProUGUI>();

        // Player 참조 갱신
        Player = GameObject.FindGameObjectWithTag("Player");
        if (Player != null)
            ps = Player.GetComponent<PlayerStatus>();

        // HUD 요소 다시 세팅
        StartCoroutine(InitHUD());

        // 스테이지 이름 갱신
        switch (scene.buildIndex)
        {
            case 1:
                stageText.text = "오두막";
                break;
            case 2:
                stageText.text = "실험실 1층";
                break;
        }
    }

    void OnEnable()
    {
        StartCoroutine(InitHUD());

        QuestManager questManager = FindObjectOfType<QuestManager>();
        if (questManager != null)
        {
            questManager.OnQuestGotten += HandleQuestGotten;
            questManager.OnQuestCleared += HandleQuestCleared;
        }
    }

    void Update()
    {
        SetHPUI();
        SetXPUI();
        SetLevelUI();
    }

    IEnumerator InitHUD()
    {
        yield return null;

        Player = GameObject.FindGameObjectWithTag("Player");
        if (Player == null)
        {
            Debug.LogWarning("HUDManager: Player not found.");
            yield break;
        }

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

    void LoadQuestData()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "Quest.json");

        if (!File.Exists(path))
        {
            Debug.LogWarning("Quest.json 파일을 찾을 수 없습니다.");
            return;
        }

        string json = File.ReadAllText(path);

        var dict = MiniJSON.Json.Deserialize(json) as Dictionary<string, object>;
        questDict.Clear();

        foreach (var key in dict.Keys)
        {
            var value = dict[key] as Dictionary<string, object>;
            QuestData qd = new QuestData();
            qd.Scene = value["Scene"] as string;
            qd.QuestType = value["QuestType"] as string;
            qd.QuestText = value["QuestText"] as string;
            qd.MessageText = value["MessageText"] as string;

            questDict[key] = qd;
        }
    }

    #region 스탯 UI 업데이트

    private PlayerData LoadPlayerData()
    {
        if (ps == null)
            return null;

        string path = Path.Combine(Application.persistentDataPath, $"{ps.CharacterType}_data.json");
        if (!File.Exists(path))
        {
            Debug.LogWarning($"플레이어 데이터 파일을 찾을 수 없습니다: {path}");
            return null;
        }

        string json = File.ReadAllText(path);
        PlayerDataWrapper wrapper = JsonUtility.FromJson<PlayerDataWrapper>(json);
        return wrapper.ToPlayerData();
    }

    private void SetHPUI(float hp = -1f)
    {
        var data = LoadPlayerData();
        if (data == null || hpFillRect == null || hpText == null) return;

        float maxHp = data.baseStats.ContainsKey(StatusType.Health) ? data.baseStats[StatusType.Health] : 100f;
        float currentHp = (hp >= 0f) ? hp : (ps != null ? ps._currentHealth : maxHp);

        currentHp = Mathf.Max(0, currentHp);
        float percent = Mathf.Clamp01(currentHp / maxHp);
        float rightValue = Mathf.Lerp(438f, 0f, percent);

        hpFillRect.offsetMax = new Vector2(-rightValue, hpFillRect.offsetMax.y);
        hpText.text = $"{Mathf.RoundToInt(currentHp)} / {maxHp}";
    }

    private void SetXPUI(float xp = -1f)
    {
        var data = LoadPlayerData();
        if (data == null || xpFillRect == null || xpText == null) return;

        float xpToNext = ps != null ? ps._xpToNextLevel : 100f;
        float currentXp = (xp >= 0f) ? xp : data.xp;

        currentXp = Mathf.Max(0, currentXp);
        float percent = Mathf.Clamp01(currentXp / xpToNext);
        float rightValue = Mathf.Lerp(438f, 0f, percent);

        xpFillRect.offsetMax = new Vector2(-rightValue, xpFillRect.offsetMax.y);
        xpText.text = $"{Mathf.RoundToInt(currentXp)} / {Mathf.RoundToInt(xpToNext)}";
    }

    private void SetLevelUI(int level = -1)
    {
        var data = LoadPlayerData();
        if (data == null || levelText == null) return;

        int currentLevel = (level >= 0) ? level : data.level;
        levelText.text = $"LV.{currentLevel}";
    }

    public void SetStageUI(int round)
    {
        if (stageText == null)
            stageText = transform.Find("Stage/Text")?.GetComponent<TextMeshProUGUI>();

        string stage = round switch
        {
            0 => "오두막",
            7 => "복도",
            8 => "제단",
            _ => $"실험실 {round}층"
        };

        if (stageText != null)
            stageText.text = stage;
    }

    #endregion

    #region 퀘스트 UI 업데이트

    private void HandleQuestGotten(string questCode)
    {
        SetQuestUI(questCode);
    }

    private void HandleQuestCleared(string questCode)
    {
        SetQuestEndUI(questCode);
    }

    public void SetQuestUI(string questCode)
    {
        if (!questDict.TryGetValue(questCode, out QuestData qd))
        {
            Debug.LogWarning($"퀘스트 데이터를 찾을 수 없습니다: {questCode}");
            return;
        }
        
        if (questParent == null)
            questParent = transform.Find("Canvas/HUD/Quests");

        GameObject newQuest = Instantiate(questPrefab, questParent);
        newQuest.tag = "Quest";

        TextMeshProUGUI tmp = newQuest.GetComponentInChildren<TextMeshProUGUI>();
        if (tmp != null)
            tmp.text = qd.QuestText;

        activeQuests[questCode] = newQuest;

        // Fade In 연출 시작
        StartCoroutine(FadeInQuest(newQuest, 1f)); // 1초 동안 페이드 인
    }

    private IEnumerator FadeInQuest(GameObject obj, float duration)
    {
        if (obj == null) yield break;

        CanvasGroup cg = obj.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = obj.AddComponent<CanvasGroup>();

        cg.alpha = 0f;

        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            cg.alpha = Mathf.Lerp(0f, 1f, timer / duration);
            yield return null;
        }

        cg.alpha = 1f;
    }

    public void SetQuestEndUI(string questCode)
    {
        if (activeQuests.TryGetValue(questCode, out GameObject questObj) && questObj != null)
        {
            StartCoroutine(FadeAndDestroy(questCode, questObj, 1f)); // 1초 동안 페이드 아웃
        }
        else
        {
            Debug.LogWarning($"퀘스트 오브젝트를 찾을 수 없습니다: {questCode}");
        }
    }

    private IEnumerator FadeAndDestroy(string questCode, GameObject obj, float duration)
    {
        if (obj == null) yield break;

        CanvasGroup cg = obj.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = obj.AddComponent<CanvasGroup>();

        float startAlpha = cg.alpha;
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            cg.alpha = Mathf.Lerp(startAlpha, 0f, timer / duration);
            yield return null;
        }

        Destroy(obj);
        activeQuests.Remove(questCode);
    }


    public string TakeQuestText(string questCode, string type)
    {
        if (!questDict.TryGetValue(questCode, out QuestData qd))
            return null;

        return type switch
        {
            "Scene" => qd.Scene,
            "QuestType" => qd.QuestType,
            "QuestText" => qd.QuestText,
            "MessageText" => qd.MessageText,
            _ => null,
        };
    }

    #endregion
}
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using TMPro;
using UnityEngine;
using Debug = UnityEngine.Debug;

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
        xp = PlayerPrefs.GetFloat("_currentXp");
        level = PlayerPrefs.GetInt("_currentLevel");

        SetXPUI(xp);
        SetLevelUI(level);
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

    private void SetHPUI(float hp)
    {
        if (hpFillRect == null || hpText == null)
            return;

        hp = Mathf.Max(0, hp);
        float percent = Mathf.Clamp01(hp / hpMax);
        float rightValue = Mathf.Lerp(438f, 0f, percent);
        hpFillRect.offsetMax = new Vector2(-rightValue, hpFillRect.offsetMax.y);
        hpText.text = $"{Mathf.RoundToInt(hp)} / {hpMax}";
    }

    private void SetXPUI(float xp)
    {
        if (xpFillRect == null || xpText == null)
            return;

        xp = Mathf.Max(0, xp);
        float percent = Mathf.Clamp01(xp / xpMax);
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
        if (stageText == null)
            return;

        string stage = round switch
        {
            0 => "오두막",
            7 => "복도",
            8 => "제단",
            _ => $"실험실 {round}층"
        };

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
        // 새 퀘스트 오브젝트 생성
        GameObject newQuest = Instantiate(questPrefab, questParent);

        // 생성한 오브젝트 내부 TextMeshProUGUI 컴포넌트 찾아서 텍스트 세팅
        TextMeshProUGUI tmp = newQuest.GetComponentInChildren<TextMeshProUGUI>();

        string questTxt = TakeQuestText(questCode,"QuestText");

        if (string.IsNullOrEmpty(questTxt))
        {
            Debug.LogWarning($"퀘스트 텍스트를 찾을 수 없습니다. 코드: {questCode}");
            return;
        }

        if (tmp != null)
        {
            tmp.text = questTxt;
        }
    }

    public void SetQuestEndUI(string questCode)
    {
        // questCode로 퀘스트 텍스트 가져오기
        string questTxt = TakeQuestText(questCode, "QuestText");
        if (string.IsNullOrEmpty(questTxt))
        {
            Debug.LogWarning($"퀘스트 텍스트를 찾을 수 없습니다. 코드: {questCode}");
            return;
        }

        // 태그 "Quest" 가진 오브젝트 중에서 퀘스트 텍스트와 일치하는 것 찾기 (최적화 위해 텍스트 비교만)
        GameObject targetQuest = null;
        foreach (var questObj in GameObject.FindGameObjectsWithTag("Quest"))
        {
            var tmp = questObj.GetComponentInChildren<TextMeshProUGUI>();
            if (tmp != null && tmp.text == questTxt)
            {
                targetQuest = questObj;
                break;
            }
        }

        if (targetQuest != null)
        {
            StartCoroutine(FadeAndDestroy(targetQuest));
        }
        else
        {
            Debug.LogWarning("완료된 퀘스트 오브젝트를 찾을 수 없습니다.");
        }
    }

    private IEnumerator FadeAndDestroy(GameObject obj)
    {
        yield return FadeManager.Instance.FadeGameObject(obj, 1f, 0f);
        Destroy(obj);
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

#region JsonUtility

// JsonUtility 배열 래퍼 클래스
[System.Serializable]
public class ListWrapper<T>
{
    public List<HUDManager.QuestData> items;
}

public static class JsonUtilityWrapper
{
    // JsonUtility로 배열을 List<T>로 변환해주는 헬퍼 함수
    public static ListWrapper<T> FromJson<T>(string json)
    {
        return JsonUtility.FromJson<ListWrapper<T>>(json);
    }
}

#endregion
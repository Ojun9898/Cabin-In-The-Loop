using UnityEngine;
using UnityEngine.UI;          // DarkUI가 자체 버튼이면 없어도 OK
using TMPro;                   // 타이틀/서브텍스트를 보여주고 싶을 때만 사용

public class ChapterSelectButton : MonoBehaviour
{
    [Header("필수")]
    [SerializeField] private string sceneName;    // 선택 시 로드할 씬 이름 (Build Settings 등록 필요)

    [Header("선택 - UI 라벨/아이콘")]
    [SerializeField] private TextMeshProUGUI title;     // 프리팹에 타이틀 텍스트가 있다면 연결
    [SerializeField] private TextMeshProUGUI subtitle;  // 난이도/설명 텍스트가 있다면 연결
    [SerializeField] private Image icon;                // 썸네일이 있다면 연결

    [Header("선택 - 시작 옵션")]
    [SerializeField] private bool resetOnStart = true;                 // 새 게임 시작 시 LV1/XP0 리셋
    [SerializeField] private CharacterType startCharacter = CharacterType.Female; // 시작 캐릭 고정(원하면 사용)

    // DarkUI 버튼의 OnClick 이벤트에 이 메서드를 연결하세요.
    public void OnClick()
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning("[ChapterSelectButton] sceneName이 비어 있습니다.");
            return;
        }

        // PlayerStatus 보장 및 시작 옵션 적용
        var ps = PlayerStatus.Ensure();
        ps.SetCharacter(startCharacter);
        if (resetOnStart) ps.ResetProgressForNewGame();

        // GameButtonController 경유가 있으면 그걸 쓰고(페이드/공통처리 일원화),
        // 없으면 FadeManager로 바로 로드합니다.
        var gbc = FindObjectOfType<GameButtonController>();
        if (gbc != null)
        {
            gbc.LoadStage(sceneName);
            return;
        }

        if (FadeManager.Instance == null)
        {
            var go = new GameObject("FadeManager");
            go.AddComponent<FadeManager>();
        }
        FadeManager.Instance.LoadSceneWithFade(sceneName);
    }

    // (선택) 에디터에서 라벨 미리보기용
    private void OnValidate()
    {
        if (title != null && !string.IsNullOrEmpty(sceneName))
            title.text = sceneName;
    }
}
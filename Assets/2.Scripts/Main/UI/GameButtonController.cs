using UnityEngine;

public class GameButtonController : MonoBehaviour
{
    [SerializeField] private CharacterType startCharacter = CharacterType.Female;
    [SerializeField] private bool resetOnStart = true; // 새 게임 리셋 여부

    // 기존 단일 시작(필요 시 유지)
    public void OnClickLoadGame()
    {
        LoadStage("(Bake)Cavin");
    }

    // StageSelectUI가 이것만 호출하면 됨
    public void LoadStage(string sceneName)
    {
        var ps = PlayerStatus.Ensure();

        // 캐릭터 고정 및 새 게임 리셋(옵션)
        ps.SetCharacter(startCharacter);
        if (resetOnStart) ps.ResetProgressForNewGame();

        if (FadeManager.Instance == null)
        {
            var go = new GameObject("FadeManager");
            go.AddComponent<FadeManager>();
        }
        FadeManager.Instance.LoadSceneWithFade(sceneName);
    }
}
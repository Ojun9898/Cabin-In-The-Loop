using UnityEngine;

public class GameButtonController : MonoBehaviour
{
    public void OnClickLoadGame()
    {
        // FadeManager 오브젝트가 없다면 생성
        if (FadeManager.Instance == null)
        {
            GameObject fadeManagerObj = new GameObject("FadeManager");
            fadeManagerObj.AddComponent<FadeManager>();
        }

<<<<<<< HEAD
        FadeManager.Instance.LoadSceneWithFade("(Bake)Cavin");
=======
        FadeManager.Instance.LoadSceneWithFade("Cavin");
>>>>>>> 1f949ed ([추가] 병합 및 씬 분리)
    }
}
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

        FadeManager.Instance.LoadSceneWithFade("(Bake)Cavin");
    }
}
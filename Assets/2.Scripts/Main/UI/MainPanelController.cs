using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DG.Tweening;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainPanelController : MonoBehaviour
{
    [SerializeField] private Transform Canvas;
    [SerializeField] private GameObject SettingPanel;

    private GameObject settingPanel;
    
    public void OnClickNewGameButton()
    {
        SceneManager.LoadScene("Game");
    }

    public void OnClickLoadGameButton()
    {
        SceneManager.LoadScene("Game");
    } 

    public void OnClickSettingButton()
    {
        if (settingPanel == null)
            settingPanel = Instantiate(SettingPanel, Canvas);

        settingPanel.SetActive(true);

        CanvasGroup canvasGroup = settingPanel.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        canvasGroup.DOFade(1, 0.1f);
    }
}

using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SettingPanelController : MonoBehaviour
{
    // BGM 조절
    // SFX 조절
    
    public void OnClickCloseButton()
    {
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        
        canvasGroup.DOFade(0, 0.1f).OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> GamePanels = new List<GameObject>();

    [SerializeField] private Image fadeImage;
    [SerializeField] private float timer, percent;
    [SerializeField] private float fadeTime = 3f;

    [SerializeField] private Button[] fadeButtons;

    private Action fadeAction;
    
    void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
            GamePanels.Add(this.transform.GetChild(i).gameObject);
        
        fadeAction += () =>
        {
            GamePanels[(int)GameManager.Instance.e_GameType].SetActive(false);
            GameManager.Instance.OnChageType(GameManager.Instance.e_GameType + 1);
            
            GamePanels[(int)GameManager.Instance.e_GameType].SetActive(true);
        };
        
        foreach (var button in fadeButtons)
            button.onClick.AddListener(FadeEvent);
    }

    public void FadeEvent()
    {
        StartCoroutine(FadeRoutine(fadeAction));
    }
    
    public void FadeEvent(Action fadeAction = null, float fadeTime = 3f, float fadeStayTime = 1f)
    {
        StartCoroutine(FadeRoutine(fadeAction, fadeTime, fadeStayTime));
    }

    IEnumerator FadeRoutine(Action fadeAction = null, float fadeTime = 3f, float fadeStayTime = 1f)
    {
        bool isFade = true;

        for (int i = 0; i < 2; i++)
        {
            timer = 0f;
            percent = 0f;

            while (percent < 1f)
            {
                timer += Time.deltaTime;
                percent = timer / fadeTime;

                float alpha = isFade ? percent : 1 - percent;

                fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, alpha);

                yield return null;
            }

            isFade = !isFade;

            if (i == 0)
                fadeAction?.Invoke();
            
            yield return new WaitForSeconds(fadeStayTime);

        }
    }
}
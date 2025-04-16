using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MainManager : Singleton<MainManager>
{
    void Start()
    {
        OnSceneLoaded();
    }

    // 오프닝 연출
    public void OnSceneLoaded()
    {
        GameObject background = GameObject.Find("Background");
        GameObject cavin = GameObject.Find("Cavin");
        GameObject title = GameObject.Find("Title");
        GameObject subTitle = GameObject.Find("SubTitle");
        GameObject buttons = GameObject.Find("Buttons");

        background.SetActive(true);
        cavin.SetActive(false);
        title.SetActive(false);
        subTitle.SetActive(false);
        buttons.SetActive(false);

        // 클릭 시 연출 skip
        if (Input.GetMouseButtonDown(0))
        {
            cavin.SetActive(true);
            title.SetActive(true);
            subTitle.SetActive(true);
            buttons.SetActive(true);
            return;
        }

        // BGM (FadeIn)

        // Cavin Image 등장 (FadeIn)
        cavin.SetActive(true);
        cavin.GetComponent<Image>().DOFade(0, 0);
        cavin.GetComponent<Image>().DOFade(1, 1.5f);

        // Title 등장 (FadeIn)
        title.SetActive(true);
        title.GetComponent<SpriteRenderer>().DOFade(0, 0);
        title.GetComponent<SpriteRenderer>().DOFade(1, 1.5f).OnComplete(() =>
        {
            // Title 글자 하나씩 등장 (FadeIn)
            for (int i = 0; i < title.transform.childCount; i++)
            {
                Transform letter = title.transform.GetChild(i);
                letter.gameObject.SetActive(true);
                letter.GetComponent<SpriteRenderer>().DOFade(0, 0);
                letter.GetComponent<SpriteRenderer>().DOFade(1, 1.5f).SetDelay(i * 0.2f);
            }
        });

        // SubTitle 등장 (FadeIn)
        subTitle.SetActive(true);
        subTitle.GetComponent<SpriteRenderer>().DOFade(0, 0);
        subTitle.GetComponent<SpriteRenderer>().DOFade(1, 1.5f).OnComplete(() =>
        {
            // SubTitle 글자 하나씩 등장 (FadeIn)
            for (int i = 0; i < subTitle.transform.childCount; i++)
            {
                Transform letter = subTitle.transform.GetChild(i);
                letter.gameObject.SetActive(true);
                letter.GetComponent<SpriteRenderer>().DOFade(0, 0);
                letter.GetComponent<SpriteRenderer>().DOFade(1, 1.5f).SetDelay(i * 0.2f);
            }
        });

        // Buttons 등장 (FadeIn)
        buttons.SetActive(true);
        buttons.GetComponent<CanvasGroup>().alpha = 0;
        buttons.GetComponent<CanvasGroup>().DOFade(1, 1.5f).OnComplete(() =>
        {
            // Buttons 클릭 가능
            buttons.GetComponentInChildren<Button>().interactable = true;
        });
    }
}

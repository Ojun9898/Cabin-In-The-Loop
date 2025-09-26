using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class MainManager : Singleton<MainManager>
{
    GameObject background, cavin, title, subTitle, buttons;
    Coroutine playOpeningCoroutine;
    bool isOpeningSkipped = false;

    void Start()
    {
        // 미리 오브젝트 캐싱
        background = GameObject.Find("Background");
        cavin = GameObject.Find("Cavin");
        title = GameObject.Find("Title");
        subTitle = GameObject.Find("SubTitle");
        buttons = GameObject.Find("Buttons");

        playOpeningCoroutine = StartCoroutine(PlayOpening());
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isOpeningSkipped)
        {
            SkipOpening();
        }
    }

    public IEnumerator PlayOpening()
    {
        background.SetActive(true);
        cavin.SetActive(false);
        title.SetActive(false);
        subTitle.SetActive(false);
        buttons.SetActive(false);

        cavin.SetActive(true);
        Image cavinImage = cavin.GetComponent<Image>();
        cavinImage.DOFade(0, 0);
        yield return cavinImage.DOFade(1, 1.5f).WaitForCompletion();

        title.SetActive(true);
        for (int i = 0; i < title.transform.childCount; i++)
        {
            var letter = title.transform.GetChild(i);
            letter.gameObject.SetActive(true);
            TMP_Text letterText = letter.GetComponent<TMP_Text>();
            Color c = letterText.color; c.a = 0f; letterText.color = c;
        }

        for (int i = 0; i < title.transform.childCount; i++)
        {
            var letter = title.transform.GetChild(i);
            TMP_Text letterText = letter.GetComponent<TMP_Text>();
            yield return letterText.DOFade(1f, 0.5f).WaitForCompletion();
            yield return new WaitForSeconds(0.4f);
        }

        subTitle.SetActive(true);
        for (int i = 0; i < subTitle.transform.childCount; i++)
        {
            var letter = subTitle.transform.GetChild(i);
            letter.gameObject.SetActive(true);
            TMP_Text letterText = letter.GetComponent<TMP_Text>();
            Color c = letterText.color; c.a = 0f; letterText.color = c;
        }

        for (int i = 0; i < subTitle.transform.childCount; i++)
        {
            var letter = subTitle.transform.GetChild(i);
            TMP_Text letterText = letter.GetComponent<TMP_Text>();
            yield return letterText.DOFade(1f, 0.5f).WaitForCompletion();
            yield return new WaitForSeconds(0.4f);
        }

        buttons.SetActive(true);
        CanvasGroup canvasGroup = buttons.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        yield return canvasGroup.DOFade(1, 1.5f).WaitForCompletion();
        buttons.GetComponentInChildren<Button>().interactable = true;
    }

    public void SkipOpening()
    {
        if (isOpeningSkipped) return;
        isOpeningSkipped = true;

        if (playOpeningCoroutine != null)
            StopCoroutine(playOpeningCoroutine);

        background.SetActive(true);
        cavin.SetActive(true);
        title.SetActive(true);
        subTitle.SetActive(true);
        buttons.SetActive(true);

        Image cavinImage = cavin.GetComponent<Image>();
        cavinImage.color = new Color(1, 1, 1, 1);

        foreach (Transform letter in title.transform)
        {
            TMP_Text text = letter.GetComponent<TMP_Text>();
            Color c = text.color; c.a = 1f; text.color = c;
        }

        foreach (Transform letter in subTitle.transform)
        {
            TMP_Text text = letter.GetComponent<TMP_Text>();
            Color c = text.color; c.a = 1f; text.color = c;
        }

        CanvasGroup canvasGroup = buttons.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1;
        buttons.GetComponentInChildren<Button>().interactable = true;
    }
}

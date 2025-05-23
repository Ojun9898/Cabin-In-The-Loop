using System.Collections;
using TMPro;
using UnityEngine;

public class MessageManager : Singleton<MessageManager>
{
    [SerializeField] private GameObject messagePrefab;

    private GameObject currentMessage;
    private TMP_Text text;
    private Canvas canvas;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    public void Message(string message)
    {
        if (canvas == null)
            canvas = CanvasManager.Instance.GetCanvas();

        if (currentMessage == null)
        {
            currentMessage = Instantiate(messagePrefab, canvas.transform);
            text = currentMessage.GetComponentInChildren<TMP_Text>();
        }

        text.text = message;
        StopAllCoroutines();
        StartCoroutine(FadeMessage());
    }

    private IEnumerator FadeMessage()
    {
        CanvasGroup group = currentMessage.GetComponent<CanvasGroup>();
        if (group == null)
            group = currentMessage.AddComponent<CanvasGroup>();

        group.alpha = 0;
        currentMessage.SetActive(true);

        float fadeInTime = 0.2f;
        for (float t = 0; t < fadeInTime; t += Time.deltaTime)
        {
            group.alpha = t / fadeInTime;
            yield return null;
        }

        group.alpha = 1;

        yield return new WaitForSeconds(0.5f);

        float fadeOutTime = 0.2f;
        for (float t = 0; t < fadeOutTime; t += Time.deltaTime)
        {
            group.alpha = 1 - (t / fadeOutTime);
            yield return null;
        }

        group.alpha = 0;
        currentMessage.SetActive(false);
    }
}
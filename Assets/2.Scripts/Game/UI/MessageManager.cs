using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MessageManager : Singleton<MessageManager>
{
    [Header("프리팹")]
    [SerializeField] private GameObject playerMPrefab;      
    [SerializeField] private GameObject otherMPrefab; 
    [SerializeField] private GameObject noticeMPrefab;       

    private Canvas canvas;
    private Queue<(string message, bool waitForClick, GameObject prefab)> messageQueue = new Queue<(string, bool, GameObject)>();
    private bool isShowing = false;

    public bool IsDone => !isShowing && messageQueue.Count == 0;

    public System.Action OnMessagesEnded; // 메시지 완료 콜백

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    public void Message(string message) => EnqueueMessage(message, true, playerMPrefab);
    public void MessageOther(string message) => EnqueueMessage(message, true, otherMPrefab);
    public void MessageNotice(string message) => EnqueueMessage(message, false, noticeMPrefab);

    private void EnqueueMessage(string message, bool waitForClick, GameObject prefab)
    {
        messageQueue.Enqueue((message, waitForClick, prefab));
        if (!isShowing)
            StartCoroutine(ProcessQueue());
    }

    private IEnumerator ProcessQueue()
    {
        isShowing = true;

        if (canvas == null)
            canvas = CanvasManager.Instance.GetCanvas();

        while (messageQueue.Count > 0)
        {
            var (msg, waitForClick, prefab) = messageQueue.Dequeue();
            GameObject msgObj = Instantiate(prefab, canvas.transform);
            TMP_Text text = msgObj.GetComponentInChildren<TMP_Text>();
            text.text = msg;

            CanvasGroup group = msgObj.GetComponent<CanvasGroup>() ?? msgObj.AddComponent<CanvasGroup>();
            msgObj.SetActive(true);

            yield return Fade(group, 0f, 1f, 0.2f);

            if (waitForClick)
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.F));
            else
                yield return new WaitForSecondsRealtime(1f); // Notice 메시지용

            yield return Fade(group, 1f, 0f, 0.2f);
            Destroy(msgObj);
        }

        isShowing = false;

        // 메시지 종료 시 콜백 호출
        OnMessagesEnded?.Invoke();
    }

    private IEnumerator Fade(CanvasGroup group, float start, float end, float duration)
    {
        float time = 0f;

        while (time < duration)
        {
            group.alpha = Mathf.Lerp(start, end, time / duration);
            time += Time.unscaledDeltaTime; // 일시정지와 상관없이 동작
            yield return null;
        }
        group.alpha = end;
    }
}

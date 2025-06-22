using UnityEngine;
using System.Collections;

public class BGMManager : MonoBehaviour
{
    public static BGMManager Instance;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] bgmClips;

    private Coroutine fadeCoroutine;
    private float defaultVolume = 1f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.loop = true;
        audioSource.volume = defaultVolume;
        
        audioSource.ignoreListenerPause = true;
    }

    public void Play(string bgmName, float fadeDuration = 1f)
    {
        AudioClip newClip = FindClipByName(bgmName);

        if (audioSource.clip == newClip)
            return;

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeToNewBGM(newClip, fadeDuration));
    }

    private IEnumerator FadeToNewBGM(AudioClip newClip, float duration)
    {
        float t = 0f;
        float startVolume = audioSource.volume;

        // Fade out
        while (t < duration)
        {
            t += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0, t / duration);
            yield return null;
        }

        // 기존 클립 중지
        audioSource.Stop();
        audioSource.clip = newClip;

        audioSource.Play();

        // Fade in
        t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0, defaultVolume, t / duration);
            yield return null;
        }

        audioSource.volume = defaultVolume;
    }


    public void SetVolume(float volume)
    {
        defaultVolume = volume;
        audioSource.volume = volume;
    }

    private AudioClip FindClipByName(string name)
    {
        foreach (var clip in bgmClips)
        {
            if (clip.name == name)
                return clip;
        }
        return null;
    }
}

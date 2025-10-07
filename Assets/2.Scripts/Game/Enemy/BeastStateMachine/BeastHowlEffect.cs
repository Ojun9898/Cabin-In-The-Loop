using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;


[DisallowMultipleComponent]
public class BeastHowlEffect : MonoBehaviour
{
     [Header("Target (HDRP/Lit)")]
    public Renderer targetRenderer;
    public string baseColorProperty = "_BaseColor";   // HDRP/Lit

    [Header("Colors")]
    public Color normalColor = Color.white;
    public Color howlColor   = Color.red;

    [Header("Timings")]
    [Tooltip("색이 붉게 변하는 데 걸리는 시간")]
    public float fadeInSeconds  = 1f;
    [Tooltip("붉은 상태로 유지되는 시간(여기 동안 1.5배)")]
    public float holdSeconds    = 5.0f;
    [Tooltip("원래 색으로 되돌아가는 데 걸리는 시간")]
    public float fadeOutSeconds = 1f;

    [Header("Damage")]
    public float buffMultiplier = 1.5f;

    // 외부에서 읽는 현재 배수(상태머신/공격상태에서 곱해 사용)
    public float CurrentDamageMultiplier => _currentMultiplier;

    MaterialPropertyBlock _mpb;
    Coroutine _burstCo;
    float _currentMultiplier = 1f;

    void Awake()
    {
        if (!targetRenderer) targetRenderer = GetComponentInChildren<Renderer>();
        _mpb = new MaterialPropertyBlock();
        SetColorImmediate(normalColor);
    }

    // === 외부 트리거 ===
    public void TriggerHowlBurst()
    {
        if (_burstCo != null) StopCoroutine(_burstCo);
        _burstCo = StartCoroutine(BurstRoutine());
    }

    IEnumerator BurstRoutine()
    {
        // 1) Fade-In (색만 서서히, 배수는 즉시 1.5배 적용)
        _currentMultiplier = buffMultiplier;
        yield return FadeColor(normalColor, howlColor, fadeInSeconds);

        // 2) Hold 5초 (이 동안 계속 1.5배 유지)
        yield return WaitUnscaled(holdSeconds);

        // 3) Fade-Out 시작과 동시에 배수 복귀(=1.0)
        _currentMultiplier = 1f;
        yield return FadeColor(howlColor, normalColor, fadeOutSeconds);

        _burstCo = null;
    }

    IEnumerator FadeColor(Color from, Color to, float sec)
    {
        if (sec <= 0f) { SetColorImmediate(to); yield break; }
        float t = 0f;
        while (t < sec)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / sec);
            ApplyColor(Color.Lerp(from, to, k));
            yield return null;
        }
        SetColorImmediate(to);
    }

    static IEnumerator WaitUnscaled(float sec)
    {
        float t = 0f;
        while (t < sec)
        {
            t += Time.unscaledDeltaTime;
            yield return null;
        }
    }

    void SetColorImmediate(Color c)
    {
        if (!targetRenderer) return;
        targetRenderer.GetPropertyBlock(_mpb);
        _mpb.SetColor(baseColorProperty, c);
        targetRenderer.SetPropertyBlock(_mpb);
    }

    void ApplyColor(Color c)
    {
        if (!targetRenderer) return;
        targetRenderer.GetPropertyBlock(_mpb);
        _mpb.SetColor(baseColorProperty, c);
        targetRenderer.SetPropertyBlock(_mpb);
    }
}

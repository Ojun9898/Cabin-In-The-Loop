using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

[DisallowMultipleComponent]
public class BeastHowlEffect : MonoBehaviour
{
    [Header("머테리얼 색 변경")]
    public Renderer targetRenderer;
    public string baseColorProperty = "_BaseColor";
    public Color normalColor = Color.white;
    public Color howlColor   = Color.red;

    [Header("효과 및 지속시간")]
    // 색이 붉게 변하는 데 걸리는 시간
    public float fadeInSeconds  = 1f;
    // 상태 유지시간
    public float holdSeconds    = 5.0f;
    // 원래 색으로 되돌아가는 데 걸리는 시간
    public float fadeOutSeconds = 1f;
    // 버프시 자신의 공격력 증가량
    public float buffMultiplier = 1.5f;

    // 외부에서 읽는 현재 배수(상태머신/공격상태에서 곱해 사용)
    public float CurrentDamageMultiplier => _currentMultiplier;

    // MaterialPropertyBlock : 원본 머티리얼을 복제하지 않고,해당 Renderer에만 색상/값을 덮어써서 표시
    MaterialPropertyBlock _mpb;
    Coroutine _burst;
    float _currentMultiplier = 1f;

    void Awake()
    {
        if (!targetRenderer) targetRenderer = GetComponentInChildren<Renderer>();
        _mpb = new MaterialPropertyBlock();
        ApplyColorImmediate(normalColor);
    }

    // 외부 트리거 
    public void TriggerHowlBurst()
    {
        // 이미 돌고있던 코루틴이 있으면 중지
        if (_burst != null) StopCoroutine(_burst);
        _burst = StartCoroutine(BurstRoutine());
    }

    IEnumerator BurstRoutine()
    {
        // 1) Fade-In (색만 서서히, 배수는 즉시 1.5배 적용)
        _currentMultiplier = buffMultiplier;
        yield return FadeColor(normalColor, howlColor, fadeInSeconds);

        // 2) 5초동안 효과유지
        yield return OverEffect(holdSeconds);

        // 3) Fade-Out 시작과 동시에 배수 복귀(=1.0)
        _currentMultiplier = 1f;
        yield return FadeColor(howlColor, normalColor, fadeOutSeconds);

        // 코루틴이 종료됬음을 알림
        _burst = null;
    }

    IEnumerator FadeColor(Color from, Color to, float second)
    {
        if (second <= 0f) { ApplyColorImmediate(to); yield break; }
        float time = 0f;
        while (time < second)
        {
            // Time.unscaledDeltaTime : 현재 프레임이 걸린 “실시간” 경과(초)(UI/이펙트 타이밍을 게임 속도와 독립적으로 유지할 때 적합)
            time += Time.unscaledDeltaTime;
            // Mathf.Clamp01 : 값을 0과 1 사이로 자름(clamp).
            // 마지막 프레임에서 1을 넘을 수 있어서(누적 오차) 항상 0~1 범위로 보장
            float k = Mathf.Clamp01(time / second);
            ApplyColor(Color.Lerp(from, to, k));
            yield return null;
        }
        ApplyColorImmediate(to);
    }

    static IEnumerator OverEffect(float second)
    {
        float time = 0f;
        while (time < second)
        {
            time += Time.unscaledDeltaTime;
            yield return null;
        }
    }

    // 기본색으로 고정하거나, 보간 없이 바로 바뀌어야 할때 사용 
    void ApplyColorImmediate(Color c)
    {
        if (!targetRenderer) return;
        // 현재 MPB 내용 가져오기(기존 값 유지)
        targetRenderer.GetPropertyBlock(_mpb);
        // 지정한 프로퍼티(_BaseColor 등)에 색 대입
        _mpb.SetColor(baseColorProperty, c);
        // 이 Renderer에만 적용(공유 머티리얼 미복제)
        targetRenderer.SetPropertyBlock(_mpb);
    }

    // 천천이 보간될때 사용
    void ApplyColor(Color c)
    {
        if (!targetRenderer) return;
        targetRenderer.GetPropertyBlock(_mpb);
        _mpb.SetColor(baseColorProperty, c);
        targetRenderer.SetPropertyBlock(_mpb);
    }
}

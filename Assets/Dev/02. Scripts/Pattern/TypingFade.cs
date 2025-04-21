using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class TypingFade : MonoBehaviour
{
    private TextMeshProUGUI textUI;
    private string currentText;

    void Awake()
    {
        textUI = this.GetComponent<TextMeshProUGUI>();
        currentText = textUI.text;
    }

    void OnEnable()
    {
        textUI.text = string.Empty;
    }

    IEnumerator Start()
    {
        yield return new WaitForSeconds(3f); // Fade 시간 대기

        // 타이핑 효과
        int textCount = currentText.Length;
        for (int i = 0; i < textCount; i++)
        {
            textUI.text += currentText[i];
            yield return new WaitForSeconds(0.15f);
        }

        yield return new WaitForSeconds(1f);
        
        // 텍스트 페이드 효과
        float timer = 0f;
        float percent = 0f;
        while (percent < 1f)
        {
            timer += Time.deltaTime;
            percent = timer / 2f;

            textUI.color = new Color(textUI.color.r, textUI.color.g, textUI.color.b, 1- percent);
            yield return null;
        }
    }
}
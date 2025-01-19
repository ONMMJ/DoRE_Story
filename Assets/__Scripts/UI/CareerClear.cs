using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CareerClear : MonoBehaviour
{
    Image fadeImg;
    bool isPlaying = false;
    Text fadeText;

    void Awake()
    {
        fadeImg = GetComponent<Image>();
        fadeText = transform.GetChild(0).GetComponent<Text>();
        fadeImg.color = new Color(255, 255, 255, 0);
        fadeText.color = new Color(255, 255, 255, 0);
    }

    public void setText(string Text)
    {
        transform.GetChild(0).GetComponent<Text>().text = Text;
    }

    public void OutStartFadeAnim()
    {
        if (isPlaying == true) //중복재생방지
        {
            return;
        }
        StartCoroutine(FadeOut(1f));    //코루틴 실행
    }

    public void InStartFadeAnim()
    {
        if (isPlaying == true) //중복재생방지
        {
            return;
        }
        StartCoroutine(FadeIn(1f));
    }

    IEnumerator FadeIn(float fadeOutTime)
    {
        isPlaying = true;
        Color color = fadeImg.color;
        while (color.a < 1f)
        {
            color.a += Time.deltaTime / fadeOutTime;
            fadeImg.color = color;
            fadeText.color = color;

            if (color.a >= 1f) color.a = 1f;

            yield return null;
        }
        isPlaying = false;
    }
    IEnumerator FadeOut(float fadeInTime)
    {
        isPlaying = true;
        Color color = fadeImg.color;
        while (color.a > 0f)
        {
            color.a -= Time.deltaTime / fadeInTime;
            fadeImg.color = color;
            fadeText.color = color;

            if (color.a <= 0f) color.a = 0f;

            yield return null;
        }
        isPlaying = false;
    }

    public IEnumerator fadeinoutplay()
    {
        InStartFadeAnim();
        yield return new WaitForSeconds(2f);
        OutStartFadeAnim();
    }
}
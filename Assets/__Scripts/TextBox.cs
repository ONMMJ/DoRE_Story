using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextBox : MonoBehaviour
{
    public static TextBox textBox;
    public Text getComText;
    public bool isStart;
    // Start is called before the first frame update
    void Start()
    {
        textBox = GetComponent<TextBox>();
        getComText = transform.Find("Text").GetComponent<Text>();
        gameObject.SetActive(false);
        isStart = false;
    }

    public static IEnumerator textTyping(string text)
    {
        if (!textBox.isStart)
        {
            textBox.gameObject.SetActive(true);
            textBox.isStart = true;
            textBox.getComText.text = "";
            float count = 0;
            float speed = 10;
            while (true)
            {
                yield return null;
                count += Time.deltaTime;
                textBox.getComText.text = text.Substring(0, (int)(Mathf.Lerp(0f,1f,count/text.Length*speed) *text.Length));
                if (count >= text.Length/speed || Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
                {
                    textBox.getComText.text = text;
                    break;
                }
            }

            while (true)
            {
                yield return null;
                if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
                    break;
            }
            textBox.isStart = false;
            textBox.gameObject.SetActive(false);
        }
    }

    public static IEnumerator textTyping(string[] texts)
    {
        foreach (var text in texts)
        {
            yield return textTyping(text);
        }
    }
}

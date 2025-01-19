using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpBar : MonoBehaviour
{
    private RectTransform rectTransform;
    private float maxWidth;
    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        maxWidth = rectTransform.rect.width;
    }

    public void SetHp(int hp, int maxHp)
    {
        float width = maxWidth * (float)hp / (float)maxHp;
        rectTransform.sizeDelta = new Vector2(width, rectTransform.rect.height);
    }
}

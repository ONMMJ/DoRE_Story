using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour
{
    static UI ui;
    // Start is called before the first frame update
    void Start()
    {
        ui = GetComponent<UI>();
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void HideUI()
    {
        ui.gameObject.SetActive(false);
    }

    public static void ShowUI()
    {
        ui.gameObject.SetActive(true);
    }
}

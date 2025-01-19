using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckWindow : MonoBehaviour
{
    public Text mainText;
    public InputField countText;
    string recycleType;
    string itemName;
    int itemCount;
    RecycleBoard RB;
    Inven_Director ID;
    Status SP;
    Money money;

    // Start is called before the first frame update
    void Start()
    {
        RB = FindObjectOfType<RecycleBoard>();
        ID = FindObjectOfType<Inven_Director>();
        SP = FindObjectOfType<Status>();
        money = FindObjectOfType<Money>();
    }

    // Update is called once per frame
    void Update()
    {
        if (recycleType != null && countText.text != "")
            if (int.Parse(countText.text) > itemCount)
            {
                Debug.Log(countText.text+"/////////////////" + itemCount);
                countText.text = itemCount.ToString();
            }
    }

    public void setText(string main, string type, string iName, int count)
    {
        gameObject.SetActive(true);
        mainText.text = main;
        recycleType = type;
        itemName = iName;
        itemCount = count;
    }

    public void ConfirmButton()
    {
        RB.updateRecycle(recycleType, int.Parse(countText.text));
        ID.StartCoroutine(ID.UpdateItemCount(itemName, (-int.Parse(countText.text)).ToString()));
        SP.AddStatPoint(recycleType, int.Parse(countText.text));
        money.AddMoney(recycleType, int.Parse(countText.text));
        resetInf();
        gameObject.SetActive(false);
    }
    public void CancleButton()
    {
        resetInf();
        gameObject.SetActive(false);
    }

    void resetInf()
    {
        recycleType = null;
        itemCount = 0;
        countText.text = "";
    }
}

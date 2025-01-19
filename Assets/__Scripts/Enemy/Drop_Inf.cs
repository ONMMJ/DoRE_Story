using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drop_Inf : MonoBehaviour
{
    string itemName;
    int count;
    Inven_Director inven;
    // Start is called before the first frame update
    void Start()
    {
        inven = FindObjectOfType<Inven_Director>();
        GetComponent<Rigidbody2D>().velocity= Vector3.up * 3f;
    }
    public void Set(string iName, int iCount)
    {
        itemName = iName;
        count = iCount;
    }
    public void PickUpItem()
    {
        Debug.Log("callCount");
        inven.StartCoroutine(inven.UpdateItemCount(itemName, count.ToString()));
        GetComponent<Rigidbody2D>().velocity = Vector3.up * 3f;
        Destroy(gameObject,0.3f);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Net;
using System.IO;
using SimpleJSON;
using System.Runtime.Serialization.Formatters.Binary;

public class Item_Inf
{
    // Start is called before the first frame update
    public string itemId;
    public Sprite itemImage;
    public string itemName;
    public int itemCount;
    public string itemType;
    public bool isItemAcquired;

    public Item_Inf(string ItemId, Sprite ItemImage, string ItemName, int ItemCount, string ItemType, bool IsItemAcquired)
    {
        this.itemId = ItemId;
        this.itemImage = ItemImage;
        this.itemName = ItemName;
        this.itemCount = ItemCount;
        this.itemType = ItemType;
        this.isItemAcquired = IsItemAcquired;
    }
}

public class Post_Item
{
    public string itemId;
    public int itemCount;

    public Post_Item(string ItemId, int ItemCount)
    {
        this.itemId = ItemId;
        this.itemCount = ItemCount;
    }
}

public class LoginData
{
    public string login_id;
    public string password;
}

public class UpdateItem
{
    public string item_name;
    public string obtained_item_count;
}

public class Inven_Director : MonoBehaviour
{
    // Start is called before the first frame update
    int Now_page;
    int Max_page;
    int Count_remainder;
    bool isStart;
    GameObject[] Slot_list = new GameObject[6];
    List<Item_Inf> Item_list = new List<Item_Inf>();
    public Sprite No_Item_mark;
    public Text Left_text;
    public Text Right_text;
    Player_Director PD;
    public CheckWindow CW;
    string url = "http://54.180.217.191:8000/v1/inventories/";
    byte[] updateFormData;

    public Animator animator;

    void Start()
    {
        isStart = true;
        StartCoroutine(GetRequest(url));
        //Login();
        for (int i = 0; i < 6; i++) {
            Slot_list[i] = transform.GetChild(i).gameObject;
        }
        animator = gameObject.transform.parent.GetComponent<Animator>();
        Debug.Log(Max_page);

        PD = FindObjectOfType<Player_Director>();
    }

    public void Login()
    {
        string url = "http://54.180.217.191:8000/v1/users/login/";
        LoginData data = new LoginData();

        data.login_id = "dd@dd.com";
        data.password = "123123";

        string str = JsonUtility.ToJson(data);
        var bytes = System.Text.Encoding.UTF8.GetBytes(str);

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        request.Method = "POST";
        request.ContentType = "application/json";
        request.ContentLength = bytes.Length;

        using (var stream = request.GetRequestStream())
        {
            stream.Write(bytes, 0, bytes.Length);
            stream.Flush();
            stream.Close();
        }
        HttpWebResponse res = (HttpWebResponse)request.GetResponse();
        StreamReader reader = new StreamReader(res.GetResponseStream());
        string userData = reader.ReadToEnd(); // user: {}
        //string json = reader.ReadToEnd();
        //Debug.Log("user: " + userData);
        //UpdateItemCount("분리수검", "4");
        //User user = new User(userData);
    }

    public IEnumerator UpdateItemCount(string item_name, string obtained_item_count)
    {
        yield return null;
        string url = "http://54.180.217.191:8000/v1/update-items/?"+ "item_name=" + item_name + "&obtained_item_count=" + obtained_item_count;
        UpdateItem item = new UpdateItem();
        item.item_name = item_name;
        item.obtained_item_count = obtained_item_count;

        string str = JsonUtility.ToJson(item);
        var bytes = System.Text.Encoding.UTF8.GetBytes(str);

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        request.Method = "POST";
        request.ContentType = "application/json";
        request.ContentLength = bytes.Length;

        using (var stream = request.GetRequestStream())
        {
            stream.Write(bytes, 0, bytes.Length);
            stream.Flush();
            stream.Close();
        }
        HttpWebResponse res = (HttpWebResponse)request.GetResponse();
        StreamReader reader = new StreamReader(res.GetResponseStream());
        string resMsg = reader.ReadToEnd(); // user: {}
        //string json = reader.ReadToEnd();
        Debug.Log("결과 msg(Message): " + resMsg);
    }


    // Update is called once per frame
    void Update()
    {
        if (animator.GetBool("Open") == false)    //인벤 닫혔을 때
        {
            if (Input.GetKeyDown("i"))
            {
                StartCoroutine(GetRequest(url));
                animator.SetBool("Open", true);
            }
        }
        else                                            //인벤 열렸을 때
        {
            if (Input.GetKeyDown("i") || Input.GetKeyDown(KeyCode.Escape))
            {
                animator.SetBool("Open", false);
            }
        }
        if (Input.GetKeyDown("z"))
        {
            Item_Inf inf = Item_list.Find(item => item.itemName == "분리수검");
            Debug.Log(inf.itemId.ToString()+ inf.itemCount.ToString()+ inf.isItemAcquired.ToString());
        }
    }

    public void Click_Right_page()
    {
        if( Now_page < Max_page )
        {
            Now_page++;
            Left_text.text = Now_page.ToString();
            HideItemSlot();
            SetItemSlot();
        }
    }
    public void Click_left_page()
    {
        if (Now_page > 1)
        {
            Now_page--;
            Left_text.text = Now_page.ToString();
            HideItemSlot();
            SetItemSlot();
        }
    }

    void HideItemSlot()
    {
        if (Now_page == Max_page && Count_remainder > 0)
        {
            for(int i = Count_remainder; i < 6; i++)
            {
                Slot_list[i].SetActive(false);
            }
        }
        if (Now_page < Max_page)
        {
            for (int i = 0; i < 6; i++)
            {
                Slot_list[i].SetActive(true);
            }
        }
    }

    void SetItemSlot()
    {
        int count = 6;
        if (Now_page == Max_page && Count_remainder > 0)
            count = Count_remainder;
        for (int i = 0; i < count; i++)
        {
            if (Item_list[(Now_page - 1) * 6 + i].isItemAcquired == false)
            {
                Slot_list[i].transform.GetChild(0).GetComponent<Image>().sprite = No_Item_mark;
                Slot_list[i].transform.GetChild(1).GetComponent<Text>().text = "???";
                Slot_list[i].transform.GetChild(2).GetComponent<Text>().text = "X 0";
            }
            else
            {
                Slot_list[i].transform.GetChild(0).GetComponent<Image>().sprite = Item_list[(Now_page - 1) * 6 + i].itemImage;
                Slot_list[i].transform.GetChild(1).GetComponent<Text>().text = Item_list[(Now_page - 1) * 6 + i].itemName;
                Slot_list[i].transform.GetChild(2).GetComponent<Text>().text = "X " + Item_list[(Now_page - 1) * 6 + i].itemCount.ToString();
            }
        }
    }

    public void Active_Button(int i)
    {
        Debug.Log((Now_page - 1) * 6 + i);
        if (PD.interaction == null)
            return;
        Interaction_Inf inf = PD.interaction.GetComponent<Interaction_Inf>();
        if (inf.type == "Recycle")
        {
            string mainText = Item_list[(Now_page - 1) * 6 + i].itemName + "을(를) " + inf.recycleType + "(으)로 버리시겠습니까?";
            string type = inf.recycleType;
            if (Item_list[(Now_page - 1) * 6 + i].itemType != inf.recycleType)
                type = "잘못된분리수거";
            string itemName = Item_list[(Now_page - 1) * 6 + i].itemName;
            int count = Item_list[(Now_page - 1) * 6 + i].itemCount;
            CW.setText(mainText, type, itemName, count);
            animator.SetBool("Open", false);
        }
    }

    IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            Item_list.Clear();
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    //Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);

                    string requestResult = webRequest.downloadHandler.text;
                    JSONNode result = JSON.Parse(requestResult);
                 
                    //Debug.Log(result);
                    //Debug.Log(result["results"]); // 배열
                    //Debug.Log(result["count"]);   // 아이템 개수
                    //Debug.Log("-----------------------------");
                    int count = result["count"];
                    var datas = result["results"];
                    Debug.Log(datas);

                    for (int i = 0; i < count; i++)
                    {
                        var data = datas[i];
                        yield return StartCoroutine(SetSprite(data));
                        // https://dorestory.s3.ap-northeast-2.amazonaws.com/static/item_images/resized_zQddhYPMQV_161812242669.png
                        // 이미지 url
                    }
                    if (isStart)
                        yield return StartCoroutine(SetStart());
                    else
                        yield return StartCoroutine(SetInf());
                    //List<Item_Inf> inventories = 
                    //Debug.Log(inventories);
                    break;
            }
        }
    }
    IEnumerator SetSprite(JSONNode Data)
    {
        //Debug.Log("SetSprite function start!!");
        //Debug.Log(Data);
        Sprite urlItemImage;
        using (WWW www = new WWW(Data["item"]["image"]))
        //using (WWW www = new WWW(Data["image"]))
        {
            yield return www;
            Texture2D tex = new Texture2D(70, 70, TextureFormat.DXT1, false);
            //Debug.Log(Data["item"]["name"]);
            www.LoadImageIntoTexture(tex);
            urlItemImage = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one * 0.5f);
        }
        Debug.Log("==================SetSprite function end======================");
        var itemInf = new Item_Inf(Data["id"], urlItemImage, Data["item"]["name"], Data["count"], Data["type"], Data["is_acquired"]);
        Item_list.Add(itemInf);
        Post_Item item = new Post_Item(Data["id"], 3);
    
    }

    public static byte[] ObjectToByteArray(Item_Inf obj)
    {
        BinaryFormatter bf = new BinaryFormatter();
        using (var ms = new MemoryStream())
        {
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }
    }

    IEnumerator SetStart()
    {
        Now_page = 1;
        Max_page = (int)Mathf.Ceil(Item_list.Count / 6.0f);
        Count_remainder = Item_list.Count % 6;
        Left_text.text = Now_page.ToString();
        Right_text.text = Max_page.ToString();

        HideItemSlot();
        SetItemSlot();
        isStart = false;
        yield return null;
    }
    IEnumerator SetInf()
    {
        HideItemSlot();
        SetItemSlot();
        yield return null;
    }
}


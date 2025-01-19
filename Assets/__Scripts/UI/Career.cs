using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Net;
using System.IO;
using SimpleJSON;
public class Career_Inf
{
    // Start is called before the first frame update
    public string careerTitle;
    public string careerText;
    public Sprite cBookImage;
    public string cBookText;
    public string careerType;
    public string careerTarget;
    public int count;
    public int maxCount;
    public bool isClear;

    public Career_Inf(string CareerTitle, string CareerText, Sprite CBImage, string CBText, string CareerType, string CareerTarget, int Count, int MaxCount, bool IsClear)
    {
        this.careerTitle = CareerTitle;
        this.careerText = CareerText;
        this.cBookImage = CBImage;
        this.cBookText = CBText;
        this.careerType = CareerType;
        this.careerTarget = CareerTarget;
        this.count = Count;
        this.maxCount = MaxCount;
        this.isClear = IsClear;
    }
}

public class UpdateCareer
{
    public string name;
    public int obtained_count;
    public bool is_clear;
}

public class Career : MonoBehaviour
{
    List<Career_Inf> careerList = new List<Career_Inf>();
    GameObject[] Slot_list = new GameObject[8];
    public GameObject careerBook;
    public CareerClear clear;
    int Now_page;
    int Max_page;
    int Count_remainder;
    bool isStart;
    public Text Left_text;
    public Text Right_text;
    Animator animator;
    RecycleBoard RB;
    string url = "http://54.180.217.191:8000/v1/careers/";

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < Slot_list.Length; i++)
        {
            Slot_list[i] = transform.Find("CareerGroup").GetChild(i).gameObject;
        }
        animator = GetComponent<Animator>();
        isStart = true;
        RB = FindObjectOfType<RecycleBoard>();
        StartCoroutine(GetRequest(url));
    }

    // Update is called once per frame
    void Update()
    {
        if (animator.GetBool("Open") == false)    //인벤 닫혔을 때
        {
            if (Input.GetKeyDown("o"))
            {
                StartCoroutine(GetRequest(url));
                animator.SetBool("Open", true);
            }
        }
        else                                            //인벤 열렸을 때
        {
            if (Input.GetKeyDown("o") || Input.GetKeyDown(KeyCode.Escape))
            {
                animator.SetBool("Open", false);
            }
        }

        foreach (var career in careerList)
        {
            if (career.isClear == false)
            {
                switch (career.careerType)
                {
                    case "recycle":
                        recycle(career);
                        break;
                    case "monsterKill":
                        break;
                    case "etc":
                        break;
                }
            }
        }
    }

    public IEnumerator UpdateCareerCount(string career_name, int obtained_count)
    {
        yield return null;
        string url = "http://54.180.217.191:8000/v1/update-career/?" + "career_name=" + career_name + "&obtained_count=" + obtained_count;
        UpdateCareer career = new UpdateCareer();
        career.name = career_name;
        career.obtained_count = obtained_count;
        string str = JsonUtility.ToJson(career);
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

    public IEnumerator UpdateCareerClear(string career_name, bool is_clear)
    {
        yield return null;
        string url = "http://54.180.217.191:8000/v1/update-career/?" + "career_name=" + career_name + "&is_clear=" + is_clear;
        UpdateCareer career = new UpdateCareer();
        career.is_clear = is_clear;
        string str = JsonUtility.ToJson(career);
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
    }

    IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            careerList.Clear();
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
        var career = Data["career"];
        using (WWW www = new WWW(career["book"]["image"]))
        //using (WWW www = new WWW(Data["image"]))
        {
            yield return www;
            Texture2D tex = new Texture2D(70, 70, TextureFormat.DXT1, false);
            www.LoadImageIntoTexture(tex);
            urlItemImage = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one * 0.5f);
        }
        Debug.Log("==================SetSprite function end======================");
        var itemInf = new Career_Inf(career["name"], career["content"], urlItemImage, career["book"]["content"], career["type_name"], career["type_detail_name"], Data["current_count"], Data["max_count"], Data["is_clear"]);
        careerList.Add(itemInf);
        Post_Item item = new Post_Item(Data["id"], 3);

    }

    IEnumerator SetStart()
    {
        Now_page = 1;
        Max_page = (int)Mathf.Ceil(careerList.Count / (float)Slot_list.Length);
        Count_remainder = careerList.Count % Slot_list.Length;
        Left_text.text = Now_page.ToString();
        Right_text.text = Max_page.ToString();

        HideItemSlot();
        SetCareerSlot();
        isStart = false;
        yield return null;
    }
    IEnumerator SetInf()
    {
        HideItemSlot();
        SetCareerSlot();
        yield return null;
    }
    public void Click_Right_page()
    {
        if (Now_page < Max_page)
        {
            Now_page++;
            Left_text.text = Now_page.ToString();
            HideItemSlot();
            SetCareerSlot();
        }
    }
    public void Click_left_page()
    {
        if (Now_page > 1)
        {
            Now_page--;
            Left_text.text = Now_page.ToString();
            HideItemSlot();
            SetCareerSlot();
        }
    }

    void HideItemSlot()
    {
        if (Now_page == Max_page && Count_remainder > 0)
        {
            for (int i = Count_remainder; i < Slot_list.Length; i++)
            {
                Slot_list[i].SetActive(false);
            }
        }
        if (Now_page < Max_page)
        {
            for (int i = 0; i < Slot_list.Length; i++)
            {
                Slot_list[i].SetActive(true);
            }
        }
    }

    void SetCareerSlot()
    {
        if (careerList.Count == 0)
            return;
        int count = Slot_list.Length;
        if (Now_page == Max_page && Count_remainder > 0)
            count = Count_remainder;
        for (int i = 0; i < count; i++)
        {
            Slot_list[i].transform.GetChild(0).GetComponent<Text>().text = careerList[(Now_page - 1) * 6 + i].careerTitle;
            if (careerList[(Now_page - 1) * Slot_list.Length + i].isClear == false)
            {
                Slot_list[i].transform.GetChild(0).GetComponent<Button>().enabled = false;
                Slot_list[i].transform.GetChild(0).GetComponent<Text>().color = Color.gray;
            }
            else
            {
                Slot_list[i].transform.GetChild(0).GetComponent<Button>().enabled = true;
                Slot_list[i].transform.GetChild(0).GetComponent<Text>().color = Color.black;
            }
        }
    }

    public void cBookButton(int i)
    {
        careerBook.SetActive(true);
        careerBook.transform.GetChild(0).GetComponent<Image>().sprite = careerList[(Now_page - 1) * 6 + i].cBookImage;  //image
        careerBook.transform.GetChild(1).GetComponent<Text>().text = careerList[(Now_page - 1) * 6 + i].careerText;  //Title
        careerBook.transform.GetChild(2).GetComponent<Text>().text = careerList[(Now_page - 1) * 6 + i].cBookText;  //Main
    }
    public void cBookOffButton()
    {
        careerBook.SetActive(false);
    }

    void recycle(Career_Inf career)
    {
        foreach(var recycle in RB.recycleCountList)
        {
            if (recycle.transform.parent.name == career.careerTarget)
            {
                if (int.Parse(recycle.text) >= career.maxCount)
                {
                    career.isClear = true;
                    StartCoroutine(UpdateCareerClear(career.careerTitle, true));
                    CareerClear(career.careerTitle);
                }
                break;
            }
        }
    }

    void CareerClear(string text)
    {
        clear.StartCoroutine(clear.fadeinoutplay());
        clear.setText(text);
    }
}

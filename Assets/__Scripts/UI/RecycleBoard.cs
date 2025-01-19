using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Net;
using System.IO;
using SimpleJSON;

public class UpdateRecycle
{
    public string name;
    public int obtained_count;
}
public class RecycleBoard : MonoBehaviour
{
    private RectTransform rectTransform;
    int isOpen;
    bool isClick;
    public GameObject Arrow;
    Sprite weaponImage;
    GameObject recycleGroup;
    public List<Text> recycleCountList= new List<Text>();  //일반, 음식, 페트, 비닐, 종이, 플리스틱, 캔류, 공병, 배터리, 폐기물, 불연성, 잘못된분리수거
    string url = "http://54.180.217.191:8000/v1/recycle_counts/";
    void Start()
    {
        isOpen = -1;
        rectTransform = GetComponent<RectTransform>();
        isClick = false;
        weaponImage = transform.Find("WeaponImage").GetComponent<Image>().sprite;
        recycleGroup = transform.Find("RecycleGroup").gameObject;
        setRecycleList();
        StartCoroutine(GetRequest(url));
        StartCoroutine(UpdateCareerCount("일반", 3));
    }

    void setRecycleList()
    {
        Transform[] recycleList = GetComponentsInChildren<Transform>();
        foreach (var a in recycleList)
        {
            if (a.name == "Text")
            {
                recycleCountList.Add(a.GetComponent<Text>());
                a.GetComponent<Text>().text = "0";
            }
        }
    }

    public void updateRecycle(string type, int count)
    {
        foreach (var a in recycleCountList)
        {
            if (a.transform.parent.name == type)
            {
                a.text = (int.Parse(a.text)+count).ToString();
                return;
            }
        }
    }

    public int getRecycleCount(string type)
    {
        foreach (var a in recycleCountList)
        {
            if (a.transform.parent.name == type)
            {
                return int.Parse(a.text);
            }
        }
        return -1;
    }
    public void ToggleOpenButton()
    {
        if (isClick)
            return;
        StartCoroutine(GetRequest(url));
        float width = rectTransform.rect.width;
        Vector3 pos = rectTransform.anchoredPosition;
        pos = new Vector3(pos.x + width * isOpen, pos.y, pos.z);
        StartCoroutine(MoveTo(pos, 0.03f));
        isOpen *= -1;
        FlipArrow(isOpen);
    }

    void FlipArrow(int Flip)
    {
        Arrow.transform.localScale = new Vector3(Flip, 1, 1);
    }
    IEnumerator MoveTo(Vector3 toPos, float time)
    {
        isClick = true;
        float count = 0;
        while (true)
        {
            Vector3 velo = Vector3.zero;
            count += Time.deltaTime;
            rectTransform.anchoredPosition = Vector3.SmoothDamp(rectTransform.anchoredPosition, toPos, ref velo, time);

            if (count >= 0.7f)
            {
                rectTransform.anchoredPosition = toPos;
                break;
            }
            yield return null;
        }
        isClick = false;
    }
    public IEnumerator UpdateCareerCount(string recycle_name, int obtained_count)
    {
        yield return null;
        string url = "http://54.180.217.191:8000/v1/update-recycle-count/?" + "recycle_name=" + recycle_name + "&obtained_count=" + obtained_count;
        UpdateRecycle recycle = new UpdateRecycle();
        recycle.name = recycle_name;
        recycle.obtained_count = obtained_count;
        string str = JsonUtility.ToJson(recycle);
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
    IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
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

                    string requestResult = webRequest.downloadHandler.text;
                    JSONNode result = JSON.Parse(requestResult);

                    int count = result["count"];
                    var datas = result["results"];
                    for (int i = 0; i < count; i++)
                    {
                        var data = datas[i];
                        recycleCountList[i].text = data["current_count"].ToString();
                    }
                    break;
            }
        }
    }
}

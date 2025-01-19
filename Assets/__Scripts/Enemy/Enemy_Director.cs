using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Net;
using System.IO;
using SimpleJSON;

public class DropItemInf
{
    public string name;
    public int percent;
    public int count;
    public Sprite image;
    public DropItemInf(string ItemName, int ItemPercent, int ItemCount, Sprite ItemImage)
    {
        this.name = ItemName;
        this.percent = ItemPercent;
        this.count = ItemCount;
        this.image = ItemImage;
    }
}

public class TempObject
{
    public string monster_name;
}
public class Enemy_Director : MonoBehaviour
{
    // Start is called before the first frame update

    [Header("Enemy Status")]
    public string monsterName;     //이름
    public int damage;     //공격력
    public int defence;    //방어력
    public int hp;         //체력
    public float speed;    //이동속도
    List<DropItemInf> dropItemList = new List<DropItemInf>();    //드랍템
    public int moveArea;      //이동범위

    Vector3 Start_Location;    //생성위치
    public bool isLook;
    bool isMove;
    bool isAttacked;
    bool isOut;
    bool isPuase;
    int direction;
    float moveTime;
    Vector3 defaultScale;
    Animator animator;
    IEnumerator coroutine;

    Rigidbody2D rb;
    public GameObject dropPrefab;

    void Start()
    {
        isLook = false;
        isMove = false;
        isOut = false;
        isPuase = true;
        isAttacked = false;
        defaultScale = transform.localScale;
        animator = GetComponent<Animator>();
        rb = gameObject.GetComponent<Rigidbody2D>();
        Start_Location = transform.parent.position;

        coroutine = Stop(1f);
        StartCoroutine(coroutine);   //생성 시 자연스럽게 나타나는 효과
        StartCoroutine(Fadein(1f)); //생성 시 자연스럽게 나타나는 효과
        StartCoroutine(GetDropItems(monsterName));
    }
    void Update()
    {

        if (isPuase)
            return;

        if (!isMove)
        {
            moveTime = Random.Range(1.0f, 2.0f);
            if (Start_Location.x + moveArea <= transform.position.x)
            {
                direction = -1;  //방향
            }
            else if (Start_Location.x - moveArea >= transform.position.x)
            {
                direction = 1;  //방향
            }
            else
            {
                if (Random.value > 0.5f) direction = 1; else direction = -1;
            }
            if (Random.value > 0.25f)
            {
                coroutine = Move(moveTime, direction);
            }
            else
            {
                coroutine = Stop(moveTime);
            }
            StartCoroutine(coroutine);
        }
        if (isLook)
        {
            StopCoroutine(coroutine);
        }
        if (hp <= 0)
        {
            StopCoroutine(coroutine);
            DropItem();
            StartCoroutine(Death());
        }
    }

    public IEnumerator GetDropItems(string monster_name)
    {
        string url = "http://54.180.217.191:8000/v1/update-items/?" + "monster_name=" + monster_name;
        TempObject item = new TempObject();
        item.monster_name = monster_name;

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
        string requestResult = reader.ReadToEnd(); // user: {}
        JSONNode result = JSON.Parse(requestResult);
        Debug.Log("결과 msg2(Message):");
        Debug.Log("result");
        var datas = result;
        Debug.Log(datas[0]);


        for (int i = 0; i < datas.Count; i++)
        {
            var data = datas[i];
            yield return StartCoroutine(SetSprite(data));
        //https://dorestory.s3.ap-northeast-2.amazonaws.com/static/item_images/resized_zQddhYPMQV_161812242669.png
        //    이미지 url
        }
    }
    IEnumerator SetSprite(JSONNode Data)
    {
        //Debug.Log("SetSprite function start!!");
        Sprite urlItemImage;
        using (WWW www = new WWW(Data["item_image"]))
        //using (WWW www = new WWW(Data["image"]))
        {
            yield return www;
            Texture2D tex = new Texture2D(30, 30, TextureFormat.DXT1, false);
            www.LoadImageIntoTexture(tex);
            urlItemImage = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one * 0.5f);
        }
        Debug.Log("==================Enemy_Director SetSprite function end======================");
        var itemInf = new DropItemInf(Data["item_name"], Data["percent"], Data["item_count"], urlItemImage);
        Debug.Log(itemInf.image.name);
        dropItemList.Add(itemInf);
        //Post_Item item = new Post_Item(Data["id"], 3);
    }
   
    public IEnumerator FollowMove(GameObject player)
    {
        isMove = true;
        isLook = true;
        animator.SetBool("isMoved", true);
        while (true)
        {
            yield return null;
            if (hp <= 0)
            {
                break;
            }
            if (isAttacked == true)
                yield return new WaitForSeconds(0.5f);
            int Flip = 1;
            Vector3 heading = player.transform.position - transform.position;
            if (heading.x > 0)
                Flip = 1;
            else
                Flip = -1;

            transform.localScale = new Vector3(Flip, 1, 1); 
            transform.Translate(new Vector3(speed * Time.deltaTime * Flip, 0.0f, 0.0f));
            if (isOut)
            {
                Vector3 scale = transform.localScale;
                scale.x = scale.x * -1;
                transform.localScale = scale;
                break;
            }
        }
    }

    public IEnumerator UnFollowMove(float time)
    {
        isOut = true;
        isPuase = true;
        gameObject.tag = "Spawn";
        float count = 0;
        Vector3 wasPos = transform.position;
        Vector3 toPos = new Vector3(transform.parent.position.x, transform.position.y, transform.position.z);
        while (true)
        {
            count += Time.deltaTime;
            transform.position = Vector3.Lerp(wasPos, toPos, count);

            if (count >= time)
            {
                transform.position = toPos;
                break;
            }
            yield return null;
        }
        animator.SetBool("isMoved", false);
        gameObject.tag = "Enemy";
        isLook = false;
        isOut = false;
        isMove = false;
        isPuase = false;
    }

    IEnumerator Move(float Maxtime, int Filp)
    {
        isMove = true;
        float time = 0;
        transform.localScale = Vector3.Scale(defaultScale, new Vector3(Filp, 1, 1));
        animator.SetBool("isMoved", true);
        while (time < Maxtime)
        {
            //Debug.Log(time + " / " + Maxtime);
            //yield return new WaitForSeconds(0.1f);
            yield return null;
            transform.Translate(new Vector3(speed * Time.deltaTime * Filp, 0.0f, 0.0f));
            time += Time.deltaTime;
            if (isAttacked == true)
            {
                yield return new WaitForSeconds(0.5f);
                break;
            }
        }
        animator.SetBool("isMoved", false);
        isMove = false;
    }
    IEnumerator Stop(float Maxtime)
    {
        isMove = true;
        float time = 0;
        while (time <= Maxtime)
        {
            yield return null;
            time += Time.deltaTime;
            if (isAttacked == true)
            {
                yield return new WaitForSeconds(0.5f);
                break;
            }
        }
        isMove = false;
    }
    IEnumerator Fadein(float Maxtime)
    {
        gameObject.tag = "Spawn";
        float time = 0;
        SpriteRenderer spr = GetComponent<SpriteRenderer>();
        Color color = spr.color;
        while (time <= Maxtime)
        {
            yield return null;
            time += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1f, time/Maxtime);
            spr.color = color;
        }
        gameObject.tag = "Enemy";
        isPuase = false;
    }
    IEnumerator Death()
    {
        isPuase = true;
        gameObject.tag = "Spawn";
        transform.localScale = Vector3.Scale(defaultScale, new Vector3(1, 0.1f, 1));
        yield return new WaitForSeconds(1f);
        Destroy(transform.parent.gameObject);
    }

    IEnumerator Attacked(float Maxtime, int Filp)
    {
        isAttacked = true;
        transform.localScale = Vector3.Scale(defaultScale, new Vector3(Filp, 1, 1));
        animator.SetBool("isAttacked", true);
        rb.velocity = new Vector2(Filp * -10, 0);
        yield return new WaitForSeconds(Maxtime);
        rb.velocity = new Vector2(0, 0);
        animator.SetBool("isAttacked", false);
        isAttacked = false;
    }
    
    void DropItem()
    {
        foreach (var dropIteminf in dropItemList) {
            if (dropIteminf.percent < Random.Range(1, 101))
                continue;
            GameObject dropItem = Instantiate(dropPrefab, transform.position, Quaternion.Euler(Vector3.zero));
            dropItem.GetComponent<Drop_Inf>().Set(dropIteminf.name, dropIteminf.count);
            dropItem.GetComponent<SpriteRenderer>().sprite = dropIteminf.image;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Attack" && isAttacked == false && isPuase == false)
        {
            hp -= other.GetComponent<Attack_Inf>().Damage;

            int Flip = 1;

            if (other.transform.parent.position.x <= transform.position.x)
                Flip = -1;
            else
                Flip = 1;


            if (hp > 0)
                StartCoroutine(Attacked(0.1f, Flip));
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(collision.gameObject.name);
    }
}
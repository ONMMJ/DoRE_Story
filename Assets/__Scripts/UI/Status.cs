using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Status : MonoBehaviour
{
    Player_Director player;
    Animator animator;

    [Header("Stat Text")]
    public Text statPoint;
    public Text hp;
    public Text damage;
    public Text defence;
    public Text Speed;
    public Text jumpCount;
    public Text DashCount;

    List<string> recycleTypes = new List<string> {"페트","공병","비닐","종이","플라스틱","캔"};

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player_Director>();
        animator = GetComponent<Animator>();

        setText();
    }

    // Update is called once per frame
    void Update()
    {
        if (animator.GetBool("Open") == false)    //인벤 닫혔을 때
        {
            if (Input.GetKeyDown("k"))
            {
                animator.SetBool("Open", true);
            }
        }
        else                                            //인벤 열렸을 때
        {
            if (Input.GetKeyDown("k") || Input.GetKeyDown(KeyCode.Escape))
            {
                animator.SetBool("Open", false);
            }
        }
    }

    void setText()
    {
        hp.text = player.maxHp.ToString();
        damage.text = player.damage.ToString();
        defence.text = player.defence.ToString();
        Speed.text = (player.movePower * 100).ToString() + "%";
        jumpCount.text = player.maxJumpCount.ToString() + "번";
        DashCount.text = player.maxDashCount.ToString() + "번";
    }

    public void AddStatPoint(string type,int point)
    {
        if (recycleTypes.Contains(type))
        {
            statPoint.text = (int.Parse(statPoint.text) + point).ToString();
        }
        else if(type == "폐기물")
        {
            statPoint.text = (int.Parse(statPoint.text) + point * 50).ToString();
        }
        else if(type == "잘못된분리수거")
        {
            statPoint.text = (int.Parse(statPoint.text) - point * 50).ToString();
        }
    }

    public void UpStatButton(string indexAndCount)    //hp, damage, defence
    {
        string[] IAC = indexAndCount.Split(',');
        int sp = int.Parse(statPoint.text);
        int statIndex = int.Parse(IAC[0]);
        int count = int.Parse(IAC[1]);
        if (sp < 0)
            return;
        if (count > sp)
        {
            count = sp;
        }
        switch (statIndex)
        {
            case 0:
                hp.text = (int.Parse(hp.text) + count).ToString();
                player.maxHp += count;
                player.SetHp();
                break;
            case 1:
                damage.text = (int.Parse(damage.text) + count).ToString();
                player.damage += count;
                break;
            case 2:
                defence.text = (int.Parse(defence.text) + count).ToString();
                player.defence += count;
                break;
        }
        sp -= count;
        statPoint.text = sp.ToString();
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Money : MonoBehaviour
{
    public Text coin;
    public Text upgradeStone;
    Player_Director player;

    List<string> normalTypes = new List<string> {"일반", "음식"};
    void Start()
    {
        player = FindObjectOfType<Player_Director>();
        //db에서 재화 값 받아오는 함수 추가 필요
    }
    void setText()
    {
        coin.text = player.coin.ToString();
        upgradeStone.text = player.upgradeStone.ToString();
    }

    public void AddMoney(string type, int point)
    {
        if (normalTypes.Contains(type))
        {
            coin.text = (int.Parse(coin.text) + point*100).ToString();
        }
        else if (type == "배터리" || type == "불연성")
        {
            upgradeStone.text = (int.Parse(upgradeStone.text) + point*5000 * 50).ToString();
        }
    }
}

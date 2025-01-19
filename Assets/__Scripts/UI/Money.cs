using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Money : MonoBehaviour
{
    public Text coin;
    public Text upgradeStone;
    Player_Director player;

    List<string> normalTypes = new List<string> {"�Ϲ�", "����"};
    void Start()
    {
        player = FindObjectOfType<Player_Director>();
        //db���� ��ȭ �� �޾ƿ��� �Լ� �߰� �ʿ�
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
        else if (type == "���͸�" || type == "�ҿ���")
        {
            upgradeStone.text = (int.Parse(upgradeStone.text) + point*5000 * 50).ToString();
        }
    }
}

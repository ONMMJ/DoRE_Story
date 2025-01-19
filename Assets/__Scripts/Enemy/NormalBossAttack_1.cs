using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalBossAttack_1 : MonoBehaviour
{
    public SpriteRenderer WarningArea;
    public GameObject Attack;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(AttackStart());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator AttackStart()
    {
        Color color;
        Vector3 pos1 = FindObjectOfType<Player_Director>().transform.position;
        pos1.y = -7.8f;
        transform.position = pos1;
        Attack.SetActive(false);
        for (int i = 0; i<6;i++)
        {
            yield return new WaitForSeconds(0.1f);
            color = WarningArea.color;
            color.a = 0.2f;
            WarningArea.color = color;
            yield return new WaitForSeconds(0.1f);
            color = WarningArea.color;
            color.a = 0;
            WarningArea.color = color;
        }
        Attack.SetActive(true);
        Vector3 pos = Attack.transform.position;
        yield return StartCoroutine(MoveTo(Attack, transform.position, 0.2f));
        yield return StartCoroutine(MoveTo(Attack, pos, 1f));
        Destroy(gameObject, 0.3f);
    }
    IEnumerator MoveTo(GameObject a, Vector3 toPos, float time)
    {
        float count = 0;
        Vector3 wasPos = a.transform.position;
        while (true)
        {
            count += Time.deltaTime;
            a.transform.position = Vector3.Lerp(wasPos, toPos, count/time);

            if (count >= time)
            {
                a.transform.position = toPos;
                break;
            }
            yield return null;
        }
    }
}

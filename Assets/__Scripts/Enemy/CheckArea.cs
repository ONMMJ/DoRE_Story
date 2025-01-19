using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckArea : MonoBehaviour
{
    GameObject enemyBody;
    Enemy_Director enemyBodyDirector;
    // Start is called before the first frame update
    void Start()
    {
        enemyBody = gameObject.transform.parent.Find("EnemyBody").gameObject;
        enemyBodyDirector = enemyBody.GetComponent<Enemy_Director>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = enemyBody.transform.position;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("ok");
            if (enemyBodyDirector.isLook == false)
                enemyBodyDirector.StartCoroutine(enemyBodyDirector.FollowMove(other.gameObject));

        }
    }
}

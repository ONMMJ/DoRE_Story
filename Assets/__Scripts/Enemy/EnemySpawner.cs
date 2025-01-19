using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemy;
    GameObject prefabEnemy;
    GameObject enemyBody;

    float time;
    public float spawnTime = 5;    //적이 스폰되는 시간
    // Start is called before the first frame update
    void Start()
    {
        enemySpawn();
        time = 0;
    }

    // Update is called once per frame
    void Update() { 
    
        if (null != prefabEnemy)
            time = 0;
        else
            time += Time.deltaTime;

        if (time < spawnTime)
            return;

        enemySpawn();
    }
    private void enemySpawn()
    {
        prefabEnemy = Instantiate(enemy, transform.position, Quaternion.Euler(Vector3.zero));
        enemyBody = prefabEnemy.transform.Find("EnemyBody").gameObject;
        prefabEnemy.transform.parent = transform;
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.Equals(enemyBody))
        {
            other.GetComponent<Enemy_Director>().StartCoroutine(other.GetComponent<Enemy_Director>().UnFollowMove(1.5f));
        }
    }
}

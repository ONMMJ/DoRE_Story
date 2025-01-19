using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollider : MonoBehaviour
{
    Transform player;
    // Start is called before the first frame update
    void Start()
    {
        player = transform.parent;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "Enemy")
            player.GetComponent<Player_Director>().Attacked(other.GetComponent<Enemy_Director>().damage);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "DropItem")
            collision.GetComponent<Drop_Inf>().PickUpItem();
    }
}

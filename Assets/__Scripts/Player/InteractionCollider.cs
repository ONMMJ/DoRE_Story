using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionCollider : MonoBehaviour
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Interaction")
            player.GetComponent<Player_Director>().interaction = collision.gameObject;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (player.GetComponent<Player_Director>().interaction == collision.gameObject)
            player.GetComponent<Player_Director>().interaction = null;
    }
}

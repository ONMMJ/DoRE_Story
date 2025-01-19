using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCollider : MonoBehaviour
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Ground" || other.tag == "MovingGround")
            player.GetComponent<Player_Director>().LandGround();
        if (other.tag == "MovingGround")
            player.GetComponent<Player_Director>().MovingGround(true, other.transform);
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "MovingGround")
            player.GetComponent<Player_Director>().MovingGround(false);
    }
}

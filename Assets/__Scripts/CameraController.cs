using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    GameObject player;
    [Header("Max Area")]
    public float left;
    public float right;
    public float up;
    public float down;
    [Header("Player Area")]
    public float xArea = 5;
    public float yArea = 3;
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player_Director>().gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 cameraPos = transform.position;
        float x, y;
        x = cameraPos.x;
        y = cameraPos.y;
        if (cameraPos.x - player.transform.position.x >= xArea)
            x = player.transform.position.x + xArea;
        if (cameraPos.x - player.transform.position.x <= -xArea)
            x = player.transform.position.x - xArea;
        if (cameraPos.y - player.transform.position.y >= yArea)
            y = player.transform.position.y + yArea;
        if (cameraPos.y - player.transform.position.y <= -yArea)
            y = player.transform.position.y - yArea;

        if (x <= left)
            x = left;
        if (x >= right)
            x = right;
        if (y <= down)
            y = down;
        if (y >= up)
            y = up;

        transform.position = new Vector3(x, y, cameraPos.z);
    }
}

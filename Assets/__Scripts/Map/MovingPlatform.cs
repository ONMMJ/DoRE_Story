using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Transform desPos;
    public Transform endPos;
    public Transform startPos;

    public float speed;

    // Start is called before the first frame update
    void Start()
    {
        GameObject startPosO = new GameObject("startPos");
        startPos = startPosO.transform;
        startPos.parent = transform.parent;
        startPos.position = transform.position;
        desPos = endPos;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = Vector2.MoveTowards(transform.position, desPos.position,Time.deltaTime*speed);
        
        if(Vector2.Distance(transform.position ,desPos.position)<=0.05f)
        {
            if (desPos == endPos) desPos = startPos;
            else desPos = endPos;
        }
    }
}

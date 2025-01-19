using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObject : MonoBehaviour
{
    [SerializeField] private float leftCap = -0;
    [SerializeField] private float rightCap = -0;

    protected enum Direction { Right, Up, Left }
    protected Direction direction = Direction.Left;

    private BoxCollider2D coll;

    // Inspector variables 
    private LayerMask player;

    // Start is called before the first frame update
    void Start()
    {
        coll = GetComponent<BoxCollider2D>();
        player = LayerMask.GetMask("Player");
    }

    // Update is called once per frame
    void Update()
    {
        direction = MoveDirection(transform.position.x, leftCap, rightCap, direction);   // 패트롤 하는 위치에 따라 방향 조정 

        if (direction == Direction.Left)
        {
            MoveObject(Vector3.left * Time.deltaTime);
        }
        else if (direction == Direction.Right)
        {
            MoveObject(Vector3.right * Time.deltaTime);
        }
        else
        { }
    }

    void MoveObject(Vector3 delta)
    {
        transform.Translate(delta);
        if (coll.IsTouchingLayers(player))
        {
            GameObject.FindGameObjectWithTag("Player").transform.Translate(delta);
        }
    }

    Direction MoveDirection(float x, float left, float right, Direction dir)
    {
        Direction output = dir;

        if (x < left) output = Direction.Right;
        if (x > right) output = Direction.Left;

        return output;
    }
}
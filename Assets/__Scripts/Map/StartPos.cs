using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartPos : MonoBehaviour
{
    // Start is called before the first frame update
    Transform target;
    void Start()
    {
        target = gameObject.transform.parent.Find("Target");
        transform.position = target.position;
    }

    // Update is called once per frame
    void Update()
    {
    }
}

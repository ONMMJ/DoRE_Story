using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map_Director : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(new Vector3(1 * Time.deltaTime, 0, 0));
    }
}

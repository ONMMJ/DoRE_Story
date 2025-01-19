using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack_Inf : MonoBehaviour
{
    public int Damage = 3;
    // Start is called before the first frame update
    void Start()
    {
            Destroy(gameObject, 0.2f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Enemy")
            Destroy(gameObject);
    }
}

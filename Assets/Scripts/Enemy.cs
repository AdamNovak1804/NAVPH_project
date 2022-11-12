using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float health = 3f;

    public float damage = 0f;

    public float armor = 0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void takeDamage(float value) 
    {
        health -= (value - armor);

        if (health <= 0f) 
        {
            // Process death
            this.gameObject.SetActive(false);
            Object.Destroy(this.gameObject);
        }
    }
}

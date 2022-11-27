using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float health = 3f;

    public float damage = 0f;

    public float armor = 0f;

    public float dyingTime = 0.5f;

    public GameObject target;

    public bool isTargeted;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0f) 
        {
            dyingTime -= Time.deltaTime;
            if (dyingTime <= 0f) 
            {
                // Process death
                this.gameObject.SetActive(false);
                Object.Destroy(this.gameObject);
            }
        }
    }

    public void TakeDamage(float value) 
    {
        health -= (value - armor);
    }

    public void MakeTargeted() 
    {
        
    }

    public void MakeUntargeted() 
    {

    }
}

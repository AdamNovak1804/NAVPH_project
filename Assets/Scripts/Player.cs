using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float damage = 0f;
    private float health = 5f;
    private int score = 0;
    private int ammo = 0;
    //private string powerUp = "none";
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddScore(int value) 
    {
        score += value;
        Debug.LogFormat("Current score is {0}", score);
    }

    public void AddAmmo(int value) 
    {
        score += value;
    }

    public void UseAmmo() 
    {
        score -= 1;
    }

    public void DrainHealth(float value) 
    {
        health -= value;

        if (health < 0f) 
        {
            Debug.Log("Player Dies");
        }
    }

    public void AddHealth(float value) 
    {
        health += value;
    }

    public float GetDamage() 
    {
        return damage;
    }
}

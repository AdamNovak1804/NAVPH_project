using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float damage = 0f;
    public float maxHealth = 7f;
    public float currentHealth = 5f;
    public int score = 0;
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
        ammo += value;
        Debug.LogFormat("Current ammo is {0}", ammo);
    }

    public void AddHealth(float value) 
    {
        if (currentHealth + value > maxHealth) {
            currentHealth = maxHealth;
        } else 
        {
            currentHealth += value;
        }
        Debug.LogFormat("Current health is {0}", currentHealth);
    }

    public void UseAmmo() 
    {
        ammo -= 1;
    }

    public void DrainHealth(float value) 
    {
        currentHealth -= value;

        if (currentHealth < 0f) 
        {
            Debug.Log("Player Dies");
        }
    }

    public float GetDamage() 
    {
        return damage;
    }

    public float GetHealth() 
    {
        return currentHealth;
    }

    public float GetAmmo() 
    {
        return ammo;
    }
}

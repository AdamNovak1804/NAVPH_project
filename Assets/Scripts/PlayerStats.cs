using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

public class PlayerStats : MonoBehaviour
{

    public delegate void UpdateHealthBar(float amount);
    public static event UpdateHealthBar UpdateHealth;

    public delegate void UpdateAmmoCount(int ammo);
    public static event UpdateAmmoCount UpdateAmmo;

    public delegate void UpdateScoreCount(int score);
    public static event UpdateScoreCount UpdateScore;

    public float damage = 0f;
    public float maxHealth = 7f;
    public float currentHealth = 5f;
    public int score = 0;
    public int ammo = 5;
    public float time = 0f;
    public int enemiesKilled = 0;

    public int maxScore;
    public int maxEnemies;

    private bool hudInitialized = false;

    private PowerUps powerUps;

    //private string powerUp = "none";
    
    // Start is called before the first frame update
    void Start()
    {
        maxScore = GameObject.FindGameObjectsWithTag("Coin").ToList().Count();
        maxEnemies = GameObject.FindGameObjectsWithTag("Enemy").ToList().Count();
        
        Enemy.UpdateKilledEnemies += AddKilledEnemy;
        powerUps = GetComponent<PowerUps>();

        
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;

        
        if (UpdateHealth != null && !hudInitialized)
        {
            UpdateHealth(currentHealth / maxHealth * 100);
            UpdateAmmo(ammo);
            UpdateScore(score);
            hudInitialized = true;
        }
    }

    public void AddKilledEnemy()
    {
        enemiesKilled++;
    }
    public void AddScore(int value) 
    {
        score += value;
        UpdateScore(score);
        Debug.LogFormat("Current score is {0}", score);
    }

    public void AddAmmo(int value) 
    {
        ammo += value;
        UpdateAmmo(ammo);
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
        UpdateHealth(currentHealth / maxHealth * 100);
        Debug.LogFormat("Current health is {0}", currentHealth);
    }


    public void UseAmmo() 
    {
        ammo -= 1;
        //UpdateAmmo(ammo);
    }

    public void DrainHealth(float value) 
    {
        if (powerUps.isPowerUpActive && powerUps.activePowerUpName == "GodArmor") 
        {
            powerUps.DeactivatePowerUp();
        } 
        currentHealth -= value;

        UpdateHealth(currentHealth / maxHealth * 100);

        Debug.Log(currentHealth);

        if (currentHealth < 0f) 
        {
            Debug.Log("PlayerStats Dies");
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

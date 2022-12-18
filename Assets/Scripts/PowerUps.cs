using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class PowerUps : MonoBehaviour
{
    public bool isPowerUpActive = false;
    public CollectiblesScript.PowerUpType activePowerUpName;

    public float activePeriod = 0.0f;
    public float duration = 8.0f;

    public float speedPowerup = 8.0f;
    private PlayerStats player;
    private AudioManager audioManager;

    public delegate void DoubleJump(bool enabled, float length);
    public static event DoubleJump UpdateDoubleJump;

    public delegate void Speed(bool enabled, float length, float newSpeed);
    public static event Speed UpdateSpeed;

    public delegate void GodArmor(bool enabled, float length);
    public static event GodArmor UpdateGodArmor;

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<PlayerStats>();
        audioManager = FindObjectOfType<AudioManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isPowerUpActive) 
        {
            activePeriod -= Time.deltaTime;
            if (activePeriod < 0.0f) 
            {
                DeactivatePowerUp();
            }
        }
    }

    private CollectiblesScript.PowerUpType ChoosePowerUp() 
    {
        Random random = new Random(Mathf.RoundToInt(Time.time));
        Array values = Enum.GetValues(typeof(CollectiblesScript.PowerUpType));
        return (CollectiblesScript.PowerUpType)values.GetValue(random.Next(values.Length));
    }

    public void ActivatePowerUp(CollectiblesScript.PowerUpType name) 
    {
        activePeriod = duration;
        
        if (name == CollectiblesScript.PowerUpType.Random)
        {
            name = ChoosePowerUp();
        }
        isPowerUpActive = true;
        activePowerUpName = name;
        if (activePowerUpName == CollectiblesScript.PowerUpType.DoubleJump) 
        {
            UpdateDoubleJump(true, duration);
        }
        if (activePowerUpName == CollectiblesScript.PowerUpType.Speed)
        {
            UpdateSpeed(true, duration, speedPowerup);
        }

        if (activePowerUpName == CollectiblesScript.PowerUpType.GodArmor)
        {
            UpdateGodArmor(true, duration);
        }
        audioManager.Play("PowerUpEnabled");
    }

    public void DeactivatePowerUp()
    {
        isPowerUpActive = false;
        if (activePowerUpName == CollectiblesScript.PowerUpType.DoubleJump)
        {
            UpdateDoubleJump(false, duration);
        }
        if (activePowerUpName == CollectiblesScript.PowerUpType.Speed)
        {
            UpdateSpeed(false, duration, 0f);
        }
        if (activePowerUpName == CollectiblesScript.PowerUpType.GodArmor)
        {
            UpdateGodArmor(false, duration);
        }
        audioManager.Play("PowerUpDisabled");
        activePowerUpName = CollectiblesScript.PowerUpType.Random;
    }
}

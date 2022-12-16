using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUps : MonoBehaviour
{
    public bool isPowerUpActive = false;
    public string activePowerUpName = "";
    private string[] powerUps = {"DoubleJump", "GodArmor", "Speed"};

    public float activePeriod = 0.0f;
    public float duration = 8.0f;

    public float speedPowerup = 8.0f;
    private Player player;
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
        player = GetComponent<Player>();
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

    private string ChoosePowerUp() 
    {
        var index = Random.Range(0,2);
        return powerUps[index];
    }

    public void ActivatePowerUp() 
    {
        activePeriod = duration;
        
        string s = "DoubleJump";
        Debug.Log(s);
        isPowerUpActive = true;
        activePowerUpName = s;
        if (activePowerUpName == "DoubleJump") 
        {
            UpdateDoubleJump(true, duration);
        }
        if (activePowerUpName == "Speed")
        {
            UpdateSpeed(true, duration, speedPowerup);
        }

        if (activePowerUpName == "GodMode")
        {
            UpdateGodArmor(true, duration);
        }
        audioManager.Play("PowerUpEnabled");
        Debug.Log(s + "is active now...");
    }

    public void DeactivatePowerUp()
    {
        isPowerUpActive = false;
        if (activePowerUpName == "DoubleJump")
        {
            UpdateDoubleJump(false, duration);
        }
        if (activePowerUpName == "Speed")
        {
            UpdateSpeed(false, duration, 0f);
        }
        if (activePowerUpName == "GodMode")
        {
            UpdateGodArmor(false, duration);
        }
        audioManager.Play("PowerUpDisabled");
        Debug.Log(activePowerUpName + "is not active anymore...");
        activePowerUpName = "";
    }
}

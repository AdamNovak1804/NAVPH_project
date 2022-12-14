using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUps : MonoBehaviour
{
    public bool isPowerUpActive = false;
    public string activePowerUpName = "";
    private string[] powerUps = {"DoubleJump", "GodArmor"};
    private float activePeriod = 0.0f;
    private PlayerController playerController;
    private AudioManager audioManager;
    // Start is called before the first frame update
    void Start()
    {
        playerController = GetComponent<PlayerController>();
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
        activePeriod = 8f;
        string s = ChoosePowerUp();
        isPowerUpActive = true;
        activePowerUpName = s;
        if (activePowerUpName == "DoubleJump") 
        {
            playerController.DoubleJumpEnabled(true);
        }
        audioManager.Play("PowerUpEnabled");
        Debug.Log(s + "is active now...");
    }

    public void DeactivatePowerUp() 
    {
        isPowerUpActive = false;
        if (activePowerUpName == "DoubleJump") 
        {
            playerController.DoubleJumpEnabled(false);
        }
        audioManager.Play("PowerUpDisabled");
        Debug.Log(activePowerUpName + "is not active anymore...");
        activePowerUpName = "";
    }
}

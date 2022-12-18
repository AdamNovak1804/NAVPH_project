using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectiblesScript : MonoBehaviour
{
// Start is called before the first frame update
    public int scoreAddition = 0;
    public int ammoAddition = 0;
    public int healthAddition = 0;
    public float timeToBecomeCollectible = 0f;
    public bool isPowerUp;
    public PowerUpType powerUpName = PowerUpType.Random;
    private bool isCollectible = false;
    private AudioManager audioManager;

    public enum PowerUpType
    {
        Random,
        DoubleJump,
        GodArmor,
        Speed
    }
    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
    }
    // Update is called once per frame
    void Update()
    {
        if (timeToBecomeCollectible <= 0f) 
        {
            isCollectible = true;
        }
        else {
            timeToBecomeCollectible -= Time.deltaTime;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (isCollectible && other.transform.name == "Astronaut") 
        {
            var player = (PlayerStats) other.transform.gameObject.GetComponent(typeof(PlayerStats));
            if (scoreAddition > 0) 
            {
                player.AddScore(scoreAddition);
                audioManager.Play("CoinPickup");
            }
            if (ammoAddition > 0) 
            {
                player.AddAmmo(ammoAddition);
                audioManager.Play("AmmoPickup");
            }
            if (healthAddition > 0) 
            {
                player.AddHealth(healthAddition);
                audioManager.Play("HealthPickup");
            }
            if (isPowerUp) 
            {
                var powerUps = (PowerUps) other.transform.gameObject.GetComponent(typeof(PowerUps));
                powerUps.ActivatePowerUp(powerUpName);
            }
            this.gameObject.SetActive(false);
            Object.Destroy(this.gameObject);
        }
    }
}

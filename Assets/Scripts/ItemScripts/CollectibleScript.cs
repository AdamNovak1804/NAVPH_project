using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleScript : MonoBehaviour
{
    // Start is called before the first frame update
    public int scoreAddition = 0;
    public int ammoAddition = 0;
    public int healthAddition = 0;

    public float timeToBecomeCollectible = 0f;

    private bool isCollectible = false;

    private AudioManager audioManager;

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

    private void OnCollisionEnter(Collision other) 
    {
        if (isCollectible && other.transform.name == "Astronaut") 
        {
            var player = (Player) other.transform.gameObject.GetComponent(typeof(Player));
            player.AddScore(scoreAddition);
            if (scoreAddition > 0) 
            {
                audioManager.Play("CoinPickup");
            }
            player.AddAmmo(ammoAddition);
            if (ammoAddition > 0) 
            {
                audioManager.Play("AmmoPickup");
            }
            player.AddHealth(healthAddition);
            if (healthAddition > 0) 
            {
                audioManager.Play("HealthPickup");
            }
            this.gameObject.SetActive(false);
            Object.Destroy(this.gameObject);
        }
    }
}

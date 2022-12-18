using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static PlayerStats;

public class HUDBossBattle : MonoBehaviour
{
    public Slider healthBarAmount = null;
    public Text ammoCount = null;
    // Start is called before the first frame update
    void Start()
    {
        healthBarAmount.value = 100;

        PlayerStats.UpdateHealth += UpdateHealthBar;
        PlayerStats.UpdateAmmo += UpdateAmmo;
    }

    void OnDisable()
    {
        PlayerStats.UpdateHealth -= UpdateHealthBar;
        PlayerStats.UpdateAmmo -= UpdateAmmo;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void UpdateHealthBar(float value)
    {
        healthBarAmount.value = value;
    }

    void UpdateAmmo(int value)
    {
        ammoCount.text = value.ToString();
    }
}


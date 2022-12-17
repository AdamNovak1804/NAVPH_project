using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using static System.Net.Mime.MediaTypeNames;
using Text = UnityEngine.UI.Text;
using System.Runtime.InteropServices;
using Image = UnityEngine.UI.Image;
using UnityEngine.SceneManagement;

public class HUDController : MonoBehaviour
{
    public Slider healthBarAmount = null;

    public Text score = null;
    public Text ammoCount = null;


    public Text text = null;
    public float elapsedTime = 0;

    public GameObject powerUp;
    private float powerUpDuration;
    public float currDuration;
    public Image powerupSprite;
    public Slider powerUpBar = null;

    public Sprite godArmor;
    public Sprite speed;
    public Sprite doubleJump;

    void Start()
    {

        healthBarAmount.value = 100;

        Player.UpdateHealth += UpdateHealthBar;
        Player.UpdateAmmo += UpdateAmmo;
        Player.UpdateScore += UpdateScore;

        PowerUps.UpdateDoubleJump += UpdateDoubleJump;
        PowerUps.UpdateSpeed += UpdateSpeed;
        PowerUps.UpdateGodArmor += UpdateGodArmor;

        powerUp.SetActive(false);
    }

    void OnDisable()
    {
        Player.UpdateHealth -= UpdateHealthBar;
        Player.UpdateAmmo -= UpdateAmmo;
        Player.UpdateScore -= UpdateScore;

        PowerUps.UpdateDoubleJump -= UpdateDoubleJump;
        PowerUps.UpdateSpeed -= UpdateSpeed;
        PowerUps.UpdateGodArmor -= UpdateGodArmor;
    }

    // Update is called once per frame
    void Update()
    {
        if (powerUp.activeSelf)
        {
            currDuration -= Time.deltaTime;

            powerUpBar.value = currDuration / powerUpDuration * 100;
        }

        elapsedTime += Time.deltaTime;
        text.text = timeToStr(elapsedTime);
    }

    void UpdateHealthBar(float value)
    {
        healthBarAmount.value = value;
    }

    void UpdateAmmo(int value)
    {
        ammoCount.text = value.ToString();
    }

    void UpdateScore(int value)
    {
        score.text = value.ToString();
    }

    void UpdateSpeed(bool enabled, float duration, float maxSpeed)
    {
        if (enabled)
            ShowSprite("speed", duration);
        powerUp.SetActive(enabled);
    }
    void UpdateDoubleJump(bool enabled, float duration)
    {
        if (enabled)
            ShowSprite("doubleJump", duration);
        powerUp.SetActive(enabled);
    }
    void UpdateGodArmor(bool enabled, float duration)
    {
        if (enabled)
            ShowSprite("godArmor", duration);
        powerUp.SetActive(enabled);
    }

    void ShowSprite(string name, float duration)
    {
        powerUpDuration = duration;
        currDuration = duration;

        if (name == "speed")
        {
            powerupSprite.sprite = speed;
        }
        if (name == "doubleJump")
        {
            powerupSprite.sprite = doubleJump;
        }
        if (name == "godArmor")
        {
            powerupSprite.sprite = godArmor;
        }
    }

    string timeToStr(float time)
    {
        string res;

        float secs = (time % 60);
        int mins = (int)(time / 60);

        res = mins.ToString("00") + ":" + secs.ToString("00.00");
        return res;
    }
}

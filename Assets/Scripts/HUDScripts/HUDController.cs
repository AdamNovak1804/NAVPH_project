using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using static System.Net.Mime.MediaTypeNames;
using Text = UnityEngine.UI.Text;

public class HUDController : MonoBehaviour
{
    [SerializeField]
    public Slider healthBarAmount = null;

    public Text score = null;
    public Text ammoCount = null;


    public Text text = null;
    public float elapsedTime = 0;

    void Start()
    {

        healthBarAmount.value = 100;

        Player.UpdateHealth += UpdateHealthBar;
        Player.UpdateAmmo += UpdateAmmo;
        Player.UpdateScore += UpdateScore;
    }

    // Update is called once per frame
    void Update()
    {
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

    string timeToStr(float time)
    {
        string res;

        float secs = (time % 60);
        int mins = (int)(time / 60);

        res = mins.ToString("00") + ":" + secs.ToString("00.00");
        return res;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerScript : MonoBehaviour
{
    // Start is called before the first frame update
    public Text text;
    public float elapsedTime = 0;
    void Start()
    {
        this.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;
        text.text = timeToStr(elapsedTime);
    }

    string timeToStr(float time)
    {
        string res;

        float secs = (time % 60);
        int mins = (int)(time/60);

        res = mins.ToString("00") + ":" + secs.ToString("00.00");
        return res;
    }
}

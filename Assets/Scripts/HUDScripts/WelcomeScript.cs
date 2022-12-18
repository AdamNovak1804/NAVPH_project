using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WelcomeScript : MonoBehaviour
{
    // Start is called before the first frame update
    float time = 0f;
    void Start()
    {
        Time.timeScale = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if (Input.anyKeyDown)
        {
            Time.timeScale = 1f;
            this.gameObject.SetActive(false);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleControllerTopToBot : MonoBehaviour
{
    public bool isEnabled;
    private bool goesBot;
    public bool topToBot = false;
    public bool botToTop = false;
    public int limit;
    public int speed;
    private Vector3 topLimit;
    private Vector3 botLimit;

    // Start is called before the first frame update
    void Start()
    {
        goesBot = topToBot;
        topLimit = transform.position;
        botLimit = transform.position;
        if (goesBot)
        {
            botLimit.y -= limit;
        }
        else
        {
            topLimit.y += limit;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (topToBot && isEnabled)
        {
            if (transform.position.y > botLimit.y)
            {
                transform.Translate(new Vector3(0, -1, 0) * speed * Time.deltaTime, Space.World);
            }
            else
            {
                goesBot = !goesBot;
                topToBot = false;
                botToTop = true;
            }
        }
        if (botToTop && isEnabled)
        {
            if (transform.position.y < topLimit.y)
            {
                transform.Translate(new Vector3(0, 1, 0) * speed * Time.deltaTime, Space.World);
            }
            else
            {
                goesBot = !goesBot;
                topToBot = true;
                botToTop = false;
            }
        }
    }
}

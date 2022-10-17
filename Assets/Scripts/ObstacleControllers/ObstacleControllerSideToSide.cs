using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleControllerSideToSide : MonoBehaviour
{
    // This is just a draft concept -we need to see where will be the map headed
    // so we can decide if obstacles move on X or Z axes
    
    public bool isEnabled;
    
    public bool leftToRight = false;
    
    public bool rightToLeft = false;

    public int limit;

    public int speed;

    private bool goesRight;

    private Vector3 leftLimit;

    private Vector3 rightLimit;

    // Start is called before the first frame update
    void Start()
    {
        goesRight = leftToRight;
        leftLimit = transform.position;
        rightLimit = transform.position;
        if (goesRight) {
            rightLimit.x += limit;
        }
        else {
            leftLimit.x -= limit;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (leftToRight && isEnabled) {
            if (transform.position.x < rightLimit.x) {
                transform.Translate(new Vector3(1, 0, 0) * speed * Time.deltaTime, Space.World);
            }
            else {
                goesRight = !goesRight;
                leftToRight = false;
                rightToLeft = true;
            }
        }
        if (rightToLeft && isEnabled) {
            if (transform.position.x > leftLimit.x) {
                transform.Translate(new Vector3(-1, 0, 0) * speed * Time.deltaTime, Space.World);
            }
            else {
                goesRight = !goesRight;
                leftToRight = true;
                rightToLeft = false;
            }
        }
    }
}

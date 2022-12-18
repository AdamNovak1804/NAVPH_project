using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallablePlatformMovement : MonoBehaviour
{
    private bool isDown = false;
    private bool isCrumbling = false;

    private float timeToCrumble = 1f;
    private float timeToWaitDown = 0f;

    // Update is called once per frame
    void Update()
    {
        if (isCrumbling && timeToCrumble > 0f) {
            timeToCrumble -= Time.deltaTime;
        }
        if (timeToCrumble < 0f) {
            isCrumbling = false;
            timeToCrumble = 0f;
            isDown = true;
            timeToWaitDown = 5f;
            moveDown();
        }
        if (isDown && timeToWaitDown > 0f) {
            timeToWaitDown -= Time.deltaTime;
        }
        if (timeToWaitDown < 0f) {
            isDown = false;
            timeToWaitDown = 0f;
            moveUp();
        }
    }

    private void moveUp() 
    {
        transform.position = Vector3.MoveTowards(transform.position, transform.position + new Vector3(0, 5, 0),Time.deltaTime * 5f);
    }

    private void moveDown() 
    {
        transform.position = Vector3.MoveTowards(transform.position, transform.position + new Vector3(0, -5, 0),Time.deltaTime * 5f);
    }

    private void OnCollisionEnter(Collision other) {
        if (!isCrumbling && other.transform.name == "Astronaut" && !isDown) 
        {
            isCrumbling = true;
            timeToCrumble = 1f;
        }
    }
}

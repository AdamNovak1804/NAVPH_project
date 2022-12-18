using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFightCamera : MonoBehaviour
{
    public GameObject target;
    public Transform[] positions;

    private float tmpTime = 10.0f;

    private bool moving = false;
    private Vector3 newPos;
    private Vector3 nextPos;
    private Queue<Transform> positionQueue;

    public void fillPositionQueue()
    {
        positionQueue = new Queue<Transform>();

        foreach(Transform x in positions)
        {
            positionQueue.Enqueue(x.transform);
        }
    }

    public void nextPosition()
    {
        if (positionQueue.Count != 0)
        {
            moving = true;
            nextPos = positionQueue.Dequeue().position;
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        // get list of positions
        fillPositionQueue();

        // go to the first position
        transform.position = positionQueue.Dequeue().position;
    }

    // Update is called once per frame
    void Update()
    {
        if (moving == true)
        {
            newPos = Vector3.Lerp(transform.position, nextPos, Time.deltaTime);
            transform.position = newPos;

            if (transform.position == nextPos)
            {
                moving = false;
            }
        }

        transform.LookAt(target.transform.position);
    }
}

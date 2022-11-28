using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMovement : MonoBehaviour
{
    public bool isEnabled = true;

    public enum StartingPositions
    {
        top,
        left,
        bottom,
        right,
    }

    public enum RotationOptions {
        left,
        right,
    }

    public StartingPositions startingPos = StartingPositions.top;

    public RotationOptions rotationDirection = RotationOptions.left;

    public float space;

    private List<Vector3> positions;

    private int currentIndex = 1;
    private Vector3 nextPos;

    public float speed;
    // Start is called before the first frame update
    void Start()
    {
        positions = new List<Vector3>();
        positions.Add(transform.position);
        float xCoor = space;
        float zCoor = space;
        // This could be shortened, but I decided to keep it this way so it is easier to map
        // platforms to their positions
        if (startingPos == StartingPositions.top) 
        {
            if (rotationDirection == RotationOptions.left) 
            {
                xCoor *= -1;
            }
            positions.Add(transform.position + new Vector3(xCoor, 0, -zCoor));
            positions.Add(transform.position + new Vector3(0, 0, 2 * -zCoor));
            positions.Add(transform.position + new Vector3(-xCoor, 0, -zCoor));
        }
        if (startingPos == StartingPositions.bottom) 
        {
            if (rotationDirection == RotationOptions.left) 
            {
                xCoor *= -1;
            }
            positions.Add(transform.position + new Vector3(xCoor, 0, zCoor));
            positions.Add(transform.position + new Vector3(0, 0, 2 * zCoor));
            positions.Add(transform.position + new Vector3(-xCoor, 0, zCoor));
        }

        if (startingPos == StartingPositions.right) 
        {
            if (rotationDirection == RotationOptions.left) 
            {
                zCoor *= -1;
            }
            positions.Add(transform.position + new Vector3(-xCoor, 0, -zCoor));
            positions.Add(transform.position + new Vector3(2 * -xCoor, 0, 0));
            positions.Add(transform.position + new Vector3(-xCoor, 0, zCoor));
        }

        if (startingPos == StartingPositions.left) 
        {
            if (rotationDirection == RotationOptions.left) 
            {
                zCoor *= -1;
            }
            positions.Add(transform.position + new Vector3(xCoor, 0, zCoor));
            positions.Add(transform.position + new Vector3(2 * xCoor, 0, 0));
            positions.Add(transform.position + new Vector3(xCoor, 0, -zCoor));

        }
        nextPos = positions[currentIndex];
    }

    // Update is called once per frame
    void Update()
    {
        if (isEnabled) 
        {
            if (transform.position != nextPos) 
            {
                transform.position = Vector3.MoveTowards(transform.position, nextPos, speed * Time.deltaTime);
            }
            else {
                if (currentIndex == 3) {
                    currentIndex = 0;
                }
                else {
                    currentIndex += 1;
                }
                nextPos = positions[currentIndex];
            }
        }
    }
}

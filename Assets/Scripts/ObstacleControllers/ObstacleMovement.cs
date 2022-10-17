using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleMovement : MonoBehaviour
{
    // This is just a draft concept -we need to see where will be the map headed
    // so we can decide if obstacles move on X or Z axes

    public bool isEnabled = true;

    public enum Directions
    {
        xDir,
        zDir,
        yDir
    }

    private Dictionary<Directions, Vector3> directions = new Dictionary<Directions, Vector3>()
    {
        {Directions.xDir, new Vector3( 1,  0,  0)},
        {Directions.zDir, new Vector3( 0,  0,  1)},
        {Directions.yDir, new Vector3( 0,  1,  0)}
    };

    public Directions direction;

    public float start;
    public float end;
    public float speed;
    public float offset = 0.1f;

    private float dirSpeed;
    private Vector3 movement;
    private Vector3 startVec;
    private Vector3 endVec;

    // Start is called before the first frame update
    void Start()
    {
        dirSpeed = speed;
        movement = directions[direction];
        startVec = transform.position + (movement * start);
        endVec = transform.position + (movement * end);
    }

    // Update is called once per frame
    void Update()
    {
        if (isEnabled)
        {
            switch(direction)
            {
                case Directions.xDir:
                    if (transform.position.x > startVec.x && transform.position.x < endVec.x)
                        transform.Translate(movement * dirSpeed * Time.deltaTime, Space.World);
                    else
                    {
                        dirSpeed *= -1;
                        transform.Translate(movement * dirSpeed * Time.deltaTime, Space.World);
                    }
                    break;
                case Directions.zDir:
                    if (transform.position.z > startVec.z && transform.position.z < endVec.z)
                        transform.Translate(movement * dirSpeed * Time.deltaTime, Space.World);
                    else
                    {
                        dirSpeed *= -1;
                        transform.Translate(movement * dirSpeed * Time.deltaTime, Space.World);
                    }
                    break;
                case Directions.yDir:
                    if (transform.position.y > startVec.y && transform.position.y < endVec.y)
                        transform.Translate(movement * dirSpeed * Time.deltaTime, Space.World);
                    else
                    {
                        dirSpeed *= -1;
                        transform.Translate(movement * dirSpeed * Time.deltaTime, Space.World);
                    }
                    break;
                default:
                    Debug.LogError("Invalid direction given");
                    break;
            }
        }
    }
}
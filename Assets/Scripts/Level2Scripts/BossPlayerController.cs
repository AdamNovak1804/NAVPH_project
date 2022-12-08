using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPlayerController : MonoBehaviour
{
    public int health = 3;
    public int ammo = 0;

    public int maxJumpCount = 2;
    public float speed = 3.0f;
    public float rotSpeed = 20f;
    public float jumpForce = 5.0f;
    public GameObject cameraPivot;
    
    private Vector3 orientation;
    private Vector3 idle;
    private bool moved = false;
    private float timeCount = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        orientation = new Vector3(0, 0, 0);
        idle = orientation;
    }

    // Update is called once per frame
    void Update()
    {
        moved = false;
        orientation = new Vector3(0, 0, 0);

        // TODO: Zmenit keycodes za actions
        if (Input.GetKey(KeyCode.W))
        {
            moved = true;
            orientation.z = 1;
            transform.Translate(new Vector3(0, 0, 90) * speed * Time.deltaTime, Space.World);
        }

        if (Input.GetKey(KeyCode.S))
        {
            moved = true;
            orientation.z = -1;
            transform.Translate(new Vector3(0, 0, -90) * speed * Time.deltaTime, Space.World);
        }

        if (Input.GetKey(KeyCode.A))
        {
            moved = true;
            orientation.x = -1;
            transform.Translate(new Vector3(-90, 0, 0) * speed * Time.deltaTime, Space.World);
        }

        if (Input.GetKey(KeyCode.D))
        {
            moved = true;
            orientation.x = 1;
            transform.Translate(new Vector3(90, 0, 0) * speed * Time.deltaTime, Space.World);
        }

        if (moved)
        {
            idle = orientation;
        }


        //transform.rotation = Quaternion.Slerp(transform.rotation, )

        //Vector3 camVec = cameraPivot.transform - transform;
    }
}

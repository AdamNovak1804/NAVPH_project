using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoScript : MonoBehaviour
{
    public (int, int) minMaxAmmo = (0, 5);
    public float start = -0.5f;
    public float end   =  0.5f;
    public float speed = 5f;

    private float dirSpeed;
    private float startPos;
    private float endPos;

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position.y + start;
        endPos = transform.position.y + end;

        dirSpeed = speed;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y > startPos && transform.position.y < endPos)
            transform.Translate(Vector3.up * dirSpeed * Time.deltaTime, Space.World);
        else
        {
            dirSpeed *= -1;
            transform.Translate(Vector3.up * dirSpeed * Time.deltaTime, Space.World);
        }
    }
}

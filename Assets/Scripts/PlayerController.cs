using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody playerRb;
    public bool doubleJumpEnabled = false;
    public float speed = 3.0f;
    public float jumpForce = 5.0f;
    public float gravityModifier = 1.5f;
    private int jumpCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // Jumping
        if (Input.GetKeyDown(KeyCode.Space) && jumpCount < 2) {
            playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumpCount += 2;
            Debug.Log("Im jumping");
            if (doubleJumpEnabled) {
                jumpCount -= 1;
                Debug.Log("Im trying double jump");
            }
            
        }

        // WASD movement
        if (Input.GetKey(KeyCode.W)) {
            transform.position += transform.forward * Time.deltaTime * speed;
        }

        if (Input.GetKey(KeyCode.S)) {
            transform.position -= transform.forward * Time.deltaTime * speed;
        }

        if (Input.GetKey(KeyCode.D)) {
            transform.position += transform.right * Time.deltaTime * speed;
        }

        if (Input.GetKey(KeyCode.A)) {
            transform.position -= transform.right * Time.deltaTime * speed;
        }


    }

    private void OnCollisionEnter(Collision other) {
        jumpCount = 0;
    }
}

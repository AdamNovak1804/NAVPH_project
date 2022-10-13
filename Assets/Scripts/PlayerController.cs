using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Rigidbody body;
    public int maxJumpCount = 2;
    public float speed = 3.0f;
    public float jumpForce = 5.0f;

    private int jumpCount = 0;

    // Update is called once per frame
    void Update()
    {
        // Jumping
        if (Input.GetKeyDown(KeyCode.Space) && jumpCount < maxJumpCount)
        {
            jumpCount += 1;
            body.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
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

    private void OnCollisionEnter(Collision collision)
    {
        jumpCount = 0;
    }
}

using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int health = 3;
    public int ammo = 0;

    public int maxJumpCount = 2;
    public float speed = 3.0f;
    public float jumpForce = 5.0f;

    private string actAnim = "";
    private Animation anim;
    private Rigidbody body;
    private int jumpCount = 0;

    private Quaternion actRot;

    void Start()
    {
        anim = GetComponent<Animation>();
        body = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 direction = transform.forward;

        if (anim.isPlaying.Equals(false) && jumpCount == 0)
        {
            actAnim = "Armature|Idle";
            anim.Play(actAnim);
        }

        // Jumping
        if (Input.GetKeyDown(KeyCode.Space) && jumpCount < maxJumpCount)
        {
            actAnim = "Armature|Jump";
            anim.Stop(actAnim);
            anim.Play(actAnim);

            jumpCount += 1;
            body.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        // WASD movement
        if (Input.GetKey(KeyCode.W))
        {
            direction.z = 1;
            transform.Translate(new Vector3(0, 0, 1) * speed * Time.deltaTime, Space.World);
        }

        if (Input.GetKey(KeyCode.S))
        {
            direction.z = -1;
            transform.Translate(new Vector3(0, 0, -1) * speed * Time.deltaTime, Space.World);
        }

        if (Input.GetKey(KeyCode.D))
        {
            direction.x = 1;
            transform.Translate(new Vector3(1, 0, 0) * speed * Time.deltaTime, Space.World);
        }

        if (Input.GetKey(KeyCode.A))
        {
            direction.x = -1;
            transform.Translate(new Vector3(-1, 0, 0) * speed * Time.deltaTime, Space.World);
        }

        direction.y = 0;

        if (Input.GetKey(KeyCode.R))
        {
            actAnim = "Armature|Reload";
            anim.Play(actAnim);
        }

        transform.forward = direction;
    }

    private void OnCollisionEnter(Collision collision)
    {
        jumpCount = 0;

        if (collision.transform.name == "MovingPlatform") 
        {
            Debug.Log("Collision with platform");
            transform.SetParent(collision.transform);
        }
    }

    private void OnCollisionExit(Collision other) 
    {
        if (other.transform.name == "MovingPlatform") 
        {
            Debug.Log("Leaving platform");
            transform.SetParent(null);
        }    
    }
}

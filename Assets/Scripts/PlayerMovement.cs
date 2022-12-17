using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static PlayerStats;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float currentSpeed;

    public float groundDrag;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;

    public float jumpCount;
    public int maxJumpCount;
    bool readyToJump;

    [HideInInspector] public float walkSpeed;
    [HideInInspector] public float sprintSpeed;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask Ground;
    public LayerMask Hittable;
    bool grounded;

    [Header("Camera")]
    public GameObject mainCamera;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    private float turnSmoothVelocity;
    public float turnSmoothTime = 0.01f;

    float targetAngle;
    float angle;

    private Animation anim;
    const string WALKING_ANIMATION = "Armature|Walking";
    const string IDLE_ANIMATION = "Armature|Idle";
    const string JUMPING_ANIMATION = "Armature|Jump";


    private void Start()
    {
        anim = GetComponent<Animation>();

        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;

        currentSpeed = moveSpeed;

        PowerUps.UpdateDoubleJump += DoubleJumpEnabled;
        PowerUps.UpdateSpeed += SpeedEnabled;
    }

    void OnDisable()
    {
        PowerUps.UpdateDoubleJump -= DoubleJumpEnabled;
        PowerUps.UpdateSpeed -= SpeedEnabled;
    }

    private void Update()
    {
        // ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, 0.06f, Ground) || Physics.Raycast(transform.position, Vector3.down, 0.06f, Hittable);

        if (grounded)
        {
            rb.drag = groundDrag;
            if (readyToJump)
            {
                jumpCount = 0;
            }
        }
        else
            rb.drag = 0;

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(horizontal, 0f, vertical);

        moveDirection = new Vector3(0, 0, 0);

        if (Input.GetKey(jumpKey) && readyToJump && (grounded || maxJumpCount > jumpCount))
        {
            jumpCount++;
            anim.Play(JUMPING_ANIMATION);
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        if (direction.magnitude > 0.1f)
        {
            if (anim.IsPlaying(IDLE_ANIMATION))
            {
                anim.Play(WALKING_ANIMATION);
            }
           
            targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + mainCamera.GetComponent<Camera>().transform.eulerAngles.y;
            
            //angle == look at enemy if targeted
            angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

            moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        }
        else
        {
            if (anim.IsPlaying(WALKING_ANIMATION))
            {
                anim.Stop();
            }
            
        }

        SpeedControl();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MovePlayer()
    {
        rb.MoveRotation(Quaternion.Euler(0f, angle, 0f));

        // on ground
        if (grounded)
            rb.AddForce(moveDirection.normalized * currentSpeed * 10f, ForceMode.Force);

        // in air
        else if (!grounded)
            rb.AddForce(moveDirection.normalized * currentSpeed * 10f * airMultiplier, ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // limit velocity if needed
        if (flatVel.magnitude > currentSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * currentSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    private void ResetJump()
    {
        readyToJump = true;
    }

    public void DoubleJumpEnabled(bool value, float duration)
    {
        if (value)
        {
            maxJumpCount = 2;
        }
        else
        {
            maxJumpCount = 1;
        }
    }

    public void SpeedEnabled(bool value, float duration, float newSpeed)
    {
        if (value)
        {
            currentSpeed = newSpeed;
        }
        else
        {
            currentSpeed = moveSpeed;
        }
    }
}
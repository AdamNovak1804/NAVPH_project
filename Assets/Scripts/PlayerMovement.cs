using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;

    public float groundDrag;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    public int maxJumpCount;
    bool readyToJump;

    [HideInInspector] public float walkSpeed;
    [HideInInspector] public float sprintSpeed;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
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

        Player.UpdateDoubleJump += DoubleJumpEnabled;
    }

    private void Update()
    {
        
        // ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, 0.1f, whatIsGround);

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(horizontal, 0f, vertical);

        moveDirection = new Vector3(0, 0, 0);

        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
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

        // handle drag
        if (grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;
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
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        // in air
        else if (!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // limit velocity if needed
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
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

    public void DoubleJumpEnabled(bool value)
    {
        Debug.Log("am i here?");
        if (value)
        {
            maxJumpCount = 2;
        }
        else
        {
            maxJumpCount = 1;
        }
    }
}
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;


public class PlayerController : MonoBehaviour
{
    const string DEATH_ANIMATION = "Armature|Death";
    const string WALKING_ANIMATION = "Armature|Walking";

    const string IDLE_ANIMATION = "Armature|Idle";
    public GameObject mainCamera;
    private Player player;

    public int maxJumpCount = 2;
    public float speed = 3.0f;
    public float jumpForce = 3.0f;

    private string actAnim = "";
    private Animation anim;
  
    private int jumpCount = 0;

    private bool isAlreadyDying = false;

    private float isDying = 1f;

    private Quaternion actRot;

    private AudioManager audioManager;


    public CharacterController controller;

    private float turnSmoothVelocity;
    public float turnSmoothTime = 0.01f;

    private float gravity;

    private PlayerCombatController playerCombatController;

    Vector3 forwardVector;
    void Start()
    {

        anim = GetComponent<Animation>();
        player = GetComponent<Player>();
        playerCombatController = GetComponent<PlayerCombatController>();
        audioManager = FindObjectOfType<AudioManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null && (player.GetHealth() <= 0f || transform.position.y <= -8f))
        {
            // Process death
            PlayAnim("Armature|Death");
            isAlreadyDying = true;
            isDying -= Time.deltaTime;
        }

        if (isDying <= 0f) 
        {
            SceneManager.LoadScene("IvoTestMenuScene");
        }

        if (isAlreadyDying) 
        {
            return;
        }

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(horizontal, 0f, vertical);

        Vector3 moveDir = new Vector3(0, 0, 0);

        if (direction.magnitude > 0.1f)
        {

            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + mainCamera.GetComponent<Camera>().transform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            PlayAnim("Armature|Idle");
        }

        if (controller.isGrounded)
        {
            gravity = 0;
            jumpCount = 0;
        }


        if (Input.GetButtonDown("Jump") && jumpCount < maxJumpCount)
        {

            actAnim = "Armature|Jump";
            anim.Stop(actAnim);
            anim.Play(actAnim);

            jumpCount += 1;
            gravity = jumpForce;
        }


        gravity -= 4f * Time.deltaTime;
        moveDir.y = gravity;

        moveDir.x *= speed;
        moveDir.z *= speed;

        controller.Move(moveDir * Time.deltaTime);
        

        if (anim.isPlaying.Equals(false) && jumpCount == 0)
        {
            actAnim = "Armature|Idle";
            anim.Play(actAnim);
        }
        

        if (Input.GetButtonDown("MeleeAttack") && playerCombatController.isAttacking <= 0f) 
        {
            playerCombatController.isAttacking = 1f;
            PlayAnim("Armature|Meelee");
            playerCombatController.shouldMeleeAttack = true;
        }

        if (Input.GetButtonDown("Shoot") && playerCombatController.isAttacking <= 0f) 
        {
            if (player.GetAmmo() >= 1f) 
            {
                playerCombatController.isAttacking = 1.2f;
                PlayAnim("Armature|Shoot");
                playerCombatController.RangeAttack();
            }
            else 
            {
                audioManager.Play("NoAmmo");
            }
        }

        if (playerCombatController.isShooting < 0.03f) 
        {
            transform.forward = direction;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        jumpCount = 0;
        if (collision.transform.name == "MovingPlatform") 
        {
            transform.SetParent(collision.transform);
        }

        if (collision.transform.name == "FinishLine") 
        {
            SceneManager.LoadScene("IvoFinishLineTestScene");
        }
    }

    private void OnCollisionExit(Collision other) 
    {
        if (other.transform.name == "MovingPlatform") 
        {
            transform.SetParent(null);
        }    
    }

    private void PlayAnim(string s) 
    {
        if (actAnim == DEATH_ANIMATION) 
        {
            // When dying, nothings else can be played
            return;
        }
        if (s == DEATH_ANIMATION) 
        {
            // Playing death stops everything
            anim.Stop();
        }
        if (s == WALKING_ANIMATION) 
        {
            if (actAnim == IDLE_ANIMATION) 
            {
                anim.Stop();
            }
            if (actAnim == "Armature|Meelee") 
            {
                return;
            }
        }
        actAnim = s;
        anim.Play(actAnim);
    }

}

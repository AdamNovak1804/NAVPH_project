using System.Numerics;
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
    public float jumpForce = 5.0f;

    private string actAnim = "";
    private Animation anim;
    private Rigidbody body;
    private int jumpCount = 0;

    private bool isAlreadyDying = false;

    private float isDying = 1f;

    private Quaternion actRot;

    private AudioManager audioManager;

    private PlayerCombatController playerCombatController;

    Vector3 forwardVector;
    void Start()
    {
        anim = GetComponent<Animation>();
        body = GetComponent<Rigidbody>();
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

        forwardVector = mainCamera.GetComponent<CameraController>().fwd.normalized;

        Vector3 direction = transform.forward;

        if (anim.isPlaying.Equals(false) && jumpCount == 0)
        {
            PlayAnim("Armature|Idle");
        }

        if (Input.GetButtonDown("Jump") && jumpCount < maxJumpCount)
        {

            actAnim = "Armature|Jump";
            anim.Stop(actAnim);
            anim.Play(actAnim);

            jumpCount += 1;
            body.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        // WASD movement
        if (Input.GetButton("Forward"))
        {
            PlayAnim("Armature|Walking");
            direction = forwardVector;
            transform.Translate(forwardVector * speed * Time.deltaTime, Space.World);
        }

        if (Input.GetButton("Backward"))
        {
            PlayAnim("Armature|Walking");
            direction = -forwardVector;
            transform.Translate(-forwardVector * speed * Time.deltaTime, Space.World);
        }

        if (Input.GetButton("Right"))
        {
            PlayAnim("Armature|Walking");
            direction = Quaternion.Euler(0, 90, 0) * forwardVector;
            transform.Translate(Quaternion.Euler(0, 90, 0) * forwardVector * speed * Time.deltaTime, Space.World);
        }

        if (Input.GetButton("Left"))
        {
            PlayAnim("Armature|Walking");
            direction = Quaternion.Euler(0, -90, 0) * forwardVector;
            transform.Translate(Quaternion.Euler(0, -90, 0) * forwardVector * speed * Time.deltaTime, Space.World);
        }

        direction.y = 0;

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

        transform.forward = direction;
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

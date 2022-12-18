using System.Diagnostics;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;


public class PlayerController : MonoBehaviour
{
    const string DEATH_ANIMATION = "Armature|Death";
    const string WALKING_ANIMATION = "Armature|Walking";
    const string IDLE_ANIMATION = "Armature|Idle";
    const string MEELEE_ANIMATION = "Armature|Meelee";
    const string SHOOTING_ANIMATION = "Armature|Shoot";

    public GameObject mainCamera;

    private float isDying = 1f;
    private string actAnim = "";
    private bool isAlreadyDying = false;
    
    private PlayerStats player;
    private Animation anim;
    private AudioManager audioManager;
    private PlayerCombatController playerCombatController;
    private Rigidbody rb;

    public FinishScreenController controller;
    public HUDController hudController;
    public DeathScreenController deathScreen;

    void Start()
    {
        anim = GetComponent<Animation>();
        player = GetComponent<PlayerStats>();
        playerCombatController = GetComponent<PlayerCombatController>();
        audioManager = FindObjectOfType<AudioManager>();
        rb = GetComponent<Rigidbody>();

    }

    // Update is called once per frame
    void Update()
    {

        if (player != null && (player.GetHealth() <= 0f || transform.position.y <= -8f))
        {
            // Process death
            PlayAnim(DEATH_ANIMATION);
            isAlreadyDying = true;
            isDying -= Time.deltaTime;
        }

        if (isDying <= 0f)
        {
            deathScreen.gameObject.SetActive(true);
            hudController.gameObject.SetActive(false);
        }

        if (isAlreadyDying)
        {
            return;
        }

        if (anim.isPlaying.Equals(false))
        {
            PlayAnim(IDLE_ANIMATION);
        }
        

        if (Input.GetButtonDown("MeleeAttack") && playerCombatController.isAttacking <= 0f) 
        {
            playerCombatController.isAttacking = 1.25f;
            PlayAnim(MEELEE_ANIMATION);
            playerCombatController.shouldMeleeAttack = true;
        }

        if (Input.GetButtonDown("Shoot") && playerCombatController.isAttacking <= 0f) 
        {
            if (player.GetAmmo() >= 1f) 
            {
                playerCombatController.isAttacking = 1.2f;
                PlayAnim(SHOOTING_ANIMATION);
                playerCombatController.RangeAttack();
            }
            else 
            {
                audioManager.Play("NoAmmo");
            }
        }

        
    }


    public void AddKnockback(Vector3 direction, float strength)
    {
        Debug.Log(direction.normalized * strength);
        rb.AddForce(direction.normalized * strength, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {      
        if (collision.transform.name == "FinishLine") 
        {
            controller.gameObject.SetActive(true);
            hudController.gameObject.SetActive(false);
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
            if (actAnim == MEELEE_ANIMATION) 
            {
                return;
            }
        }
        actAnim = s;
        anim.Play(actAnim);
    }

   

}

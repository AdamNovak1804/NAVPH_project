using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    const string DEATH_ANIMATION = "Armature|Death";
    const string MEELEE_ANIMATION = "Armature|Meelee";
    const string IDLE_ANIMATION = "Armature|Idle";
    const string WALKING_ANIMATION = "Armature|Walking";

    public float strength = 10f;
    public float health = 3f;
    public float damage = 0f;
    public float armor = 0f;
    public float dyingTime = 0.5f;
    public float waitAfterHit = 0.5f;

    public Transform pointOfRangeAttack;
    public GameObject projectile;

    private bool isDying = false;
    private Animation anim;
    private Rigidbody rb;
    private PlayerNavMesh playerNav;

    public AudioManager audioManager;

    public delegate void Died();
    public static event Died UpdateKilledEnemies;

    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        anim = this.GetComponent<Animation>();
        playerNav = GetComponent<PlayerNavMesh>();
        audioManager = FindObjectOfType<AudioManager>();
    }

    // Update is called once per frame
    void Update()
    {
        // enemy should die
        if (health <= 0.0f)
        {
            if (isDying == false) 
            {
                isDying = true;
                anim.Play(DEATH_ANIMATION);
            }
            else if (anim.IsPlaying(DEATH_ANIMATION) == false)
            {
                Die();
            }
            return;
        }

        // check if walking animation should be played based on current objective
        if (playerNav.GetMovingStatus() == true && NotAttackingOrWalking() == true)
        {
            anim.Play(WALKING_ANIMATION);
        }
        else if (playerNav.GetMovingStatus() == false)
        {
            anim.Play(IDLE_ANIMATION);
        }
    }

    private bool NotAttackingOrWalking()
    {
        return !(anim.IsPlaying(WALKING_ANIMATION) || anim.IsPlaying(MEELEE_ANIMATION));
    }

    public void Die()
    {
        UpdateKilledEnemies();
        this.gameObject.SetActive(false);
        Object.Destroy(this.gameObject);
    }

    public void TakeDamage(float value) 
    {
        health -= (value - armor);
        audioManager.Play("ZombieGrunt");
    }

    public void ApplyPushback(Vector3 playerPos, float strength)
    {
        rb.AddForce((this.transform.position - playerPos).normalized * strength, ForceMode.Impulse);
        playerNav.WaitAfterImpact(waitAfterHit);
    }

    public void RangeAttack() 
    {
        // Set better limits when projectiles are finished and speed is decided
        this.transform.LookAt(playerNav.GetPlayerPosition().position);
        var obj =  Object.Instantiate(projectile.gameObject, pointOfRangeAttack.position, Quaternion.Euler(-90,0,0));
        Projectile proj = (Projectile) obj.gameObject.GetComponent<Projectile>();
        proj.isEnemyProjectile = true;
        proj.ShootTowards(pointOfRangeAttack, playerNav.GetPlayerPosition().position + new Vector3(0,1,0));
    }

    public void MeeleeAttack(GameObject player, float isAtacking)
    {
        // Play animation of attack
        anim.Play(MEELEE_ANIMATION);
        if (isDying)
        {
            return;
        }

        if(player.GetComponent<PlayerCombatController>().isAttacking < isAtacking)
        {
            return;
        }

        player.GetComponent<PlayerStats>().DrainHealth(damage);
        player.GetComponent<PlayerController>().AddKnockback(player.gameObject.transform.position - this.transform.position, strength);
    }
}

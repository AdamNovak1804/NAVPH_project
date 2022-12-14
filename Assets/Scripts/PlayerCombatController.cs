using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatController : MonoBehaviour
{
    public Transform pointOfMeleeAttack;
    public float rangeOfMeleeAttack = 0.5f;

    public float rangeOfScan = 10f;
    public LayerMask enemyLayers;

    public GameObject projectile;
    private Player player;

    [System.NonSerialized]
    public float isAttacking = 0f;

    [System.NonSerialized]
    public bool shouldMeleeAttack = false;

    public float isShooting = 0.0f;

    private bool enemyLocked = false;
    private Vector3 closestEnemy;

    public GameObject target;

    private GameObject targetedEnemy;

    private AudioManager audioManager;
    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<Player>();
        audioManager = FindObjectOfType<AudioManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isShooting > 0f) 
        {
            isShooting -= Time.deltaTime;
        }
        if (isAttacking > 0f) {
            isAttacking -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Lock")) 
        {
            if (!enemyLocked) 
            {
                LocateEnemy();
            } 
            else 
            {
                enemyLocked = false;
                if (targetedEnemy != null) 
                {
                    targetedEnemy.SetActive(false);
                    Object.Destroy(targetedEnemy);
                }
            }
        }
        
        if (shouldMeleeAttack && isAttacking <= 0.25f) 
        {
            MakeMeleeImpact();
            shouldMeleeAttack = false;
        }
    }

    private void MakeMeleeImpact() 
    {
        Collider[] enemies = Physics.OverlapSphere(pointOfMeleeAttack.position, rangeOfMeleeAttack, enemyLayers);

        if (enemies != null && enemies.Length != 0) 
        {
            var enemy = enemies[0];
            Enemy enemyScript = (Enemy) enemy.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                enemyScript.TakeDamage(player.GetDamage());
                enemyScript.ApplyPushback(pointOfMeleeAttack.position);
                return;
            }
            BreakableScript breakableScript = (BreakableScript) enemy.GetComponent<BreakableScript>();
            if (breakableScript != null) 
            {
                audioManager.Play("BreakObject");
                breakableScript.DestroyObject();
            }
        }
    }

    public void RangeAttack() 
    {
        // Set better limits when projectiles are finished and speed is decided
        var obj =  Object.Instantiate(projectile.gameObject, pointOfMeleeAttack.position, Quaternion.Euler(-90,0,0));
        Projectile proj = (Projectile) obj.gameObject.GetComponent<Projectile>();
        player.UseAmmo();
        audioManager.Play("PlayerLaserShot");
        if (enemyLocked && targetedEnemy != null) 
        {
            targetedEnemy.SetActive(false);
            Object.Destroy(targetedEnemy);
        }
        if (enemyLocked) 
        {
            isShooting = 0.06f;
            var currentTransform = transform;
            transform.LookAt(closestEnemy);
            proj.ShootTowards(pointOfMeleeAttack, closestEnemy);
            //isShooting = false;
            // transform.forward = currentTransform.forward;
            enemyLocked = false;
        } else 
        {
            proj.ShootTowards(pointOfMeleeAttack, pointOfMeleeAttack.position + transform.forward * 50f);
        }
    }

    private void LocateEnemy() 
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, rangeOfScan, enemyLayers);

        if (enemies != null && enemies.Length != 0) 
        {
            bool locatedFirst = false;
            foreach (Collider enemy in enemies) 
            {
                Enemy enemyScript = (Enemy) enemy.GetComponent<Enemy>();
                if (enemyScript != null) 
                {
                    if (!locatedFirst) 
                    {
                        locatedFirst = true;
                        enemyLocked = true;
                        closestEnemy = enemy.gameObject.transform.position;
                        continue;
                    }
                    if (Vector3.Distance(transform.position, closestEnemy) > Vector3.Distance(transform.position, enemy.gameObject.transform.position))
                    {
                        closestEnemy = enemy.gameObject.transform.position;
                    }
                }
            }
            if (enemyLocked) 
            {
                targetedEnemy = CreateTarget();
            }
        }
    }

    private GameObject CreateTarget() 
    {
        return Object.Instantiate(target, closestEnemy + new Vector3(0,2,0), Quaternion.Euler(90,0,0));
    }

    private void OnDrawGizmosSelected() {
        if (pointOfMeleeAttack != null) 
        {
            Gizmos.DrawWireSphere(pointOfMeleeAttack.position, rangeOfMeleeAttack);
            Gizmos.DrawWireSphere(transform.position, rangeOfScan);
        }
    }
}
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class PlayerController : MonoBehaviour
{
    public GameObject camera;
    public Transform pointOfMeleeAttack;
    public float rangeOfMeleeAttack = 0.5f;

    public float rangeOfScan = 10f;
    public LayerMask enemyLayers;

    private Player player;

    public int maxJumpCount = 2;
    public float speed = 3.0f;
    public float jumpForce = 3.0f;

    public GameObject projectile;

    private string actAnim = "";
    private Animation anim;
  
    private int jumpCount = 0;

    private float isAttacking = 0f;

    private float meleeDamage;

    private bool enemyLocked = false;
    private Vector3 closestEnemy;

    public GameObject target;

    private GameObject targetedEnemy;

    private Quaternion actRot;

    private AudioManager audioManager;

    public CharacterController controller;

    private float turnSmoothVelocity;
    private float turnSmoothTime = 0.01f;

    private float gravity;

    Vector3 forwardVector;
    void Start()
    {

        anim = GetComponent<Animation>();
        player = GetComponent<Player>();
        audioManager = FindObjectOfType<AudioManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isAttacking > 0f) {
            isAttacking -= Time.deltaTime;
        }

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(horizontal, 0f, vertical);

        Vector3 moveDir = new Vector3(0, 0, 0);

        if (direction.magnitude > 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + camera.transform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        }
        if (controller.isGrounded)
        {
            gravity = 0;
            jumpCount = 0;
        }

        if (Input.GetKeyDown(KeyCode.Space) && jumpCount < maxJumpCount)
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
        

        if (Input.GetKey(KeyCode.R))
        {
            actAnim = "Armature|Reload";
            anim.Play(actAnim);
        }

        if (Input.GetKeyDown(KeyCode.L)) 
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

        if (Input.GetKeyDown(KeyCode.J) && isAttacking <= 0f) 
        {
            isAttacking = 1f;
            actAnim = "Armature|Meelee";
            anim.Play(actAnim);
            Attack();
        }

        if (Input.GetKeyDown(KeyCode.K) && isAttacking <= 0f) 
        {
            isAttacking = 1.2f;
            actAnim = "Armature|Shoot";
            anim.Play(actAnim);
            RangeAttack();
        }
    }

    private void FixedUpdate()
    {
        
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

    private void Attack() 
    {
        Collider[] enemies = Physics.OverlapSphere(pointOfMeleeAttack.position, rangeOfMeleeAttack, enemyLayers);

        if (enemies != null && enemies.Length != 0) 
        {
            var enemy = enemies[0];
            Debug.Log("Hitted" + enemy.name);
            Enemy enemyScript = (Enemy) enemy.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                Debug.Log(enemy.name + " took " + player.GetDamage() + " damage.");
                enemyScript.TakeDamage(player.GetDamage());
                return;
            }
            BreakableScript breakableScript = (BreakableScript) enemy.GetComponent<BreakableScript>();
            if (breakableScript != null) 
            {
                breakableScript.DestroyObject();
            }
        }
    }

    private void RangeAttack() 
    {
        // Set better limits when projectiles are finished and speed is decided
        var obj =  Object.Instantiate(projectile.gameObject, pointOfMeleeAttack.position, Quaternion.identity);
        Projectile proj = (Projectile) obj.gameObject.GetComponent<Projectile>();
        audioManager.Play("PlayerLaserShot");
        if (enemyLocked && targetedEnemy != null) 
        {
            targetedEnemy.SetActive(false);
            Object.Destroy(targetedEnemy);
        }
        if (enemyLocked) 
        {
            proj.ShootTowards(pointOfMeleeAttack, closestEnemy);
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
        return Object.Instantiate(target, closestEnemy + new Vector3(0,3,0), Quaternion.identity);
    }

    private void OnDrawGizmosSelected() {
        if (pointOfMeleeAttack != null) 
        {
            Gizmos.DrawWireSphere(pointOfMeleeAttack.position, rangeOfMeleeAttack);
            Gizmos.DrawWireSphere(transform.position, rangeOfScan);
        }
    }

}

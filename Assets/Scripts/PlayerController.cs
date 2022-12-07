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
    public Transform pointOfMeleeAttack;
    public float rangeOfMeleeAttack = 0.5f;

    public float rangeOfScan = 10f;
    public LayerMask enemyLayers;

    private Player player;

    public int maxJumpCount = 2;
    public float speed = 3.0f;
    public float jumpForce = 5.0f;

    public GameObject projectile;

    private string actAnim = "";
    private Animation anim;
    private Rigidbody body;
    private int jumpCount = 0;

    private float isAttacking = 0f;

    private float isDying = 1f;

    private float meleeDamage;

    private bool enemyLocked = false;
    private Vector3 closestEnemy;

    public GameObject target;

    private GameObject targetedEnemy;

    private Quaternion actRot;

    private AudioManager audioManager;

    Vector3 forwardVector;
    void Start()
    {
        anim = GetComponent<Animation>();
        body = GetComponent<Rigidbody>();
        player = GetComponent<Player>();
        audioManager = FindObjectOfType<AudioManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null && (player.GetHealth() <= 0f || transform.position.y <= -8f))
        {
            // Process death
            PlayAnim("Armature|Death");
            isDying -= Time.deltaTime;
        }

        if (isDying <= 0f) 
        {
            SceneManager.LoadScene("IvoTestMenuScene");
        }

        if (isAttacking > 0f) {
            isAttacking -= Time.deltaTime;
        }
        forwardVector = mainCamera.GetComponent<CameraController>().fwd.normalized;

        Vector3 direction = transform.forward;

        if (anim.isPlaying.Equals(false) && jumpCount == 0)
        {
            actAnim = "Armature|Idle";
            anim.Play(actAnim);
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

        if (Input.GetButtonDown("Reload"))
        {
            PlayAnim("Armature|Reload");
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

        if (Input.GetButtonDown("MeleeAttack") && isAttacking <= 0f) 
        {
            isAttacking = 1f;
            PlayAnim("Armature|Meelee");
            Attack();
        }

        if (Input.GetButtonDown("Shoot") && isAttacking <= 0f) 
        {
            isAttacking = 1.2f;
            PlayAnim("Armature|Shoot");
            RangeAttack();
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

    private void Attack() 
    {
        Collider[] enemies = Physics.OverlapSphere(pointOfMeleeAttack.position, rangeOfMeleeAttack, enemyLayers);

        if (enemies != null && enemies.Length != 0) 
        {
            var enemy = enemies[0];
            Enemy enemyScript = (Enemy) enemy.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                enemyScript.TakeDamage(player.GetDamage());
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
        }
        actAnim = s;
        anim.Play(actAnim);
    }

}

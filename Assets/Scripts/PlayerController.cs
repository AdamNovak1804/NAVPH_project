using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class PlayerController : MonoBehaviour
{
    public GameObject camera;
    public Transform pointOfMeleeAttack;
    public float rangeOfMeleeAttack = 0.5f;
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

    private float meleeDamage;

    private Quaternion actRot;

    Vector3 forwardVector;
    void Start()
    {
        anim = GetComponent<Animation>();
        body = GetComponent<Rigidbody>();
        player = GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isAttacking > 0f) {
            isAttacking -= Time.deltaTime;
        }
        forwardVector = camera.GetComponent<CameraController>().fwd.normalized;

        Vector3 direction = transform.forward;

        if (anim.isPlaying.Equals(false) && jumpCount == 0)
        {
            actAnim = "Armature|Idle";
            anim.Play(actAnim);
        }
        // REWORK CONTROLS SYSTEM
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
            direction = forwardVector;
            transform.Translate(forwardVector * speed * Time.deltaTime, Space.World);
        }

        if (Input.GetKey(KeyCode.S))
        {
            direction = -forwardVector;
            transform.Translate(-forwardVector * speed * Time.deltaTime, Space.World);
        }

        if (Input.GetKey(KeyCode.D))
        {
            direction = Quaternion.Euler(0, 90, 0) * forwardVector;
            transform.Translate(Quaternion.Euler(0, 90, 0) * forwardVector * speed * Time.deltaTime, Space.World);
        }

        if (Input.GetKey(KeyCode.A))
        {
            direction = Quaternion.Euler(0, -90, 0) * forwardVector;
            transform.Translate(Quaternion.Euler(0, -90, 0) * forwardVector * speed * Time.deltaTime, Space.World);
        }

        direction.y = 0;

        if (Input.GetKey(KeyCode.R))
        {
            actAnim = "Armature|Reload";
            anim.Play(actAnim);
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
        proj.ShootTowards(pointOfMeleeAttack, pointOfMeleeAttack.position + new Vector3(0,0,20));
    }

    private void OnDrawGizmosSelected() {
        if (pointOfMeleeAttack != null) 
        {
            Gizmos.DrawWireSphere(pointOfMeleeAttack.position, rangeOfMeleeAttack);
        }
    }

}

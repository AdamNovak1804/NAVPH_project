using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float lifeTime = 3f;

    public float damage = 1f;

    public float speed = 1f;

    public bool isEnabled = false;

    public LayerMask hittableLayers;

    private bool targetHit = false;

    public bool isEnemyProjectile = false;

    private Vector3 goalPosition;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, goalPosition) < 0.01f) 
        {
            targetHit = true;
        }
        if (isEnabled && lifeTime >= 0f && !targetHit) 
        {
            lifeTime -= Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, goalPosition, Time.deltaTime * speed);
            
        }
        if (isEnabled && lifeTime >= 0f && targetHit) 
        {
            lifeTime -= Time.deltaTime;
            transform.position = transform.forward * Time.deltaTime * speed;
        }
        if (lifeTime < 0f) 
        {
            this.gameObject.SetActive(false);
            Object.Destroy(this.gameObject);
        }
        
    }

    public void ShootTowards(Transform startingPosition, Vector3 goalPosition) 
    {
        this.goalPosition = goalPosition;
        this.transform.LookAt(goalPosition);
        transform.Rotate(new Vector3(-90,0,0));
        isEnabled = true;
    }

    private void OnTriggerEnter(Collider other) {
        Debug.Log("Hit" + other.name);
        if (isEnemyProjectile) 
        {
            Player player = (Player) other.GetComponent<Player>();
            if (player != null) 
            {
                player.DrainHealth(10f);
                this.gameObject.SetActive(false);
                Object.Destroy(this.gameObject);
            }
        }
        Enemy enemy = (Enemy) other.GetComponent<Enemy>();
        if (enemy != null) 
        {   
            enemy.TakeDamage(damage);
            this.gameObject.SetActive(false);
            Object.Destroy(this.gameObject);
            Debug.Log("Hitted "+ other.name);
        }
        // Proces hit
    }


}

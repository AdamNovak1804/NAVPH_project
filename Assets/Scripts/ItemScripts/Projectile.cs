using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float lifeTime = 3f;
    public float damage = 1f;
    public float speed = 1f;
    public bool isEnabled = false;
    public bool isEnemyProjectile = false;
    public LayerMask hittableLayers;

    private bool targetHit = false;
    private Vector3 goalPosition;

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
        transform.Rotate(new Vector3(-90, 0, 0));
        isEnabled = true;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (isEnabled)
        {
            Debug.Log("Hit" + other.name);
            if (isEnemyProjectile)
            {
                PlayerStats player = (PlayerStats)other.GetComponent<PlayerStats>();
                if (player != null)
                {
                    player.DrainHealth(damage);
                    this.gameObject.SetActive(false);
                    Object.Destroy(this.gameObject);
                }
                return;
            }
            Enemy enemy = (Enemy)other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                this.gameObject.SetActive(false);
                Object.Destroy(this.gameObject);
            }
            else
            {
                Boss boss = (Boss)other.GetComponent<Boss>();

                if (boss != null)
                {
                    boss.HitTarget();
                    this.gameObject.SetActive(false);
                    Object.Destroy(this.gameObject);
                }
            }
        }
    }
}

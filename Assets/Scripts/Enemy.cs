using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float health = 3f;

    public float damage = 0f;

    public float armor = 0f;

    public float dyingTime = 0.5f;

    private Rigidbody rb;

    public Transform pointOfRangeAttack;

    public GameObject projectile;
    private PlayerNavMesh playerNav;

    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        playerNav = GetComponent<PlayerNavMesh>();
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0f) 
        {
            dyingTime -= Time.deltaTime;
            if (dyingTime <= 0f) 
            {
                // Process death
                Debug.Log(gameObject.name + " should be dying");
                this.gameObject.SetActive(false);
                Object.Destroy(this.gameObject);
            }
        }
    }

    public void TakeDamage(float value) 
    {
        // Apply pushback
        health -= (value - armor);
    }

    // Needs to be reworked!
    // TODO
    public void ApplyPushback(Vector3 position) 
    {
        Debug.Log("Im here");
        Vector3 direction = position - base.transform.position;
        direction.y = 0;

        //rb.AddForce(-10f * direction.normalized, ForceMode.Impulse);
        transform.position = transform.position + direction * 2f;
        playerNav.WaitAfterImpact(0.5f);
    }

    public void RangeAttack() 
    {
        // Set better limits when projectiles are finished and speed is decided
        var obj =  Object.Instantiate(projectile.gameObject, pointOfRangeAttack.position, Quaternion.Euler(-90,0,0));
        Projectile proj = (Projectile) obj.gameObject.GetComponent<Projectile>();
        proj.ShootTowards(pointOfRangeAttack, playerNav.GetPlayerPosition().position + new Vector3(0,1,0));
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float lifeTime = 3f;

    public float damage = 1f;

    public float speed = 1f;

    public bool isEnabled = false;

    private Vector3 goalPosition;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isEnabled && lifeTime >= 0f) 
        {
            lifeTime -= Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, goalPosition, Time.deltaTime * speed);
        }
        if (lifeTime < 0f) 
        {
            this.gameObject.SetActive(false);
            Object.Destroy(this.gameObject);
        }
        
    }

    public void ShootForward(Transform startingPosition) 
    {

    }

    public void ShootTowards(Transform startingPosition, Vector3 goalPosition) 
    {
        this.goalPosition = goalPosition;
        isEnabled = true;
    }

    private void OnCollisionEnter(Collision other) 
    {
        
    }
}

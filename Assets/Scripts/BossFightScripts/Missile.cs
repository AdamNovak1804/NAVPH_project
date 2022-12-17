using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    public float lethalDistance = 3.0f;
    public float speed = 2.0f;
    public float ceilingHeight = 25.0f;

    private float damage = 0.0f;
    private GameObject target;
    private Player player;
    private Vector3 curDir;

    public enum MissileType
    {
        explosive,
        breaking
    }
    private MissileType thisMissile;

    public void setDamage(float damage)
    {
        this.damage = damage;
    }

    public void setMissileType(string missile)
    {
        thisMissile = (MissileType)System.Enum.Parse(typeof(MissileType), missile);
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Missile Active!");

        target = GameObject.FindWithTag("Player");
        player = target.GetComponent<Player>();

        curDir = new Vector3(0.0f, speed, 0.0f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += curDir * Time.deltaTime;

        // the missile reached off camera, translate to target cursor
        if (transform.position.y >= ceilingHeight)
        {
            Debug.Log("I'm going down!");
            Vector3 newPos = new Vector3(target.transform.position.x, transform.position.y - 0.1f, target.transform.position.z);
            transform.position = newPos;
            curDir *= -1;
        }
    }

    // Explode when hitting anything
    void OnCollisionEnter()
    {
        Debug.Log("I hit!");

        Vector3 targetCenter = target.transform.position + new Vector3(0.0f, 1.8f, 0.0f);

        float distanceFromTarget = Vector3.Distance(targetCenter, transform.position);

        switch (thisMissile)
        {
            case MissileType.explosive:
                Debug.Log("Explosive missile goes Boom!");

                if (distanceFromTarget <= lethalDistance)
                {
                    player.DrainHealth(damage);
                }
                break;
            case MissileType.breaking:
                Debug.Log("Breaking missile goes Boom!");

                break;
       }

        gameObject.SetActive(false);
        Destroy(gameObject);
    }
}

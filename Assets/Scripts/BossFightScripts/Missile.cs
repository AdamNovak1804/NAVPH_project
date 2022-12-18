using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    public float lethalDistance = 3.0f;
    public float speed = 2.0f;
    public float ceilingHeight = 25.0f;
    public float missileScale = 3.0f;

    private float damage = 0.0f;
    private GameObject target;
    private PlayerStats player;
    private Vector3 curDir;

    public enum MissileType
    {
        explosive,
        breaking
    }
    private MissileType thisMissile;
    private AudioManager audioManager;

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
        audioManager = FindObjectOfType<AudioManager>();
        target = GameObject.FindWithTag("Player");
        player = target.GetComponent<PlayerStats>();

        curDir = new Vector3(0.0f, speed, 0.0f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += curDir * Time.deltaTime;

        // the missile reached off camera, translate to target cursor
        if (transform.position.y >= ceilingHeight)
        {
            Vector3 newPos = new Vector3(target.transform.position.x, transform.position.y - 0.1f, target.transform.position.z);
            transform.position = newPos;
            transform.localScale *= missileScale;
            transform.Rotate(0, 180.0f, 0);
            curDir *= -1;
        }
    }

    // Explode when hitting anything
    void OnCollisionEnter(Collision collision)
    {
        audioManager.Play("MissileHit");

        Vector3 targetCenter = target.transform.position + new Vector3(0.0f, 1.8f, 0.0f);

        float distanceFromTarget = Vector3.Distance(targetCenter, transform.position);

        switch (thisMissile)
        {
            case MissileType.explosive:
                if (distanceFromTarget <= lethalDistance)
                {
                    player.DrainHealth(damage);
                }
                break;
            case MissileType.breaking:
                if (collision.gameObject.tag == "GlassPane")
                {
                    collision.gameObject.SetActive(false);
                }
                break;
       }

        gameObject.SetActive(false);
        Destroy(gameObject);
    }
}

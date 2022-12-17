using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public float missileDelay = 1.0f;
    public GameObject missile;
    public float tmpTester;
    public float explosiveMissileDamage = 2.0f;
    
    public Texture explosiveMissileTexture;

    private const string IDLE_ANIMATION = "Armature|Idle";
    private const string WALKING_ANIMATION = "Armature|Walking";
    private const string SHOOTING_ANIMATION = "Armature|Shoot";

    private enum BossState
    {
        idle,
        walking,
        shootingExplosive,
        shootingBreaking,
        beingHurt,
        dying
    }

    private Animation anim;
    private BossState actState;
    private float actMissileDelay = 1.0f;
    private bool hasShot = false;

    // Start is called before the first frame update
    void Start()
    {
        anim = gameObject.GetComponent<Animation>();
        actState = BossState.idle;
    }

    // Update is called once per frame
    void Update()
    {
        if ((tmpTester -= Time.deltaTime) < 0.0f)
        {
            actState = BossState.shootingExplosive;
            tmpTester = 9999;
        }


        switch(actState) 
        {
            case BossState.idle:
                if (!anim.IsPlaying(IDLE_ANIMATION))
                {
                    anim.Play(IDLE_ANIMATION);
                }
                break;
            case BossState.walking:
                if (!anim.IsPlaying(WALKING_ANIMATION))
                {
                    anim.Play(WALKING_ANIMATION);
                }
                // move by increment to a keyposition
                // if in the position, change state
                break;
            case BossState.shootingExplosive:
                actMissileDelay -= Time.deltaTime;
                bool isShooting = anim.IsPlaying(SHOOTING_ANIMATION);

                // play shooting animation if not shot yet
                if (isShooting == false && hasShot == false)
                {
                    anim.Play(SHOOTING_ANIMATION);
                }
                // after shooting animation revert to idle
                else if (isShooting == false && hasShot == true)
                {
                    hasShot = false;
                    actState = BossState.idle;
                    actMissileDelay = missileDelay;
                    break;
                }

                // shoot a rocket
                if (actMissileDelay < 0.0f && hasShot == false)
                {
                    hasShot = true;
                    actMissileDelay = missileDelay;
                    
                    Vector3 missilePosition = gameObject.transform.GetChild(0).gameObject.transform.position;
                    Quaternion missileRotation = transform.rotation * Quaternion.Euler(0.0f, 90.0f, 90.0f);
                    GameObject missileInstance = Object.Instantiate(missile, missilePosition, missileRotation);

                    missileInstance.GetComponent<Renderer>().material.SetTexture("_MainTex", explosiveMissileTexture);
                    
                    Missile missileScript = missileInstance.GetComponent<Missile>();
                    missileScript.setDamage(explosiveMissileDamage);
                    missileScript.setMissileType("breaking");
                }
                break;
        }
    }
}

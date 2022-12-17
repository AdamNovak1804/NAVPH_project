using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public float missileDelay = 1.0f;
    public GameObject missile;
    public float tmpTester;
    public float explosiveMissileDamage = 2.0f;
    public int explosionsByLevel = 5;
    public float vulnerableTime = 5.0f;
    
    public Texture explosiveMissileTexture;
    public Texture breakingMissileTexture;
    private float actVulnerableTime;

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
        dying,
        vulnerable
    }

    private Dictionary<int, string> bossStates;
    private Animation anim;
    private BossState actState;
    private float actMissileDelay = 1.0f;
    private bool hasShot = false;
    private bool fightHasBegun = false;

    private Queue<BossState> actionList = new Queue<BossState>();

    public void startFight()
    {
        fightHasBegun = true;
    }

    private void FillActionQueue()
    {
        // fill the queue with random shooting actions
        for (int i = 0; i < explosionsByLevel; i++)
        {
            actionList.Enqueue((BossState)System.Enum.Parse(typeof(BossState), bossStates[Random.Range(0, 2)]));
        }
        // at the end of the individual level, one state has to exist where the enemy is exposed
        actionList.Enqueue(BossState.vulnerable);
    }

    // Start is called before the first frame update
    void Start()
    {
        actVulnerableTime = vulnerableTime;

        bossStates = new Dictionary<int, string>() {
            {0, "shootingExplosive"},
            {1, "shootingBreaking"},
        };

        anim = gameObject.GetComponent<Animation>();
        actState = BossState.idle;
    }

    // Update is called once per frame
    void Update()
    {
        // only testing the fight has begun abiilty
        if ((tmpTester -= Time.deltaTime) < 0.0f)
        {
            fightHasBegun = true;
        }

        if (actionList.Count == 0)
        {
            FillActionQueue();
        }

        // do stuff based on the current action
        switch(actState) 
        {
            case BossState.idle:
                if (!anim.IsPlaying(IDLE_ANIMATION))
                {
                    anim.Play(IDLE_ANIMATION);
                    
                    if (fightHasBegun == true)
                    {
                        actState = actionList.Dequeue();
                    }
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
                HandleMissileShooting("explosive", explosiveMissileTexture);
                break;
            case BossState.shootingBreaking:
                HandleMissileShooting("breaking", breakingMissileTexture);
                break;
            case BossState.vulnerable:
                if (!anim.IsPlaying(IDLE_ANIMATION))
                {
                    anim.Play(IDLE_ANIMATION);
                }

                actVulnerableTime -= Time.deltaTime;

                if (actVulnerableTime < 0.0f)
                {
                    actState = BossState.idle;
                }
                break;
        }
    }

    public void HandleMissileShooting(string missileType, Texture missileTexture)
    {
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
            return;
        }

        // shoot a rocket
        if (actMissileDelay < 0.0f && hasShot == false)
        {
            hasShot = true;
            actMissileDelay = missileDelay;
            
            Vector3 missilePosition = gameObject.transform.GetChild(0).gameObject.transform.position;
            Quaternion missileRotation = transform.rotation * Quaternion.Euler(0.0f, 90.0f, 90.0f);
            GameObject missileInstance = Object.Instantiate(missile, missilePosition, missileRotation);

            missileInstance.GetComponent<Renderer>().material.SetTexture("_MainTex", missileTexture);
            
            Missile missileScript = missileInstance.GetComponent<Missile>();
            missileScript.setDamage(explosiveMissileDamage);
            missileScript.setMissileType(missileType);
        }
    }
}

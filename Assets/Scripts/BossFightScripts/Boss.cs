using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boss : MonoBehaviour
{
    public int lives = 3;
    public float missileDelay = 1.0f;
    public float explosiveMissileDamage = 2.0f;
    public int explosionsByLevel = 5;
    public float waitInterval = 2.0f;
    public float vulnerableTime = 5.0f;
    
    public Transform[] bossPositions;
    public Texture explosiveMissileTexture;
    public Texture breakingMissileTexture;
    public GameObject missile;
    public GameObject target;
    public Camera camera;

    private const string IDLE_ANIMATION = "Armature|Idle";
    private const string WALKING_ANIMATION = "Armature|Walking";
    private const string SHOOTING_ANIMATION = "Armature|Shoot";
    private const string TAKE_DAMAGE_ANIMATION = "Armature|TakeDamage";
    private const string DYING_ANIMATION = "Armature|Die";

    private float actMissileDelay = 1.0f;
    private float actWaitingTime;
    private float actVulnerableTime;
    private bool hasDied = false;
    private bool hasShot = false;
    private bool fightHasBegun = false;

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

    private Vector3 targetPosition;
    private GameObject shield;
    private Dictionary<int, string> bossStates;
    private Animation anim;
    private BossState actState;
    private BossFightCamera cameraScript;
    private NavMeshAgent navMeshAgent;
    private Queue<BossState> actionQueue = new Queue<BossState>();
    private Queue<Vector3> positionQueue = new Queue<Vector3>();

    public void HitTarget()
    {
        if (actState == BossState.vulnerable)
        {
            lives -= 1;
            actVulnerableTime = vulnerableTime;

            // if all lives had been lost then die
            if (lives == 0)
            {
                actState = BossState.dying;
            }
            // go to the next position
            else
            {
                shield.SetActive(true);
                actWaitingTime = waitInterval;
                actState = BossState.beingHurt;
            }
        }
    }

    public void startFight()
    {
        fightHasBegun = true;
    }

    private void FillActionQueue()
    {
        // fill the queue with random shooting actions
        for (int i = 0; i < explosionsByLevel; i++)
        {
            actionQueue.Enqueue((BossState)System.Enum.Parse(typeof(BossState), bossStates[Random.Range(0, 2)]));
        }
        // at the end of the individual level, one state has to exist where the enemy is exposed
        actionQueue.Enqueue(BossState.vulnerable);
    }

    private void FillPositionQueue()
    {
        // fill the queue with boss positions
        foreach(Transform pos in bossPositions)
        {
            positionQueue.Enqueue(pos.position);
        }
    }

    private bool ShouldBeTurning()
    {
        return ((actState != BossState.walking && actState != BossState.beingHurt && actState != BossState.dying));
    }

    // Start is called before the first frame update
    void Start()
    {
        FillPositionQueue();

        actVulnerableTime = vulnerableTime;
        actWaitingTime = 0.0f;

        bossStates = new Dictionary<int, string>() {
            {0, "shootingExplosive"},
            {1, "shootingBreaking"},
        };

        shield = gameObject.transform.GetChild(1).gameObject;
        anim = gameObject.GetComponent<Animation>();
        navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
        cameraScript = camera.gameObject.GetComponent<BossFightCamera>();
        actState = BossState.idle;
    }

    // Update is called once per frame
    void Update()
    {
        if (ShouldBeTurning())
        {
            Vector3 targetDirection = target.transform.position - transform.position;
            Vector3 lookAtTarget = Vector3.RotateTowards(transform.forward, targetDirection, 1.0f * Time.deltaTime, 0.0f);
            lookAtTarget.y = 0;

            transform.rotation = Quaternion.LookRotation(lookAtTarget);
        }

        if (actionQueue.Count == 0)
        {
            FillActionQueue();
        }

        // do stuff based on the current action
        switch(actState) 
        {
            case BossState.idle:
                actWaitingTime -= Time.deltaTime;

                if (!anim.IsPlaying(IDLE_ANIMATION))
                {
                    anim.Play(IDLE_ANIMATION);
                }
                    
                if (fightHasBegun == true && actWaitingTime <= 0.0f)
                {
                    actWaitingTime = waitInterval;
                    actState = actionQueue.Dequeue();
                }
                break;
            case BossState.walking:
                if (!anim.IsPlaying(WALKING_ANIMATION))
                {
                    anim.Play(WALKING_ANIMATION);
                }

                if (transform.position == targetPosition)
                {
                    actState = BossState.idle;
                }
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

                shield.SetActive(false);
                actVulnerableTime -= Time.deltaTime;

                if (actVulnerableTime < 0.0f)
                {
                    shield.SetActive(true);
                    actVulnerableTime = vulnerableTime;
                    actState = BossState.idle;
                }
                break;
            case BossState.beingHurt:
                actWaitingTime -= Time.deltaTime;

                if (!anim.IsPlaying(TAKE_DAMAGE_ANIMATION))
                {
                    anim.Play(TAKE_DAMAGE_ANIMATION);
                }

                if (actWaitingTime <= 0.0f)
                {
                    actWaitingTime = waitInterval;
                    cameraScript.nextPosition();
                    actState = BossState.walking;
                    targetPosition = positionQueue.Dequeue();
                    targetPosition.y = transform.position.y;
                    navMeshAgent.SetDestination(targetPosition);
                }
                break;
            case BossState.dying:
                if (!anim.IsPlaying(DYING_ANIMATION))
                {
                    anim.Play(DYING_ANIMATION);

                    if (hasDied == true)
                    {
                        gameObject.SetActive(false);
                        Destroy(gameObject);
                        // destroy the wall beneath the boss
                    }

                    hasDied = true;
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

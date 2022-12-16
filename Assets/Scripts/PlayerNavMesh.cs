using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// Inspired by https://www.youtube.com/watch?v=atCOd4o7tG4
public class PlayerNavMesh : MonoBehaviour
{
    public enum EnemyType
    {
        melee,
        ranged,
    }
    public EnemyType enemyType;
    public Player player;
    public float sightDistance = 8.0f;
    public float attackWait = 1.2f;
    public float hitDistance = 2.0f;

    private NavMeshAgent navMeshAgent;
    private float isWaiting = 0f;
    private float isAttacking = 0f;
    private Enemy enemy;
    
    private void Awake() {
        navMeshAgent = GetComponent<NavMeshAgent>();
        enemy = GetComponent<Enemy>();
    }

    // Update is called once per frame
    private void Update()
    {
        var distance = Vector3.Distance(transform.position, player.gameObject.transform.position);
        isAttacking -= Time.deltaTime;
        
        if (isWaiting > 0f) 
        {
            isWaiting -= Time.deltaTime;
            return;
        }

        // Add radius limit
        if (enemyType == EnemyType.melee)
        {
            if (distance < sightDistance)
            {
                navMeshAgent.destination = player.gameObject.transform.position;
            }
            else 
            {
                navMeshAgent.destination = transform.position;
            }
        }
        
        // If ranged - if in first range - setDestination
        if (enemyType == EnemyType.ranged) 
        {
            if (isAttacking <= 0f) 
            {
                enemy.RangeAttack();
                isAttacking = attackWait;
            }
        }
        // If in second range - stop and shoot - validate second first
        // If close to combat, attack
        if (enemyType == EnemyType.melee)
        {
            if (isAttacking <= 0f && distance < hitDistance)
            {
                enemy.MeeleeAttack(player);
                isAttacking = attackWait;
            }
        }
    }


    public void WaitAfterImpact(float value) 
    {
        isWaiting = value;
    }

    public Transform GetPlayerPosition() 
    {
        return player.transform;
    }
}

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
    private NavMeshAgent navMeshAgent;
    public GameObject player;
    
    private void Awake() {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    private void Update()
    {
        // Add radius limit
        if (enemyType == EnemyType.melee)
        {
            navMeshAgent.destination = player.gameObject.transform.position;
        }
        
        // If ranged - if in first range - setDestination
        // If in second range - stop and shoot - validate second first
        // If close to combat, attack
    }
}

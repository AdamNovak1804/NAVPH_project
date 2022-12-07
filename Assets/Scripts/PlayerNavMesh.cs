using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// Inspired by https://www.youtube.com/watch?v=atCOd4o7tG4
public class PlayerNavMesh : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;
    public GameObject player;
    private void Awake() {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    private void Update()
    {
        // Add radius limit
        navMeshAgent.destination = player.gameObject.transform.position;

        // If close to combat, attack
    }
}
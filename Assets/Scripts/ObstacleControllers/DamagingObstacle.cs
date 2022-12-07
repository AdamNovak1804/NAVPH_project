using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagingObstacle : MonoBehaviour
{
    public float damage;

    private void OnCollisionEnter(Collision other) 
    {
        if (other.gameObject.name == "Astronaut") 
        {
            Debug.Log("Detect hit");
            Player player = other.gameObject.GetComponent<Player>();
            if (player != null) 
            {
                player.DrainHealth(damage);
            }
        }
    }
}

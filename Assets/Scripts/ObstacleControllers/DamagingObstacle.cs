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
            PlayerStats player = other.gameObject.GetComponent<PlayerStats>();
            PlayerController controller = other.gameObject.GetComponent<PlayerController>();

            if (player != null) 
            {
                player.DrainHealth(damage);

            }
        }
    }
}

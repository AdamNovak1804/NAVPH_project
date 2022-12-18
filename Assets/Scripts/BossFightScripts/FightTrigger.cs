using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightTrigger : MonoBehaviour
{
    public GameObject boss;
    public GameObject door;

    private Boss bossScript;

    void Start()
    {
        bossScript = boss.gameObject.GetComponent<Boss>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            door.SetActive(true);
            
            bossScript.startFight();
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightTrigger : MonoBehaviour
{
    public GameObject boss;

    private Boss bossScript;

    void Start()
    {
        bossScript = boss.gameObject.GetComponent<Boss>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            bossScript.startFight();
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }
}

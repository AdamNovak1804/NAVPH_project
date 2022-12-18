using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableScript : MonoBehaviour
{
    public GameObject spawnItem;

    public void DestroyObject() 
    {
        Object.Instantiate(spawnItem.gameObject, transform.position, Quaternion.Euler(-90,0,0));
        this.gameObject.SetActive(false);
        Object.Destroy(this.gameObject);
    }
}

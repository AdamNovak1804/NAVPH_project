using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableScript : MonoBehaviour
{
    public GameObject spawnItem;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void DestroyObject() 
    {
        Object.Instantiate(spawnItem.gameObject, transform.position, Quaternion.identity);
        this.gameObject.SetActive(false);
        Object.Destroy(this.gameObject);
    }
}

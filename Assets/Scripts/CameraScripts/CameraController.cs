using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;


public class CameraController : MonoBehaviour
{
   
    public GameObject player;

    public float minDist = 7.0f;
    public float maxDist = 7.0f;

    public float steps = 10.0f;

    public float angle = 35.0f;

    public Vector3 fwd;
    
    int currIP = 0;
    float time = 0.0f;

    Vector3 lookAt;

    List<(Vector3, int)> keyPositions = new List<(Vector3, int)>();
    List<Vector3> ipPositions = new List<Vector3>();
    
    int currDir = 1;
    Vector3 fromPos;

    Quaternion currentRot;
    Quaternion nextRot;

    // Start is called before the first frame update
    void Start()
    {

        //initialize position, get keyPositions, interpolate them, do stuff
        var scene = SceneManager.GetActiveScene();
        var rails = GameObject.FindGameObjectsWithTag("CameraRail").ToList();
        
        Debug.Log(rails.Count());

        foreach(var obj in rails)
        {
            Debug.Log(obj.GetComponent<Rails>().number);
            keyPositions.Add((obj.transform.position, obj.GetComponent<Rails>().number));
        }

        

        keyPositions = keyPositions.OrderBy(t => t.Item2).ToList();
       

        ipPositions = interpolate(keyPositions);
        

        this.transform.position = fromPos = ipPositions[currIP];


        fwd = ipPositions[currIP + 1] - this.transform.position;
        this.transform.rotation = Quaternion.LookRotation(fwd, Vector3.up) * Quaternion.Euler(angle, 0, 0);

        currentRot = Quaternion.LookRotation(this.transform.forward, Vector3.up) * Quaternion.Euler(angle, 0, 0);
        nextRot = Quaternion.LookRotation(ipPositions[currIP + 2] - ipPositions[currIP + 1], Vector3.up) * Quaternion.Euler(angle, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (currIP < (ipPositions.Count - currDir))
        {
            var dist = Math.Sqrt(Math.Pow(Vector3.Distance(player.transform.position, this.transform.position), 2)
            - Math.Pow((this.transform.position.y - player.transform.position.y), 2));

            if (dist >= maxDist)
            {
                Debug.Log("moving forward");
                moveCamera(1);
            }
            else if (dist <= minDist && currIP - 1 >= 0)
            {
                Debug.Log("moving backwards");
                moveCamera(-1);
            }
        }
    }

    void moveCamera(int newDir)
    {
        time += Time.deltaTime;

        var speed = player.GetComponent<PlayerController>().speed;
 
        if (currDir != newDir)
        {
            time = 0;
            fromPos = this.transform.position;
            
            currDir = newDir;

            nextRot = currentRot;
            currentRot = this.transform.rotation;
        }
        
        var dist = Vector3.Distance(fromPos, ipPositions[currIP + currDir]);

        this.transform.position = Vector3.Lerp(fromPos, ipPositions[currIP + currDir], time / (dist / speed));

        Debug.Log(currentRot);
        Debug.Log(nextRot);

        this.transform.rotation = Quaternion.Slerp(currentRot, nextRot, time / (dist / speed));
                

        if (time / (dist / speed) > 1)
        {
            time = 0;
            currIP += currDir;
            fromPos = ipPositions[currIP];

           

            if ((currIP + 1) >= ipPositions.Count())
            {
                return;
            }

            fwd = ipPositions[currIP + 1] - this.transform.position;

            var next = (currIP + 2) < ipPositions.Count() ?  ipPositions[currIP + 2] - ipPositions[currIP + 1] : ipPositions[currIP + 1] - ipPositions[currIP];
            
            if (currDir < 0)
            {
                currentRot = Quaternion.LookRotation(next, Vector3.up) * Quaternion.Euler(angle, 0, 0); 
                nextRot = Quaternion.LookRotation(fwd, Vector3.up) * Quaternion.Euler(angle, 0, 0);
                return;
            }
            currentRot = Quaternion.LookRotation(fwd, Vector3.up) * Quaternion.Euler(angle, 0, 0);
            nextRot = Quaternion.LookRotation(next, Vector3.up) * Quaternion.Euler(angle, 0, 0);
        }
    }
    List<Vector3> interpolate(List<(Vector3, int)> positions)
    {
        List<Vector3> interpolated = new List<Vector3>();
        
        if ((positions.Count - 1) % 2 != 0)
        {
            positions.Add(positions[positions.Count - 1]);
        }
              

        for (int i = 0; i < (positions.Count - 1); i += 2)
        {
            for (int j = 0; j < steps; j++)
            {
                float step = (float) j / steps;
                interpolated.Add(Vector3.Lerp(Vector3.Lerp(positions[i].Item1, positions[i + 1].Item1, step), Vector3.Lerp(positions[i + 1].Item1, positions[i + 2].Item1, step), step));

            }
        }
        return interpolated;
    }
}

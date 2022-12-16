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
    public float maxDist = 8.0f;

    public float steps = 20.0f;

    public float angle = 35.0f;

    public Vector3 fwd;
    
    int currIP = 0;
    float time = 0.0f;

    List<(Vector3, int)> keyPositions = new List<(Vector3, int)>();
    List<Vector3> ipPositions = new List<Vector3>();
    
    int currDir = 1;
    Vector3 fromPos;

    Quaternion currentRot;
    Quaternion nextRot;

    // Start is called before the first frame update
    void Start()
    {
        PowerUps.UpdateSpeed += ChangeFovToSpeed;

        //initialize position, get keyPositions, interpolate them, do stuff
        var scene = SceneManager.GetActiveScene();
        var rails = GameObject.FindGameObjectsWithTag("CameraRail").ToList();
        
        foreach(var obj in rails)
        {
            keyPositions.Add((obj.transform.position, obj.GetComponent<Rails>().number));
        }

        keyPositions = keyPositions.OrderBy(t => t.Item2).ToList();
       

        ipPositions = Interpolate(keyPositions);
        

        this.transform.position = fromPos = ipPositions[currIP];


        fwd = ipPositions[currIP + 1] - this.transform.position;
        this.transform.rotation = Quaternion.LookRotation(fwd, Vector3.up) * Quaternion.Euler(angle, 0, 0);

        currentRot = this.transform.rotation;
        nextRot = Quaternion.LookRotation(ipPositions[currIP + 2] - ipPositions[currIP + 1], Vector3.up) * Quaternion.Euler(angle, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (currIP < (ipPositions.Count - currDir))
        {
            var dist = Math.Sqrt(Math.Pow(Vector3.Distance(player.transform.position, this.transform.position), 2)
            - Math.Pow((this.transform.position.y - player.transform.position.y), 2));

            var angle = (Math.PI / 180) * Math.Abs(Vector2.Angle(new Vector2(player.transform.position.x - transform.position.x, player.transform.position.z - transform.position.z), new Vector2(fwd.x, fwd.z)));

            var trueDist = Math.Cos(angle) * dist;
            

            if (trueDist >= maxDist)
            {
                Debug.Log("moving forward");
                MoveCamera(1);
            }
            else if (trueDist <= minDist && currIP - 1 >= 0)
            {
                Debug.Log("moving backwards");
                MoveCamera(-1);
            }
        }
    }

    void MoveCamera(int newDir)
    {
        time += Time.deltaTime;

        var speed = player.GetComponent<PlayerMovement>().currentSpeed;
 
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

        this.transform.rotation = Quaternion.Lerp(currentRot, nextRot, time / (dist / speed));
                

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
    List<Vector3> Interpolate(List<(Vector3, int)> positions)
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

    private void ChangeFovToSpeed(bool enabled, float duration, float newSpeed)
    {
        if (enabled)
        {
            GetComponent<Camera>().fieldOfView *= 1.25f;
            return;
        }
        GetComponent<Camera>().fieldOfView *= 0.8f;
    }
}

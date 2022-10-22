using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Unity.VisualScripting;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;
using Vector3 = UnityEngine.Vector3;

public class CameraController : MonoBehaviour
{
    
    float time = 0.0f;

    GameObject player;

    float steps = 10.0f;
    int currIP = 0;

    Vector3 lookAt;

    List<Vector3> keyPositions = new List<Vector3>();
    List<Vector3> ipPositions = new List<Vector3>();
    List<Vector3> keyCenters = new List<Vector3>();
    List<Vector3> ipCenters = new List<Vector3>();

    bool forward = true;
    Vector3 fromPos;
    Vector3 fromCenter;

    // Start is called before the first frame update
    void Start()
    {

        //initialize position, get keyPositions, interpolate them, do stuff
        /*var scene = SceneManager.GetActiveScene();
        List<GameObject> objects = GameObject.FindGameObjectsWithTag("CameraRail").ToList();

        foreach(GameObject obj in objects)
        {
            keyPositions.Add(obj.transform.position);
        }*/
        keyPositions.Add(new Vector3(6.26f, 7f, -12.83f));
        keyPositions.Add(new Vector3(6.36f, 7f, -4.83f));
        keyPositions.Add(new Vector3(6.36f, 7f, -0.32f));
        keyPositions.Add(new Vector3(8.9f,7f, 6.83f));
        keyPositions.Add(new Vector3(16.36f, 7f, 13f));
        keyPositions.Add(new Vector3(24f, 7f, 17f));
        keyPositions.Add(new Vector3(31f, 7f, 26f));

        keyCenters.Add(new Vector3(6.36f, 0f, -4.83f));
        keyCenters.Add(new Vector3(6.36f, 0f, -0.32f));
        keyCenters.Add(new Vector3(8.9f, 0f, 6.83f));
        keyCenters.Add(new Vector3(16.36f, 0f, 13f));
        keyCenters.Add(new Vector3(24f, 0f, 17f));
        keyCenters.Add(new Vector3(31f, 0f, 26f));
        keyCenters.Add(new Vector3(35f, 0f, 30f));

        ipPositions = interpolate(keyPositions); 
        ipCenters = interpolate(keyCenters);

        this.transform.position = fromPos = ipPositions[currIP];
        fromCenter = ipCenters[currIP];

        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        
        if (currIP < (ipPositions.Count - 1))
        {

            if (Vector3.Distance(player.transform.position, this.transform.position) > 10)
            {   
                time += Time.deltaTime;
                
                if (!forward)
                {
                    time = 0;
                    fromPos = this.transform.position;
                    fromCenter = lookAt;
                    forward = true;
                }
                            
                var dist = Vector3.Distance(fromPos, ipPositions[currIP + 1]);

                this.transform.position = Vector3.Lerp(fromPos, ipPositions[currIP + 1], time/ (dist / 3));
                
                lookAt = Vector3.Lerp(fromCenter, ipCenters[currIP + 1], time / (dist / 3));
                this.transform.LookAt(lookAt);

                if (time/ (dist / 3) > 1)
                {
                    time = 0;
                    currIP++;
                    fromPos = ipPositions[currIP];
                    fromCenter = ipCenters[currIP];
                }
            }   
            else if (Vector3.Distance(player.transform.position, this.transform.position) < 7 && currIP - 1 > 0) {

                time += Time.deltaTime;
                
                if (forward)
                {
                    time = 0;
                    fromPos = this.transform.position;
                    fromCenter = lookAt;
                    forward = false;
                }
                
                var dist = Vector3.Distance(fromPos, ipPositions[currIP - 1]);

                this.transform.position = Vector3.Lerp(fromPos, ipPositions[currIP - 1], time / (dist / 3));

                lookAt = Vector3.Lerp(fromCenter, ipCenters[currIP - 1], time / (dist / 3));
                this.transform.LookAt(lookAt);

                if (time / (dist / 3) > 1)
                {
                    time = 0;
                    currIP--;
                    fromPos = ipPositions[currIP];
                    fromCenter = ipCenters[currIP];
                }
            }
        }

    }

    List<Vector3> interpolate(List<Vector3> positions)
    {
        List<Vector3> interpolated = new List<Vector3>();
        Debug.Log(positions.Count);
        if ((positions.Count - 1) % 2 != 0)
        {
            positions.Add(positions[positions.Count - 1]);
        }

        Debug.Log(positions.Count);

        for (int i = 0; i < (positions.Count - 1); i += 2)
        {
            for (int j = 0; j < steps; j++)
            {
                float step = (float) j / steps;
                interpolated.Add(Vector3.Lerp(Vector3.Lerp(positions[i], positions[i + 1], step), Vector3.Lerp(positions[i + 1], positions[i + 2], step), step));

            }
        }
        return interpolated;
    }
}

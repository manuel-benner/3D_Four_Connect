using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kamera : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // C -> Kamera vom Betrachter weg
        // V -> Kamera zum Betrachter hin
        if (Input.GetKey(KeyCode.C))
        {
            if(!(transform.position.y < 33))
            {
                transform.Translate(0, -0.1f, 0.4f);
                var currEulerAngles = transform.eulerAngles;
                currEulerAngles.x -= 0.08f;
                transform.rotation = Quaternion.Euler(currEulerAngles);
            }
        }
        
        else if (Input.GetKey(KeyCode.V))
        {
            if(!(transform.position.y > 200))
            {
                transform.Translate(0, 0.1f, -0.4f);
                var currEulerAngles = transform.eulerAngles;
                currEulerAngles.x += 0.08f;
                transform.rotation = Quaternion.Euler(currEulerAngles);
            }            
        }
        

        // J -> Kamera nach links um das Spielfeld herum
        // K -> Kamera nach rechts um das Spielfeld herum
        if (Input.GetKey(KeyCode.J))
            transform.RotateAround(new Vector3(0, 0, 0),
            new Vector3(0, 1, 0), 0.25f);
        else if (Input.GetKey(KeyCode.K))
            transform.RotateAround(new Vector3(0, 0, 0),
            new Vector3(0, 1, 0), -0.25f);

    }
}

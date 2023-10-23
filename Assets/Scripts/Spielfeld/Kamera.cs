using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kamera : MonoBehaviour
{
    float inputFactor;
    // Start is called before the first frame update
    void Start()
    {
        inputFactor = 150.0f;
    }

    // Update is called once per frame
    void Update()
    {
        float yChange;
        float zChange;
        float angleChange;

        // C -> Kamera vom Betrachter weg
        // V -> Kamera zum Betrachter hin
        if (Input.GetKey(KeyCode.C))
        {
            if (!(transform.position.y < 33))
            {
                yChange = -0.5f * inputFactor * Time.deltaTime; // Adjust for desired speed
                zChange = 0.7f * inputFactor * Time.deltaTime;  // Adjust for desired speed
                transform.Translate(0, yChange, zChange);
                var currEulerAngles = transform.eulerAngles;
                currEulerAngles.x -= 0.25f * inputFactor * Time.deltaTime; // Adjust for desired rotation speed
                transform.rotation = Quaternion.Euler(currEulerAngles);
            }
        }
        else if (Input.GetKey(KeyCode.V))
        {
            if (!(transform.position.y > 150))
            {
                yChange = 0.5f * inputFactor * Time.deltaTime;  // Adjust for desired speed
                zChange = -0.7f * inputFactor * Time.deltaTime; // Adjust for desired speed
                transform.Translate(0, yChange, zChange);
                var currEulerAngles = transform.eulerAngles;
                currEulerAngles.x += 0.25f * inputFactor * Time.deltaTime;  // Adjust for desired rotation speed
                transform.rotation = Quaternion.Euler(currEulerAngles);
            }
        }

        // J -> Kamera nach links um das Spielfeld herum
        // K -> Kamera nach rechts um das Spielfeld herum
        if (Input.GetKey(KeyCode.J))
        {
            angleChange = 0.5f * inputFactor * Time.deltaTime; // Adjust for desired rotation speed
            transform.RotateAround(new Vector3(0, 0, 0), new Vector3(0, 1, 0), angleChange);
        }
        else if (Input.GetKey(KeyCode.K))
        {
            angleChange = -0.5f * inputFactor * Time.deltaTime; // Adjust for desired rotation speed
            transform.RotateAround(new Vector3(0, 0, 0), new Vector3(0, 1, 0), angleChange);
        }
    }
}

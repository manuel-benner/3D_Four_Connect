using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCamera : MonoBehaviour
{
    float yChange;
    float zChange;
    float angleChange;

    float inputFactor;
    // Start is called before the first frame update
    void Start()
    {
        inputFactor = 150.0f;
    }

    // Update is called once per frame
    void Update()
    {
        angleChange = 0.05f * inputFactor * Time.deltaTime;
        transform.RotateAround(new Vector3(0, 0, 0),
        new Vector3(0, 1, 0), angleChange);
    }
}

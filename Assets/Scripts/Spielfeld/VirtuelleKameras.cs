using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VirtuelleKameras : MonoBehaviour
{
    private int startCam;
    private int currentCam;
    private float angleChange;
    private float inputFactor;
    private Vector3 mouseDownPosLastFrame;

    private GameObject[] cameras;

    // Start is called before the first frame update
    void Start()
    {
        startCam = 4;
        cameras = FindGameObjectsWithTagSorted("VirtualCameras");
        InitializeCams();
        inputFactor = 10.0f;
        mouseDownPosLastFrame = Input.mousePosition;
    }


    private GameObject[] FindGameObjectsWithTagSorted(string tag)
    {
        GameObject[] sortedCams = GameObject.FindGameObjectsWithTag(tag);
        Array.Sort(sortedCams, (a, b) => a.name.CompareTo(b.name));
        return sortedCams;
    }

    private void InitializeCams()
    {
        foreach(GameObject cam in  cameras)
        {
            cam.SetActive(false);
        }
        cameras[startCam].SetActive(true);
        currentCam = startCam;
    }


    // Update is called once per frame
    void Update()
    {
        handleZoom();
        handleOrbit();
        mouseDownPosLastFrame = Input.mousePosition;
    }

    private void handleZoom()
    {
        if (Input.mouseScrollDelta == new Vector2(0.00f, 1.00f))
        {
            zoomIn();
        }
        else if (Input.mouseScrollDelta == new Vector2(0.00f, -1.00f))
        {
            zoomOut();
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            zoomIn();
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            zoomOut();
        }
    }

    private void zoomOut()
    {
        if (currentCam < 11)
        {
            cameras[currentCam].SetActive(false);
            currentCam++;
            cameras[currentCam].SetActive(true);
        }
    }

    private void zoomIn()
    {
        if (currentCam >= 1)
        {
            cameras[currentCam].SetActive(false);
            currentCam--;
            cameras[currentCam].SetActive(true);
        }
    }

    private void handleOrbit()
    {
        if (Input.GetKey(KeyCode.A))
        {
            angleChange = 0.5f * inputFactor * Time.deltaTime;
            orbitAllCameras(angleChange);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            angleChange = -0.5f * inputFactor * Time.deltaTime;
            orbitAllCameras(angleChange);
        }
        else if (Input.GetMouseButton(1))
        {
            if (Input.mousePosition.x < mouseDownPosLastFrame.x)
            {
                float deltaMouse = (Input.mousePosition.x - mouseDownPosLastFrame.x);
                angleChange = 0.5f * inputFactor * Time.deltaTime * deltaMouse;
                orbitAllCameras(angleChange);
            }
            else if (Input.mousePosition.x > mouseDownPosLastFrame.x)
            {
                float deltaMouse = (Input.mousePosition.x - mouseDownPosLastFrame.x) * -1;
                angleChange = -0.5f * inputFactor * Time.deltaTime * deltaMouse;
                orbitAllCameras(angleChange);
            }
        }
    }

    private void orbitAllCameras(float delta)
    {
        foreach (GameObject cam in cameras)
        {
            cam.transform.RotateAround(new Vector3(0, 0, 0), new Vector3(0, 1, 0), delta);
        }
    }
}

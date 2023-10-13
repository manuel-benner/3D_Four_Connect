using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HostWait : MonoBehaviour
{
    private float StartTime;

    [SerializeField] private string levelString;

    [SerializeField] private float? secondsUntilTimeout;

    [SerializeField] GameObject NetworkSelection;

    //private wait networkEventClass;


    private void OnEnable()
    {
        StartTime = Time.time;
        if (secondsUntilTimeout == null)
        {
            secondsUntilTimeout = 5f;
        }
        if(NetworkSelection == null)
        {
            Debug.Log("No Game obj network selection given");
        }
        // start
        NetworkManager.Singleton.StartHost();
    }


    // Update is called once per frame
    void Update()
    {
        
        // calculate how much time passed since start
        int clientsConnected = NetworkManager.Singleton.ConnectedClientsList.Count;
        if (clientsConnected == 2)
        {
            StartGame();
        }
    }
    public void StartGame()
    {
        if (NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsHost)
        {
            Debug.Log("Loading Scene: " + levelString);

            NetworkManager.Singleton.SceneManager.LoadScene(levelString, LoadSceneMode.Single);
        }
        else
        {
            Debug.Log("Error: Must be Server or Host to load a scene.");
        }
    }

    public void Exit()
    {
        NetworkManager.Singleton.Shutdown();
        NetworkSelection.SetActive(true);
        gameObject.SetActive(false);
    }
}

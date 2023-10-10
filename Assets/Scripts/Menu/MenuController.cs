using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

public class MenuController : MonoBehaviour
{
    public string levelString;

    private readonly NetworkManager networkManager = NetworkManager.Singleton;

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

    public void HostBtn()
    {
        NetworkManager.Singleton.StartHost();
        StartGame();
    }

    public void JoinBtn()
    {
        NetworkManager.Singleton.StartClient();
    }

    public void ExitBtn()
    {
        Application.Quit();
    }
}

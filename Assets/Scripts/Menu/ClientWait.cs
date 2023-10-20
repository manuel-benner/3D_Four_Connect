using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using static MainMenuController;
using UnityEngine.Events;

public class ClientWait : MonoBehaviour
{
    private float StartTime;
    private bool clientconnected;

    [SerializeField] private float? secondsUntilTimeout;

    [SerializeField] private GameObject ErrorBox;

    [SerializeField] GameObject NetworkGameSelection;

    private NetworkEventListener networkEventClass;

    // Start is called before the first frame update
    private void OnEnable()
    {
        clientconnected = false;
        StartTime = Time.time;
        if (secondsUntilTimeout == null)
        {
            secondsUntilTimeout = 5f;
        }
        if (ErrorBox == null)
        {
            Debug.Log("InvalidIpScreen not set as class parameter");
        }
        // subscribe to onconnection event
        networkEventClass = new NetworkEventListener(this);
        // start Client
        NetworkManager.Singleton.StartClient();
    }

    private void OnDisable()
    {
        networkEventClass.RemoveEvents();
    }


    // Update is called once per frame
    void Update()
    {
        // calculate how much time passed since start
        float timeSinceStartOfScript = Time.time - StartTime;

        if(timeSinceStartOfScript > secondsUntilTimeout && ! clientconnected)
        {
            Error("Connection timed out");
        }
    }
    private void SetupErrorBox(string ErrorMessage, UnityAction ButtonAction)
    {
        MenuElement ErrorBoxSetup = new MenuElement(ErrorBox);

        ErrorBoxSetup.ConfigureTextElement("ErrorMessage", ErrorMessage);

        ErrorBoxSetup.ConfigureButton("AcceptBtn", ButtonAction);

    }

    private void Error(string message)
    {
        NetworkManager.Singleton.Shutdown();
        UnityAction BackToNetwork = BackToNetworkSelection;
        SetupErrorBox("Connection timed out",BackToNetwork);
        gameObject.SetActive(false);
        ErrorBox.SetActive(true);
    }

    private void BackToNetworkSelection()
    {
        gameObject.SetActive(false);
        NetworkGameSelection.SetActive(true);
        ErrorBox.SetActive(false) ;
    }

    public class NetworkEventListener : NetworkBehaviour
    {
        private ClientWait clientWait;
        public NetworkEventListener(ClientWait outerClass)
        {
            clientWait = outerClass;
        }

        private void Start()
        {
            if (IsClient)
            {
                NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
                NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
            }
        }
        public void RemoveEvents()
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        }

        private void OnClientDisconnected(ulong obj)
        {
            Debug.Log($"Client: {obj} Disconnected");
            if (NetworkManager.Singleton.LocalClientId == obj)
            {
                clientWait.Error("Got disconnected from server");
            }
        }

        private void OnClientConnected(ulong obj)
        {
            clientWait.clientconnected = true;
        }
    }

}

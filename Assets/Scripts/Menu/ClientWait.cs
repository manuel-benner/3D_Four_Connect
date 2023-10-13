using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ClientWait : MonoBehaviour
{
    private float StartTime;
    private bool clientconnected;

    [SerializeField] private float? secondsUntilTimeout;

    [SerializeField] private GameObject InvalidIpScreen;

    private wait networkEventClass;

    // Start is called before the first frame update
    private void OnEnable()
    {
        clientconnected = false;
        StartTime = Time.time;
        if (secondsUntilTimeout == null)
        {
            secondsUntilTimeout = 5f;
        }
        if (InvalidIpScreen == null)
        {
            Debug.Log("InvalidIpScreen not set as class parameter");
        }
        // subscribe to onconnection event
        networkEventClass = new wait(this);
        // start Client
        NetworkManager.Singleton.StartClient();
    }


    // Update is called once per frame
    void Update()
    {
        // calculate how much time passed since start
        float timeSinceStartOfScript = Time.time - StartTime;

        if(timeSinceStartOfScript > secondsUntilTimeout && ! clientconnected)
        {
            CaseInvalidIp();
        }
    }

    private void CaseInvalidIp()
    {
        NetworkManager.Singleton.Shutdown();
        InvalidIpScreen.SetActive(true);
        gameObject.SetActive(false);
    }
    public class wait : NetworkBehaviour
    {
        private ClientWait clientWait;
        public wait(ClientWait outerClass)
        {
            clientWait = outerClass;
        }

        private void Start()
        {
            if (IsClient)
            {
                NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            }
        }

        private void OnClientConnected(ulong obj)
        {
            clientWait.clientconnected = true;
        }
    }

}

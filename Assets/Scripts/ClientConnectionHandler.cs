using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ClientConnectionHandler : NetworkBehaviour
{
    [SerializeField] int MaxClients;

    NetworkManager networkManager;
    private void Start()
    {
        networkManager = NetworkManager.Singleton;
    }
    public override void OnNetworkSpawn()
    {
        if (IsServer || IsHost)
        {
            NetworkManager.ConnectionApprovalCallback = ConnectionApprovalCallback;
        }
    }

    private void ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        int ConnectedClients = networkManager.ConnectedClientsList.Count;

        if (MaxClients > ConnectedClients)
        {
            response.Approved = true;
        }
        else
        {
            response.Reason = "Game is full";
            response.Approved = false;
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CustomNetworkmanager : NetworkManager
{
    [SerializeField] private bool clientConnected;

    private void Start()
    {
        if(IsServer|| IsHost)
        {
            OnClientConnectedCallback += NewClientConnected;
        }

    }

   
    private void NewClientConnected(ulong obj)
    {
        ClientRpcParams clientRpcParams = new ClientRpcParams()
        {
            Send =new ClientRpcSendParams
            {
                TargetClientIds = new[] {obj},
            }
        };
    }

    [ClientRpc]
    private void ClientConnected( ClientRpcParams clientRpcParams = default)
    {
        if (IsHost || IsServer) return;

        clientConnected = true;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkEvents : NetworkBehaviour
{
    private UI_Controller ui_Controller;

    private void Start()
    {
        this.ui_Controller = gameObject.GetComponent<UI_Controller>();
    }

    [ClientRpc]
    public void ResetUIClientRpc()
    {
        ui_Controller.GameObjectsToStartState();
    }

    [ClientRpc]
    public void OtherPlayerLeftClientRpc()
    {
        ui_Controller.OnOpponentLeft();
    }

    [ServerRpc(RequireOwnership = false)]
    public void LeaveMessageToOtherPlayerServerRpc()
    {
        OtherPlayerLeftClientRpc();
    }
}

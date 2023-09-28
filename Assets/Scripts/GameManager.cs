using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class GameManager : NetworkBehaviour
{
    [SerializeField] private GameObject kugelBlauPrefab;
    [SerializeField] private GameObject kugelRotPrefab;

    public void SpawnBall(Vector3 position)
    {
        if(IsClient)
        {
            SpawnBallServerRpc(position, new ServerRpcParams());
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnBallServerRpc(Vector3 position, ServerRpcParams serverRpcParams)
    {
        GameObject spawnedKugelObject = Instantiate(kugelBlauPrefab, position, Quaternion.identity);
        spawnedKugelObject.GetComponent<NetworkObject>().Spawn(true);
        Debug.Log("Spielstein gespawnt für Spieler Nr. " + serverRpcParams.Receive.SenderClientId + ".");
    }

    [ClientRpc]
    private void SendSpawnToClientRpc()
    {

    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class GameManager : NetworkBehaviour
{
    [SerializeField] private GameObject kugelBlauPrefab;
    [SerializeField] private GameObject kugelRotPrefab;

    public override void OnNetworkSpawn()
    {
        if (!NetworkManager.Singleton.IsServer)
        {
            Spielfeld.Instance.myStatus = Spielfeld.Status.opponentTurn;
            Spielfeld.Instance.player = 1;
            Debug.Log(Spielfeld.Instance.myStatus);
            return;
        }
        Debug.Log(Spielfeld.Instance.myStatus);
        Spielfeld.Instance.myStatus = Spielfeld.Status.myTurn;
        Spielfeld.Instance.player = 0;
    }

    public void SpawnBall(Vector3 position, string stringIdentifier)
    {
        if(IsClient)
        {
            SpawnBallServerRpc(position, stringIdentifier, new ServerRpcParams());
        }
    }

    public void ResetPlayfield()
    {
        ResetPlayfieldServerRpc();
    }


    [ServerRpc(RequireOwnership = false)]
    private void ResetPlayfieldServerRpc()
    {
        ResetClientRpc();
    }

    [ClientRpc]
    private void ResetClientRpc()
    {
        Spielfeld.Instance.resetPlayfieldThisClient();
    }


    [ServerRpc(RequireOwnership = false)]
    private void SpawnBallServerRpc(Vector3 position, string stringIdentifier, ServerRpcParams serverRpcParams)
    {
        ulong clientId = serverRpcParams.Receive.SenderClientId;

        GameObject prefab = clientId == 1 ? kugelRotPrefab: kugelBlauPrefab;

        GameObject spawnedKugelObject = Instantiate(prefab, position, Quaternion.identity);
        spawnedKugelObject.GetComponent<NetworkObject>().Spawn(true);
        SendSpawnToClientRpc(stringIdentifier);
        Debug.Log("Spielstein gespawnt für Spieler Nr. " + clientId + ".");
    }

    [ClientRpc]
    private void SendSpawnToClientRpc(string sphereIdentifier)
    {
        if(Spielfeld.Instance.myStatus == Spielfeld.Status.myTurn)
        {
            Spielfeld.Instance.placedSphere = false;
            Spielfeld.Instance.myStatus = Spielfeld.Status.opponentTurn;
            Spielfeld.Instance.NewTurn();
        }
        else if(Spielfeld.Instance.myStatus == Spielfeld.Status.opponentTurn)
        {
            Spielfeld.Instance.placedSphere = false;
            Spielfeld.Instance.myStatus = Spielfeld.Status.myTurn;
            Spielfeld.Instance.HandleSphereSpawn(sphereIdentifier);
            Spielfeld.Instance.NewTurn();
        }
        Debug.Log(Spielfeld.Instance.myStatus);
    }

}

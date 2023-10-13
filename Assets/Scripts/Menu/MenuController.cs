using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Unity.Netcode;
using System;
using Unity.Netcode.Transports.UTP;
using System.Net;
using TMPro;

public class MenuController : NetworkBehaviour
{
    public string levelString;

    #region Local Network


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

    public void JoinBtn(GameObject InvalidIpScreen)
    {

        //if (NetworkManager.Singleton.StartClient()) Debug.Log("Client Started");

        //Debug.Log("Error: Invalid Ip inserted cannot join a game");
        //InvalidIpScreen.gameObject.SetActive(true);
        
        try
        {
            NetworkManager.Singleton.StartClient();
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
            InvalidIpScreen.gameObject.SetActive(true);
        }
    }

    public void SetAdress(GameObject InvalidIpScreen)
    {
        GameObject inputObj = getGameobj("InputUI", "IpToJoin");
        string ipAdress = inputObj.GetComponent<TMP_InputField>().text;
        if (! SetJoinAdress(ipAdress))
        {
            Debug.Log("Error: Invalid Ip inserted cannot join a game");
            InvalidIpScreen.gameObject.SetActive(true);
        }
    }


    public bool SetJoinAdress(string IpAdress)
    {
        // validating Ip adress
        if (! IPAddress.TryParse(IpAdress, out IPAddress _)) return false;

        //setting valid Ip
        NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address = IpAdress;
        return true;
    }

    #endregion

    public void StartHotseat()
    {
        Debug.Log("Starting Hotseat game");
        throw new NotImplementedException();
    }

    public void ExitBtn()
    {
        Application.Quit();
    }

    private GameObject getGameobj(string Tag, string Name)
    {
        GameObject objToFind = null;
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag(Tag))
        {
            if (obj.name == Name)
            {
                objToFind = obj;
            }
        }
        if (objToFind == null)
        {
            throw new Exception($"Could not find the game object {Name} with the tag {Tag}");
        }
        return objToFind;
    }

    public void ResetInputField(GameObject toReset)
    {
        if (toReset.TryGetComponent<TMP_InputField>(out TMP_InputField textToReset))
        {
            textToReset.text = "";
            return;
        }
        Debug.Log("called reset TextMesh method on a object without Textmesh: " + textToReset.name);
    }

}

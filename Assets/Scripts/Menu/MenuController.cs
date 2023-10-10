using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Unity.Netcode;

public class MenuController : MonoBehaviour
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
        GameObject inputObj = getGameobj("InputUI", "IpToJoin");
        string ipAdress = inputObj.GetComponent<TextMeshProUGUI>().text;
        if (SetJoinAdress(ipAdress))
        {
            try
            {
                NetworkManager.Singleton.StartClient();
            }
            catch
            {
                Debug.Log("Error: Invalid Ip inserted cannot join a game");
                InvalidIpScreen.gameObject.SetActive(true);
                //TODO: reset NetworkManager
            }

        }
        Debug.Log("Error: Invalid Ip inserted cannot join a game");
        InvalidIpScreen.gameObject.SetActive(true);
    }

    public void ResetTextMeshObj(GameObject toReset)
    {
        if (toReset.gameObject.TryGetComponent<TextMeshPro>(out TextMeshPro textToReset))
        {
            textToReset.text = "";
        }
        Debug.Log("called reset TextMesh method on a object without Textmesh: " + textToReset.name);
    }


    public bool SetJoinAdress(string IpAdress)
    {
        // validating Ip adress
        if (! IPAddress.TryParse(IpAdress, out IPAddress addr)) return false;

        //setting valid Ip
        NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address = IpAdress;
        return true;
    }

    #endregion

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

}

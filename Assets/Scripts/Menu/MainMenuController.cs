using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Assets.Scripts;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] GameObject root;
    [SerializeField] GameObject NewGameSelection;
    [SerializeField] GameObject NetworkGameSelection;
    [SerializeField] GameObject HostWaitScreen;
    [SerializeField] GameObject JoinScreen;
    [SerializeField] GameObject ClientWaitScreen;
    [SerializeField] GameObject ErrorBox;
    [SerializeField] GameObject NetworkManagerObj;


    // Start is called before the first frame update
    void Start()
    {
        
        SetupGameObjects();

        // Setting up all button logic 
        SetupMainMenu();
    }

    private void OnEnable()
    {
        SetupGameObjects();
    }

    private void SetupGameObjects()
    {
        // Deactivating all MainMenu objects except for root which is the object to be interacted with first
        root.SetActive(true);
        NewGameSelection.SetActive(false);
        NetworkGameSelection.SetActive(false);
        HostWaitScreen.SetActive(false);
        JoinScreen.SetActive(false);
        ClientWaitScreen.SetActive(false);
        ErrorBox.SetActive(false);
        NetworkManagerObj.SetActive(false);
    }



    /// <summary>
    /// Setting all Button functionality in code 
    /// </summary>
    private void SetupMainMenu()
    {
        SetupRoot();
        SetupNewGame();
        SetupNetworkGameSelection();
        SetupJoinScreen();
        SetupHostWait();
    }

    #region RootSetup
    private void SetupRoot()
    {
        root.gameObject.AddComponent<Assets.Scripts.MenuElement>();
        Assets.Scripts.MenuElement rootSetup = root.GetComponent<Assets.Scripts.MenuElement>();
        rootSetup.ChangeToGameObject("NewGameBtn", NewGameSelection);

        UnityAction Exit = ExitBtn;
        rootSetup.ConfigureButton("ExitBtn", Exit);
    }
    public void ExitBtn()
    {
        Application.Quit();
    }

    #endregion

    #region NewGameSetup
    
    private void SetupNewGame()
    {
        Assets.Scripts.MenuElement NewGameSelectionSetup = NewGameSelection.AddComponent<Assets.Scripts.MenuElement>();

        NewGameSelectionSetup.ConfigureButton("LocalGame", StartHotseat);

        NewGameSelectionSetup.ChangeToGameObject("NetworkGame", NetworkGameSelection);

        NewGameSelectionSetup.ChangeToGameObject("Back", root);

    }
    public void StartHotseat()
    {
        SceneManager.LoadScene("Spielfeld_Hotseat",LoadSceneMode.Single);
    }

    private void SetScene(string sceneName, bool isActive)
    {
        foreach (GameObject obj in SceneManager.GetSceneByName(sceneName).GetRootGameObjects())
        {
            obj.SetActive(isActive);
        }
    }

    #endregion

    #region NetworkGameSelectionSetup

    private void SetupNetworkGameSelection()
    {
        Assets.Scripts.MenuElement NetworkGameSelectionSetup = NetworkGameSelection.AddComponent<Assets.Scripts.MenuElement>();

        NetworkGameSelectionSetup.ChangeToGameObject("CreateGame", HostWaitScreen);

        NetworkGameSelectionSetup.ChangeToGameObject("JoinGame", JoinScreen);

        NetworkGameSelectionSetup.ChangeToGameObject("Back", NewGameSelection);

    }

    #endregion

    #region JoinScreenSetup
    
    private void SetupJoinScreen()
    {

        Assets.Scripts.MenuElement JoinScreenSetup = JoinScreen.AddComponent<Assets.Scripts.MenuElement>();

        UnityAction TakeAdress = SetAdress;
        JoinScreenSetup.ConfigureButton("JoinGame", TakeAdress);
        JoinScreenSetup.ChangeToGameObject("Back", NetworkGameSelection);
    }

    private void SetAdress()
    {
        string ipAdress = JoinScreen.GetComponentInChildren<TMP_InputField>().text;
        if (!SetJoinAdress(ipAdress))
        {
            Debug.Log("Error: Invalid Ip inserted cannot join a game");
            UnityAction BackToNetworkSelect = ErrorBoxBackToNetworkGameSelection;
            SetupErrorBox("Invalid Ip inserted, cannot join a game", BackToNetworkSelect);
            ErrorBox.gameObject.SetActive(true);
            JoinScreen.SetActive(false);
        }
        else
        {
            JoinScreen.SetActive(false);
            ClientWaitScreen.SetActive(true);
        }
    }
    private bool SetJoinAdress(string IpAdress)
    {
        // validating Ip adress
        if (!IPAddress.TryParse(IpAdress, out IPAddress _)) return false;

        //setting valid Ip
        NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address = IpAdress;
        return true;
    }

    private void ErrorBoxBackToNetworkGameSelection()
    {
        ErrorBox.gameObject.SetActive(false);
        NetworkGameSelection.SetActive(true);
    }

    #endregion

    #region Setup ErrorBox
    // Every setup method is called except this one, because the error box has to be set up before every use (so it is reusable)
    private void SetupErrorBox(string ErrorMessage, UnityAction ButtonAction)
    {
        Assets.Scripts.MenuElement ErrorBoxSetup = ErrorBox.AddComponent<Assets.Scripts.MenuElement>();

        ErrorBoxSetup.ConfigureTextElement("ErrorMessage",ErrorMessage);

        ErrorBoxSetup.ConfigureButton("AcceptBtn", ButtonAction);

    }


    #endregion

    #region SetupHostWait

    private void SetupHostWait()
    {
        Assets.Scripts.MenuElement HostWaitSetup = HostWaitScreen.AddComponent<Assets.Scripts.MenuElement>();
        UnityAction CancelHosting = HostWaitScreen.GetComponent<HostWait>().Exit;
        HostWaitSetup.ConfigureButton("Back", CancelHosting);

    }

    #endregion


}

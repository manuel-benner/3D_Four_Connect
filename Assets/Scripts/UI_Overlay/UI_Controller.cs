using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.UI.Button;

public class UI_Controller: MonoBehaviour
{
    [SerializeField] GameObject PauseMenu;
    [SerializeField] GameObject PlayerTurn;
    [SerializeField] GameObject ConfirmationScreen;
    [SerializeField] GameObject MessageBox;
    [SerializeField] bool NetworkActivated;

    private delegate void onOpponentLeft();
    private event onOpponentLeft OnOpponentLeft;

    internal NetworkEvents networkEvents;

    internal bool menuCallable;

    private void OnEnable()
    {
        // deactivating every GameObject, except PlayerTurn
        SetupGameObjects();
    }

    private void OnDisable()
    {
        if (NetworkActivated)
        {
            OnOpponentLeft -= OpponentLeftMessagebox;
            Spielfeld.OnReset -= OnResetMessagebox;
            Spielfeld.OnWin -= OnWinMessagebox;
            Spielfeld.OnDraw -= OnDrawAction;
            Spielfeld.OnNewTurn -= UpdatePlayerTurn;
        }
        else
        {
            Spielfeld_Hotseat.OnWin -= OnWinMessagebox;
            Spielfeld_Hotseat.OnDraw -= OnDrawAction;
            Spielfeld_Hotseat.OnNewTurn -= UpdatePlayerTurn;
        }
    }


    // Start is called before the first frame update
    private void Start()
    {
        if (PauseMenu == null) throw new Exception("PauseMenu is not given");
        if (PlayerTurn == null) throw new Exception("PlayerTurn is not given");
        if (ConfirmationScreen == null) throw new Exception("ConfirmationScreen is not given");
        if (MessageBox == null) throw new Exception("MessageBox is not given");

        // deactivating every GameObject, except PlayerTurn
        SetupGameObjects();
        //// setting all confirmation events
        //SetupConfirmationScreeen();
        // setting all Text elements and Button obClick methods
        PauseMenuSetup();
        // setting the messagebox onclick method to close the messagebox
        MessageboxSetup();
        menuCallable = true;

        if (NetworkActivated)
        {
            OnOpponentLeft += OpponentLeftMessagebox;
            networkEvents = new NetworkEvents(this);
            Spielfeld.OnReset += OnResetMessagebox;
            Spielfeld.OnWin += OnWinMessagebox;
            Spielfeld.OnDraw += OnDrawAction;
            Spielfeld.OnNewTurn += UpdatePlayerTurn;
        }
        else
        {
            Spielfeld_Hotseat.OnWin += OnWinMessagebox;
            Spielfeld_Hotseat.OnDraw += OnDrawAction;
            Spielfeld_Hotseat.OnNewTurn += UpdatePlayerTurn;
            networkEvents = null;
        }
        StartCoroutine(SetPlayerTurn());
    }

    IEnumerator SetPlayerTurn()
    {
        yield return new WaitForSeconds(Time.deltaTime);
        UpdatePlayerTurn();
    }

    private void OnResetMessagebox()
    {
        CustomMessagebox("The game has been reset", SetupGameObjects);
    }

    private void OnDrawAction()
    {
        if (NetworkActivated)
        {
            if (NetworkManager.Singleton.IsClient)
            {
                CustomMessagebox("The game ended in a draw, wait for the host to restart the game");
            }else if (NetworkManager.Singleton.IsHost)
            {
                CustomConfirmationScreen("The game ended in a draw, do you want to start another game", ButtonMethodRestartGame, ButtonMethodLeaveGame);
            }
        }
    }

    private void OpponentLeftMessagebox()
    {
        MessageBox.SetActive(true);
        UnityAction playerleftMethod = ButtonMethodLeaveGame;
        MessageBox.GetComponent<SetMessageBox>().SetUpMessageBox("Other player left", GameLeftMessagebox);
    }

    private void OnWinMessagebox()
    {
        Debug.Log($"Winner ...");
    }

    private void PauseMenuSetup()
    {
        Button[] PauseMenuButtons = GetPauseMenuButtons();
        //setting the action, that is called in the onclick method 

        Button RestartButton = GetButtonByName(PauseMenuButtons, "RestartGame");
        if (NetworkActivated)
        {
            if (NetworkManager.Singleton == null)
            {
                //throw new Exception("No Network Manager given");
            }
            else if (NetworkManager.Singleton.IsHost)
            {
                RestartButton.enabled = true;
            }
            else if (NetworkManager.Singleton.IsClient)
            {
                RestartButton.enabled = false;
            }
        }
        else
        {
            RestartButton.enabled = true;
        }

        RestartButton.onClick.AddListener(() => CustomConfirmationScreen("Do you really want to restart the game", ButtonMethodRestartGame, CloseConfirmationScreen));

        GetButtonByName(PauseMenuButtons, "LeaveGame").onClick.AddListener(() => CustomConfirmationScreen("Do you really want to leave the game", ButtonMethodLeaveGame, CloseConfirmationScreen));
        GetButtonByName(PauseMenuButtons, "ResumeGame").onClick.AddListener(TogglePauseMenu);
    }

    void Update()
    {
        if (Input.GetButton("Cancel") && menuCallable)
        {
            TogglePauseMenu();
            StartCoroutine(PauseMenuCooldown());
        }
    }

    public void SetupGameObjects()
    {
        MessageBox.SetActive(false);
        PlayerTurn.SetActive(true);
        PlayerTurn.AddComponent<MenuElement>();
        PauseMenu.SetActive(false);
        PauseMenu.AddComponent<MenuElement>();
        ConfirmationScreen.SetActive(false);
        ConfirmationScreen.AddComponent<MenuElement>();
    }

    private void UnloadSceneAndBackToMainMenu()
    {
        if (NetworkActivated)
        {
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
            NetworkManager.Singleton.Shutdown();
        }
        else
        {
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        }
    }

    private void SetScene(string sceneName, bool isActive)
    {
        foreach (GameObject obj in SceneManager.GetSceneByName(sceneName).GetRootGameObjects())
        {
            obj.SetActive(isActive);
        }
    }


    //Works
    #region Setup PlayerTurn

    private void UpdatePlayerTurn()
    {
        string statusStr; 
        if (NetworkActivated)
        {
            statusStr = Spielfeld.Instance.myStatus.ToString();
        }
        else
        {
            statusStr = Spielfeld_Hotseat.Instance.myStatus.ToString();
        }
        MenuElement PlayerTurnConfig = PlayerTurn.GetComponent<MenuElement>();
        PlayerTurnConfig.ConfigureTextElement("PlayerTurn_Text", statusStr);
    }



    #endregion

    //Works
    #region Opening PauseMenu
    private void TogglePauseMenu()
    {
        if (PauseMenu.activeSelf)
        {
            PauseMenu.SetActive(false);
            PlayerTurn.SetActive(true);
        }
        else
        {
            PauseMenu.SetActive(true);
            PlayerTurn.SetActive(false);
        }
    }

    IEnumerator PauseMenuCooldown()
    {
        // deactivate menu
        menuCallable = false;
        // Wait for cooldown duration
        yield return new WaitForSeconds(0.3f);
        // Reactivate menu
        menuCallable = true;
    }
    #endregion

    #region ConfirmationScreenConfig

    private void CustomConfirmationScreen(string ConfirmationScreenMessage, UnityAction Confirm, UnityAction Decline)
    {
        ConfirmationScreen.SetActive(true);
        ConfirmationScreen.GetComponent<SetConfirmationScreen>().SetComponent(ConfirmationScreenMessage, Confirm, Decline);
        PauseMenu.SetActive(false);
    }

    private void CloseConfirmationScreen()
    {
        ConfirmationScreen.SetActive(false);
        PauseMenu.SetActive(true);
    }
    #endregion

    #region MessageboxConfig

    UnityAction GameLeftMessagebox;

    private void MessageboxSetup()
    {
        GameLeftMessagebox += UnloadSceneAndBackToMainMenu;
    }

    private void CustomMessagebox(string Message, UnityAction acceptAction = null)
    {
        MessageBox.SetActive(true);
       
        MessageBox.GetComponent<SetMessageBox>().SetUpMessageBox(Message, acceptAction);
    }

    #endregion

    #region PauseMenuConfig

    private Button GetButtonByName(Button[] buttonList, string name)
    {
        foreach (Button button in buttonList)
        {
            if (button.name == name)
            {
                return button;
            }
        }
        throw new Exception($"In UI_Controller: cannot open Button Obj {name}");
    }

    private Button[] GetPauseMenuButtons()
    {
        return PauseMenu.GetComponentsInChildren<Button>();
    }
    #endregion

    #region MainMenu Button Methods
    private void ButtonMethodLeaveGame()
    {
        if (NetworkActivated)
        {
            //send server RPC that this client has left
            
        }

        UnloadSceneAndBackToMainMenu();
    }

    private void ButtonMethodRestartGame()
    {
        if (NetworkActivated)
        {
            Spielfeld.Instance.resetPlayfield();
            networkEvents.ResetUIClientRpc();
        }
        else
        {
            Spielfeld_Hotseat.Instance.resetPlayfield();
        }
        ConfirmationScreen.SetActive(false);
        // client RPC that resets the UI on the client
        
        // reset Ui on this side
        SetupGameObjects();
    }
    #endregion

    internal class NetworkEvents : NetworkBehaviour
    {
        UI_Controller ui_Controller;
        public NetworkEvents(UI_Controller outerClass)
        {
            this.ui_Controller = outerClass;
        }

        private void Start()
        {
            if (IsClient)
            {
                NetworkManager.Singleton.OnServerStopped += ServerStopped;
            }
            if (IsHost)
            {
                NetworkManager.Singleton.OnClientDisconnectCallback += ClientDisconnected;
            }
        }

        private void OnDisable()
        {
            if (IsClient)
            {
                NetworkManager.Singleton.OnServerStopped -= ServerStopped;
            }
            else if(IsHost)
            {
                NetworkManager.Singleton.OnClientDisconnectCallback -= ClientDisconnected;
            }
        }
    

        [ClientRpc]
        public void ResetUIClientRpc()
        {
            ui_Controller.SetupGameObjects();
        }

        [ClientRpc]
        public void OtherPlayerLeftClientRpc(ClientRpcParams rpcParams = default)
        {
            ui_Controller.OnOpponentLeft?.Invoke();
        }




        [ServerRpc(RequireOwnership = false)]
        public void LeaveMessageToOtherPlayerServerRpc(ulong specificClient, ServerRpcParams rpcParams = default)
        {
            
            var SenderCientId = rpcParams.Receive.SenderClientId;

        }

        private void ClientDisconnected(ulong obj)
        {
            ui_Controller.OpponentLeftMessagebox();
        }

        private void ServerStopped(bool obj)
        {
            ui_Controller.OpponentLeftMessagebox();
        }
    }


}

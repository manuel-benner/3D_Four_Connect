using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
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


    internal NetworkEvents networkEvents;

    internal bool menuCallable;


    // Start is called before the first frame update
    private void OnEnable()
    {
        if (PauseMenu == null) throw new Exception("PauseMenu is not given");
        if (PlayerTurn == null) throw new Exception("PlayerTurn is not given");
        if (ConfirmationScreen == null) throw new Exception("ConfirmationScreen is not given");
        if (MessageBox == null) throw new Exception("MessageBox is not given");

        if (NetworkActivated)
        {
            networkEvents = new NetworkEvents(this);
            Spielfeld.OnReset += SetupGameObjects;
            Spielfeld.OnWin += OnWinMessagebox;
            Spielfeld.OnDraw += OnDrawMessagebox;
        }
        else
        {
            Spielfeld_Hotseat.OnWin += OnWinMessagebox;
            Spielfeld_Hotseat.OnDraw += OnDrawMessagebox;
            networkEvents = null;
        }
        //setting the event OnNewTurn
        SetupPlayerTurn();
        // deactivating every GameObject, except PlayerTurn
        SetupGameObjects();
        // setting all Text elements and Button obClick methods
        PauseMenuSetup();
        // setting the messagebox onclick method to close the messagebox
        MessageboxSetup();
        menuCallable = true;
    }

    private void OnDrawMessagebox()
    {
        Debug.Log("Draw");
    }

    private void OnWinMessagebox()
    {
        Debug.Log($"Winner ...");
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
        PauseMenu.SetActive(false);
        ConfirmationScreen.SetActive(false);
    }

    private void SafeNetworkManagerShutdown()
    {
        if(NetworkActivated)
        {
            NetworkManager.Singleton.Shutdown();
        }
    }

    

    #region Setup PlayerTurn

    private void SetupPlayerTurn()
    {
        if (NetworkActivated)
        {
            Spielfeld.OnNewTurn += UpdatePlayerTurn;
        }
        else
        {
            Spielfeld_Hotseat.OnNewTurn += UpdatePlayerTurn;
        }
    }

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
        MenuElement PlayerTurnConfig = new MenuElement(PlayerTurn);
        PlayerTurnConfig.ConfigureTextElement("PlayerTurn_Text", statusStr);
    }



    #endregion

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

    UnityAction ReturnToMenu;
    UnityAction ConfirmedRestart;
    UnityAction ConfirmedLeave;

    private void SetupConfirmationScreeen()
    {
        ConfirmedRestart += ButtonMethodRestartGame;
        ConfirmedLeave += ButtonMethodLeaveGame;
        ReturnToMenu += BackToPauseMenu;
    }

    public void RestartConfirmation()
    {
        ConfirmationScreen.SetActive(true);
        ConfirmationScreen.GetComponent<SetConfirmationScreen>().SetComponent("Do you really want to restart?", ConfirmedRestart, ReturnToMenu);
        PauseMenu.SetActive(false);
    }

    public void LeaveConfirmation()
    {
        ConfirmationScreen.SetActive(true);
        ConfirmationScreen.GetComponent<SetConfirmationScreen>().SetComponent("Do you really want to leave?", ConfirmedLeave, ReturnToMenu);
        PauseMenu.SetActive(false);
    }

    private void BackToPauseMenu()
    {
        ConfirmationScreen.SetActive(false);
        PauseMenu.SetActive(true);
    }
    #endregion

    #region MessageboxConfig

    UnityAction GameLeftMessagebox;

    private void MessageboxSetup()
    {
        GameLeftMessagebox += MessageboxBackToMainMenu;
    }
    
    private void OpponentLeftMessagebox()
    {
        MessageBox.SetActive(true);
        UnityAction playerleftMethod = ButtonMethodLeaveGame;
        MessageBox.GetComponent<SetMessageBox>().SetUpMessageBox("Other player left", GameLeftMessagebox);
    }

    private void MessageboxBackToMainMenu()
    {
        SafeNetworkManagerShutdown();
        SceneManager.LoadScene("MainMenu");
    }

    #endregion

    #region PauseMenuConfig
    private void PauseMenuSetup()
    {
        Button[] PauseMenuButtons = GetPauseMenuButtons();
        //setting the action, that is called in the onclick method 
        SetupConfirmationScreeen();

        SetRestartButton(GetButtonByName(PauseMenuButtons, "RestartGame"));
        GetButtonByName(PauseMenuButtons, "LeaveGame").onClick.AddListener(LeaveConfirmation);
        GetButtonByName(PauseMenuButtons, "ResumeGame").onClick.AddListener(TogglePauseMenu);
    }

    private void SetRestartButton(Button RestartButton)
    {
        RestartButton.onClick.AddListener(RestartConfirmation);
        if (NetworkActivated)
        {
            if (NetworkManager.Singleton == null)
            {
                throw new Exception("No Network Manager given");
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
    }

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
        SafeNetworkManagerShutdown();

        if (NetworkActivated)
        {
            if (NetworkManager.Singleton.IsHost)
            {

            }
            if (NetworkManager.Singleton.IsClient)
            {

            }
        }

        SceneManager.LoadScene("MainMenu");
    }

    private void ButtonMethodRestartGame()
    {
        if (NetworkActivated)
        {
            Spielfeld.Instance.resetPlayfield();
        }
        else
        {
            Spielfeld_Hotseat.Instance.resetPlayfield();
        }
        ConfirmationScreen.SetActive(false);
        // client RPC that resets the UI on the client
        networkEvents.ResetUIClientRpc();
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

        [ClientRpc]
        public void ResetUIClientRpc()
        {
            ui_Controller.SetupGameObjects();
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

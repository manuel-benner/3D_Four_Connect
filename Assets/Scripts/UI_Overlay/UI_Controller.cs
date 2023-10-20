using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.UI.Button;

public class UI_Controller : MonoBehaviour
{
    [SerializeField] GameObject PauseMenu;
    [SerializeField] GameObject IngameOverlay;
    [SerializeField] GameObject ConfirmationScreen;
    [SerializeField] GameObject MessageBox;

    internal NetworkEvents networkEvents;

    internal bool menuCallable;


    // Start is called before the first frame update
    void Start()
    {
        if (PauseMenu == null) throw new Exception("PauseMenu is not given");
        if (IngameOverlay == null) throw new Exception("IngameOverlay is not given");
        if (ConfirmationScreen == null) throw new Exception("ConfirmationScreen is not given");
        if (MessageBox == null) throw new Exception("MessageBox is not given");

        Setup();


        networkEvents = new NetworkEvents(this);

        MessageboxSetup();
        SetPauseMenuButtonFunctions();




        menuCallable = true;
    }


    public void Setup()
    {
        MessageBox.SetActive(false);
        IngameOverlay.SetActive(true);
        PauseMenu.SetActive(false);
        ConfirmationScreen.SetActive(false);
    }


    #region Opening PauseMenu

    void Update()
    {
        if (Input.GetButton("Cancel") && menuCallable)
        {
            TogglePauseMenu();
            StartCoroutine(PauseMenuCooldown());
        }
    }

    private void TogglePauseMenu()
    {
        if (PauseMenu.activeSelf)
        {
            PauseMenu.SetActive(false);
            IngameOverlay.SetActive(true);
        }
        else
        {
            PauseMenu.SetActive(true);
            IngameOverlay.SetActive(false);
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

    #region ConfirmationConfig

    UnityAction ReturnToMenu;
    UnityAction ConfirmedRestart;
    UnityAction ConfirmedLeave;

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
        GameLeftMessagebox += GoToMainMenu;
    }
    
    private void OtherPlayerLeftMessagebox()
    {
        MessageBox.SetActive(true);
        MessageBox.GetComponent<SetMessageBox>().SetUpMessageBox("Other player left", GameLeftMessagebox);
    }

    private void GoToMainMenu()
    {
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene("MainMenu");
    }

    #endregion

    #region PauseMenuButtons
    private void SetPauseMenuButtonFunctions()
    {
        Button[] PauseMenuButtons = GetPauseMenuButtons();
        //setting the action, that is called in the onclick method 
        ConfirmedRestart += ButtonMethodRestartGame;
        ConfirmedLeave += ButtonMethodLeaveGame;
        ReturnToMenu += BackToPauseMenu;

        SetRestartButton(GetButtonByName(PauseMenuButtons, "RestartGame"));
        GetButtonByName(PauseMenuButtons, "LeaveGame").onClick.AddListener(LeaveConfirmation);
        GetButtonByName(PauseMenuButtons, "ResumeGame").onClick.AddListener(TogglePauseMenu);
    }

    private void SetRestartButton(Button RestartButton)
    {
        
        RestartButton.onClick.AddListener(RestartConfirmation);
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

    private void ButtonMethodLeaveGame()
    {
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene("MainMenu");
    }

    private void ButtonMethodRestartGame()
    {
        //Spielfeld.Instance.resetPlayfield();
        Spielfeld.Instance.myStatus = Spielfeld.Status.newGame;
        ConfirmationScreen.SetActive(false);
        // client RPC that resets the UI on the client
        networkEvents.ResetUIClientRpc();
        // reset Ui on this side
        Setup();
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
            ui_Controller.Setup();
        }

        private void ClientDisconnected(ulong obj)
        {
            ui_Controller.OtherPlayerLeftMessagebox();
        }

        private void ServerStopped(bool obj)
        {
            ui_Controller.OtherPlayerLeftMessagebox();
        }
    }
}

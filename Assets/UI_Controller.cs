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
    //[SerializeField] GameObject Spielfeld;

    bool menuCallable;


    // Start is called before the first frame update
    void Start()
    {
        if (PauseMenu == null) throw new Exception("PauseMenu is not given");
        if (IngameOverlay == null) throw new Exception("IngameOverlay is not given");
        if (ConfirmationScreen == null) throw new Exception("ConfirmationScreen is not given");


        SetPauseMenuButtonFunctions();

        IngameOverlay.SetActive(true);
        PauseMenu.SetActive(false);

        menuCallable = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Cancel") && menuCallable)
        {
            TogglePauseMenu();
            StartCoroutine(Cooldown());
        }
    }

    #region RestartConfiguration

    UnityAction AllNo;
    UnityAction restartYes;
    UnityAction LeaveYes;

    public void RestartConfirmation()
    {
        ConfirmationScreen.SetActive(true);
        ConfirmationScreen.GetComponent<SetConfirmationScreen>().SetComponent("Do you really want to restart?", restartYes, AllNo);
        PauseMenu.SetActive(false);
    }
    
    public void LeaveConfirmation()
    {
        ConfirmationScreen.SetActive(true);
        ConfirmationScreen.GetComponent<SetConfirmationScreen>().SetComponent("Do you really want to leave?", LeaveYes, AllNo);
        PauseMenu.SetActive(false);
    }

    private void BackToPauseMenu()
    {
        ConfirmationScreen.SetActive(false);
        PauseMenu.SetActive(true);
    }


    #endregion

    #region PauseMenu

    IEnumerator Cooldown()
    {
        // Deactivate myButton
        menuCallable = false;
        // Wait for cooldown duration
        yield return new WaitForSeconds(0.2f);
        // Reactivate myButton
        menuCallable = true;
    }

    private void SetPauseMenuButtonFunctions()
    {
        Button[] PauseMenuButtons = getPauseMenuButtons();
        //setting the action, that is called in the onclick method 
        restartYes += restartGame;
        LeaveYes += leaveGame;
        AllNo += BackToPauseMenu;

        SetRestartButton(getButtonByName(PauseMenuButtons, "RestartGame"));
        SetLeaveButton(getButtonByName(PauseMenuButtons, "LeaveGame"));
        SetResumeButton(getButtonByName(PauseMenuButtons, "ResumeGame"));
    }

    private void SetResumeButton(Button ResumeButton)
    {
        ResumeButton.onClick.AddListener(TogglePauseMenu);
    }

    private void SetLeaveButton(Button LeaveButton)
    {
        
        LeaveButton.onClick.AddListener(LeaveConfirmation);
    }

    private void SetRestartButton(Button RestartButton)
    {
        
        RestartButton.onClick.AddListener(RestartConfirmation);
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

    private void leaveGame()
    {
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene("MainMenu");
    }

    private void restartGame()
    {
        Spielfeld.Instance.resetPlayfield();
    }


    private Button getButtonByName(Button[] buttonList, string name)
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

    private Button[] getPauseMenuButtons()
    {
        return PauseMenu.GetComponentsInChildren<Button>();
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
    #endregion








    private class NetworkEvents : NetworkBehaviour
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

        private void ClientDisconnected(ulong obj)
        {
            throw new NotImplementedException();
        }

        private void ServerStopped(bool obj)
        {
            throw new NotImplementedException();
        }
    }
}

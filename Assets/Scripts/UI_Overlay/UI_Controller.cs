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

public class UI_Controller: MonoBehaviour
{
    [SerializeField] GameObject PauseMenu;
    [SerializeField] GameObject PlayerTurn;
    [SerializeField] GameObject ConfirmationScreen;
    [SerializeField] GameObject MessageBox;
    [SerializeField] bool NetworkActivated;
    GameObject[] KugelAuswahl;

    internal bool menuCallable;

    private void OnEnable()
    {
        KugelAuswahl = GameObject.FindGameObjectsWithTag("KugelAuswahl");
    }

    private void OnDisable()
    {
        // make sure that the interactable spheres are active 
        //ToggleInteraction(true);
        KugelAuswahl = null;

        if (NetworkActivated)
        {
            Spielfeld.OnReset -= OnResetAction;
            Spielfeld.OnWin -= OnWinActionNetwork;
            Spielfeld.OnDraw -= OnDrawAction;
            Spielfeld.OnNewTurn -= UpdatePlayerTurn;
        }
        else
        {
            Spielfeld_Hotseat.OnWin -= OnWinActionHotseat;
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

        KugelAuswahl = GameObject.FindGameObjectsWithTag("KugelAuswahl");

        // adding the MenuElement class to game objets
        SetupGameObjects();

        // deactivating every GameObject, except PlayerTurn
        GameObjectsToStartState();

        // setting all Text elements and Button obClick methods
        PauseMenuSetup();

        if (NetworkActivated)
        {
            Spielfeld.OnReset += OnResetAction;
            Spielfeld.OnWin += OnWinActionNetwork;
            Spielfeld.OnDraw += OnDrawAction;
            Spielfeld.OnNewTurn += UpdatePlayerTurn;
        }
        else
        {
            Spielfeld_Hotseat.OnWin += OnWinActionHotseat;
            Spielfeld_Hotseat.OnDraw += OnDrawAction;
            Spielfeld_Hotseat.OnNewTurn += UpdatePlayerTurn;

        }
        menuCallable = true;
        StartCoroutine(SetPlayerTurn());

    }
    #region Game Events

    IEnumerator SetPlayerTurn()
    {
        yield return new WaitForSeconds(Time.deltaTime);
        UpdatePlayerTurn();
    }

    private void OnResetAction()
    {
        if (! NetworkManager.Singleton.IsServer)
        {
            CustomMessagebox("The game has been reset", GameObjectsToStartState);
        }
    }

    private void OnDrawAction()
    {
        if (NetworkActivated)
        {
            if (NetworkManager.Singleton.IsHost)
            {
                CustomConfirmationScreen("The game ended in a draw, do you want to restart the game ?", ButtonMethodRestartGame, ButtonMethodLeaveGame);
            }
            else
            {
                CustomConfirmationScreen("The game ended in a draw, do you want to wait for the host to restart the game ?", GameObjectsToStartState, ButtonMethodLeaveGame);
            }
        }
    }

    public void OnOpponentLeft()
    {
        MessageBox.SetActive(true);
        Debug.Log("Opponent left");
        CustomMessagebox("Other player left", UnloadSceneAndBackToMainMenu);
    }

    private void OnWinActionHotseat(Spielfeld_Hotseat.status Winner)
    {
        string Message;
        if (Winner == Spielfeld_Hotseat.status.Player1)
        {
            Message = "Player 1 won";
        }
        else
        {
            Message = "Player 2 won";
        }
        Message += " do you want to play again ?";
        CustomConfirmationScreen(Message, ButtonMethodRestartGame, ButtonMethodLeaveGame);

    }

    private void OnWinActionNetwork(Spielfeld.Status Winner )
    {
        string Message; 
        if(Winner == Spielfeld.Status.myTurn)
        {
            Message = "You won";
        }
        else if(Winner == Spielfeld.Status.opponentTurn)
        {
            Message = "You lost";
        }
        else
        {
            throw new Exception("Error in Spielfeld.OnWin -> wrong Status returned ");
        }

        if (NetworkManager.Singleton.IsServer)
        {
            Message += " :do you want to play again?";
            CustomConfirmationScreen(Message, ButtonMethodRestartGame, ButtonMethodLeaveGame);
        }
        else
        {
            Message += " :do you want to wait for the host to restart the game ?";
            CustomConfirmationScreen(Message, GameObjectsToStartState, ButtonMethodLeaveGame);
        }
    }

    #endregion

    private void PauseMenuSetup()
    {
        Button[] PauseMenuButtons = GetPauseMenuButtons();
        //setting the action, that is called in the onclick method 

        Button RestartButton = GetButtonByName(PauseMenuButtons, "RestartGame");


        MenuElement PauseMenuSetup = PauseMenu.GetComponent<MenuElement>();
        bool active;
        if (NetworkActivated)
        {            
            if (NetworkManager.Singleton == null)
            {
                throw new Exception("No Network Manager given");
            }
            else if (NetworkManager.Singleton.IsServer)
            {
                active = true;
            }
            else
            {
                active = false;
            }
        }
        else
        {
            active = true;
        }
        PauseMenuSetup.SetButton("RestartGame", active);
        PauseMenuSetup.ConfigureButton("RestartGame", () => CustomConfirmationScreen("Do you really want to restart the game", ButtonMethodRestartGame, CloseConfirmationScreen));
        PauseMenuSetup.ConfigureButton("LeaveGame", () => CustomConfirmationScreen("Do you really want to leave the game", ButtonMethodLeaveGame, CloseConfirmationScreen));
        PauseMenuSetup.ConfigureButton("ResumeGame", TogglePauseMenu);
    }

    void Update()
    {
        if (Input.GetButton("Cancel") && menuCallable)
        {
            TogglePauseMenu();
            StartCoroutine(PauseMenuCooldown());
        }
    }

    private void SetupGameObjects()
    {
        if (NetworkActivated)
        {
            gameObject.AddComponent<NetworkEvents>();
        }
        ToggleInteraction(true);
        PlayerTurn.AddComponent<MenuElement>();
        PauseMenu.AddComponent<MenuElement>();
        ConfirmationScreen.AddComponent<MenuElement>();
        MessageBox.AddComponent<MenuElement>();

    }

    public void GameObjectsToStartState()
    {
        Debug.Log("Reset GameObjects to start state");
        MessageBox.SetActive(false);
        PlayerTurn.SetActive(true);
        PauseMenu.SetActive(false);
        ConfirmationScreen.SetActive(false);
    }

    private void UnloadSceneAndBackToMainMenu()
    {
        if (NetworkActivated)
        {
            NetworkManager.Singleton.Shutdown();
            // doing this before changing scene, because in Main Menu will be another Object with NetworkManager
            DestroyNetworkManagerObj();
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
            
        }
        else
        {
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
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
            ToggleInteraction(true);
            PauseMenu.SetActive(false);
            PlayerTurn.SetActive(true);
        }
        else
        {
            ToggleInteraction(false);
            PauseMenu.SetActive(true);
            PlayerTurn.SetActive(false);
        }
    }

    private void ToggleInteraction(bool SetAs)
    {
        foreach(GameObject interactableObj in KugelAuswahl)
        {
            if (NetworkActivated){
                interactableObj.GetComponent<KugelAuswahlFarbeAendern>().active = SetAs;
            }
            else
            {
                interactableObj.GetComponent<KugelAuswahlFarbeAendernHotseat>().active = SetAs;
            }
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

        MenuElement ConfirmationScreenEle = ConfirmationScreen.GetComponent<MenuElement>();
        ConfirmationScreenEle.ConfigureTextElement("Question-Text",ConfirmationScreenMessage);
        ConfirmationScreenEle.ConfigureButton("Agree", Confirm);
        ConfirmationScreenEle.ConfigureButton("Disagree", Decline);

        ConfirmationScreen.SetActive(true);
        PauseMenu.SetActive(false);
    }

    private void CloseConfirmationScreen()
    {
        ConfirmationScreen.SetActive(false);
        PauseMenu.SetActive(true);
    }
    #endregion

    #region MessageboxConfig

    private void CustomMessagebox(string Message, UnityAction acceptAction)
    {
        MessageBox.SetActive(true);
       
        MenuElement MessageBoxElement = MessageBox.GetComponent<MenuElement>();

        MessageBoxElement.ConfigureTextElement("Message_Text", Message);

        MessageBoxElement.ConfigureButton("Accept_Btn", acceptAction);
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
            if (NetworkManager.Singleton.IsServer)
            {
                gameObject.GetComponent<NetworkEvents>().OtherPlayerLeftClientRpc();
            }
            else
            {
                gameObject.GetComponent<NetworkEvents>().LeaveMessageToOtherPlayerServerRpc();
            }
            var rpcParams = new ServerRpcParams();
            rpcParams.Receive.SenderClientId = NetworkManager.Singleton.LocalClientId;
        }
        UnloadSceneAndBackToMainMenu();
    }

    private void DestroyNetworkManagerObj()
    {
        GameObject NetworkManagerObj = GameObject.FindGameObjectWithTag("NetworkManager");
        if (NetworkManagerObj != null) Destroy(NetworkManagerObj);
        else throw new Exception("There is no Game object with the ta NetworkManager -> maybe the Tag has benn reset");
    }

    private void ButtonMethodRestartGame()
    {
        // reset Ui on this side
        GameObjectsToStartState();
        if (NetworkActivated)
        {
            Spielfeld.Instance.resetPlayfield();
            ToggleInteraction(true);
            gameObject.GetComponent<NetworkEvents>().ResetUIClientRpc();
        }
        else
        {
            ToggleInteraction(true);
            Spielfeld_Hotseat.Instance.resetPlayfield();
        }        
    }
    #endregion


}

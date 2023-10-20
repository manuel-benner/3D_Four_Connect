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

public class MainMenuController : MonoBehaviour
{
    [SerializeField] GameObject root;
    [SerializeField] GameObject NewGameSelection;
    [SerializeField] GameObject NetworkGameSelection;
    [SerializeField] GameObject HostWaitScreen;
    [SerializeField] GameObject JoinScreen;
    [SerializeField] GameObject ClientWaitScreen;
    [SerializeField] GameObject ErrorBox;


    // Start is called before the first frame update
    void Start()
    {
        // Deactivating all MainMenu objects except for root which is the object to be interacted with first
        root.SetActive(true);
        NewGameSelection.SetActive(false);
        NetworkGameSelection.SetActive(false);
        HostWaitScreen.SetActive(false);
        JoinScreen.SetActive(false);
        ClientWaitScreen.SetActive(false);
        ErrorBox.SetActive(false);
        
        // Setting up all button logic 
        SetupMainMenu();
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
        MenuElement rootSetup = new MenuElement(root);

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
        MenuElement NewGameSelectionSetup = new MenuElement(NewGameSelection);

        UnityAction StartLocal = StartHotseat;
        NewGameSelectionSetup.ConfigureButton("LocalGame", StartLocal);

        NewGameSelectionSetup.ChangeToGameObject("NetworkGame", NetworkGameSelection);

        NewGameSelectionSetup.ChangeToGameObject("Back", root);

    }
    public void StartHotseat()
    {
        SceneManager.LoadScene("Spielfeld_Hotseat", LoadSceneMode.Single);
        Debug.Log("Starting hotseat game");
    }

    #endregion

    #region NetworkGameSelectionSetup

    private void SetupNetworkGameSelection()
    {
        MenuElement NetworkGameSelectionSetup = new MenuElement(NetworkGameSelection);

        NetworkGameSelectionSetup.ChangeToGameObject("CreateGame", HostWaitScreen);

        NetworkGameSelectionSetup.ChangeToGameObject("JoinGame", JoinScreen);

        NetworkGameSelectionSetup.ChangeToGameObject("Back", NewGameSelection);

    }

    #endregion

    #region JoinScreenSetup
    
    private void SetupJoinScreen()
    {
        
        MenuElement JoinScreenSetup = new MenuElement(JoinScreen);

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
        MenuElement ErrorBoxSetup = new MenuElement(ErrorBox);

        ErrorBoxSetup.ConfigureTextElement("ErrorMessage",ErrorMessage);

        ErrorBoxSetup.ConfigureButton("AcceptBtn", ButtonAction);

    }


    #endregion

    #region SetupHostWait

    private void SetupHostWait()
    {
        MenuElement HostWaitSetup = new MenuElement(HostWaitScreen);
        UnityAction CancelHosting = HostWaitScreen.GetComponent<HostWait>().Exit;
        HostWaitSetup.ConfigureButton("Back", CancelHosting);

    }

    #endregion

    /// <summary>
    /// Class that is used to setup a Menu Gameobject, it acesses every button child element of this game Object and can set the onclick method 
    /// </summary>
    internal class MenuElement
    {
        GameObject MenuObject;

        UnityAction<GameObject> ChangeAction;

        public MenuElement(GameObject menuObject)
        {
            MenuObject = menuObject;
        } 

        public void ConfigureButton(string ButtonName, UnityAction ButtonOnclick)
        {
            // get the button with the given name ifit does not exist throw an error
            Button btn = GetButtonByName(ButtonName);

            btn.onClick.AddListener(ButtonOnclick);
        }

        public void ChangeToGameObject(string ButtonName, GameObject otherGameObj)
        {
            Button btn = GetButtonByName(ButtonName);
            ChangeAction = ChangeTo;
            btn.onClick.AddListener(() => ChangeAction(otherGameObj));
        }

        private void ChangeTo(GameObject changeTo)
        {
            MenuObject.SetActive(false);
            changeTo.SetActive(true);
        }

        public void ConfigureTextElement(string TextElementName, string TextToShow)
        {
            TMP_Text text = GetTextByName(TextElementName);
            if (text == null) throw new Exception($"TMP_Text {TextElementName} does not Exist in {MenuObject.name}");

            text.text = TextToShow;
        }

        private Button GetButtonByName(string name)
        {
            foreach (Button button in MenuObject.GetComponentsInChildren<Button>())
            {
                if (button.name == name) return button;
            }
            throw new Exception($"Button {name} does not Exist in {MenuObject.name}");
        }

        private TMP_Text GetTextByName(string name)
        {
            foreach(TMP_Text text in MenuObject.GetComponentsInChildren<TMP_Text>())
            {
                if (text.name == name) return text;
            }
            throw new Exception($"TMP_Text {name} does not Exist in {MenuObject.name}");
        }
    }
}

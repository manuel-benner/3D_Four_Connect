using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] GameObject root;
    [SerializeField] GameObject NewGameSelection;
    [SerializeField] GameObject NetworkGameSelection;
    [SerializeField] GameObject HostWaitScreen;
    [SerializeField] GameObject JoinScreen;
    [SerializeField] GameObject ClientWaitScreen;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    #region RootSetup


    private void SetupRoot()
    {

    }

    #endregion

    #region NewGameSetup

    private void SetupNewGame()
    {

    }

    #endregion

    #region NetworkGameSetup

    private void SetupNetworkGame()
    {

    }

    #endregion

    #region WaitScreenSetup

    #endregion

    #region JoinScreenSetup

    #endregion









    #region ButtonHelper

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

    private Button[] GetButtons(GameObject objToGetButtons)
    {
        return objToGetButtons.GetComponentsInChildren<Button>();
    }
    #endregion



}

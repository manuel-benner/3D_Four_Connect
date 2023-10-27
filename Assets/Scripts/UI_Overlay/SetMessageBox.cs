using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SetMessageBox : MonoBehaviour
{
    [SerializeField] GameObject OkButton;
    [SerializeField] GameObject Text;

    Button Ok;
    TMP_Text TextM;

    UnityAction closeElement;
    private void OnEnable()
    {
        Ok = GetComponentInChildrenByName<Button>("Accept_Button");
        TextM = GetComponentInChildrenByName<TMP_Text>("Message_Text");
        closeElement += closeEle;
    }

    private void closeEle()
    {
        gameObject.SetActive(false);
    }


    private T GetComponentInChildrenByName<T>(string Name)
        where T : Component
    {
        T[] AllComponentsOfThisType = GetComponentsInChildren<T>();
        
        foreach(T t in AllComponentsOfThisType)
        {
            if(t.name == Name)
            {
                return t as T;
            }
        }
        return null;
    }

    /// <summary>
    /// Method that sets the functionality of the Accept button 
    /// </summary>
    /// <param name="Message"> Message thet is shown to the player</param>
    /// <param name="OkAction"> Unity action that is called if the button is clicked, if nothing is set, the ok button closes the Messagebox</param>
    public void SetUpMessageBox(string Message, UnityAction OkAction = null)
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
        TextM.text = Message;

        if (OkAction != null)
        {
            Ok.onClick.AddListener(OkAction);
        }

        Ok.onClick.AddListener(closeElement);

    }
}

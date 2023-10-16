using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SetConfirmationScreen : MonoBehaviour
{
    UnityAction Yes;
    UnityAction No;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void SetComponent(string Question, UnityAction YesDelegate, UnityAction NoDelegate)
    {
        GetTMP_TextByName(GetTMP(), "Question-Text").text = Question;
        Yes = YesDelegate;
        No = NoDelegate;
        SetBtns();
    }

    


    private void SetBtns()
    {
        Button[] buttons = GetButtons();
        Button YesBtn = GetButtonByName(buttons, "Agree");
        Button NoBtn = GetButtonByName(buttons, "Disagree");

        YesBtn.onClick.AddListener(this.Yes);
        NoBtn.onClick.AddListener(this.No);
    }

    private TMP_Text GetTMP_TextByName(TMP_Text[] arr, string name)
    {
        foreach(TMP_Text text in arr)
        {
            if (text.name == name)
            {
                return text;
            }
        }
        throw new Exception($"SetConfirmationScreen: There is no TMP named: {name}");
    }

    private Button GetButtonByName(Button[] buttons, string name)
    {
        foreach(Button btn in buttons)
        {
            if(btn.name == name)
            {
                return btn;
            }
        }
        throw new Exception($"SetConfirmationScreen: There is no button named: {name}");
    }

    private TMP_Text[] GetTMP()
    {
        return GetComponentsInChildren<TMP_Text>();
    }


    private Button[] GetButtons()
    {
        return GetComponentsInChildren<Button>();
    }


}

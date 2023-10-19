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
    private void Start()
    {
        Ok = OkButton.GetComponent<Button>();
        TextM = Text.GetComponent<TMP_Text>();
        closeElement += closeEle;
    }

    private void closeEle()
    {
        gameObject.SetActive(false);
    }

    public void SetUpMessageBox(string Message, UnityAction OkAction = null)
    {
        TextM.text = Message;
        if (OkAction != null)
        {
            Ok.onClick.AddListener(OkAction);
        }
        else
        {
            Ok.onClick.AddListener(closeElement);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Turn_UI : MonoBehaviour
{
    Spielfeld.Status? lastStatus;


    // Start is called before the first frame update
    void Start()
    {
        lastStatus = null;
        GetComponent<TextMeshProUGUI>().text = "test";
    }

    // Update is called once per frame
    void Update()
    {
        
        if (lastStatus != Spielfeld.Instance.myStatus || lastStatus == null)
        {
            GetComponent<TextMeshProUGUI>().text = Spielfeld.Instance.myStatus.ToString();
        }
        lastStatus = Spielfeld.Instance.myStatus;
    }
}

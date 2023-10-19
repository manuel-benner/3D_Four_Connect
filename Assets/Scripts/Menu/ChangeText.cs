using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChangeText : MonoBehaviour
{

    public void SetText(string text)
    {
        TMP_Text textMesh = GetTextMeshObj();

        textMesh.text = text;
    }

    private TMP_Text GetTextMeshObj()
    {
        GameObject TextMeshGameobj = GetChildGameObjectByName("TextMeshObj", GetChildGameObjectByName("Text"));

        return TextMeshGameobj.GetComponent<TMP_Text>();

    }

    private GameObject GetChildGameObjectByName(string ObjName, GameObject overrideObj = null)
    {
        GameObject objToSearchIn;
        if (overrideObj != null) objToSearchIn = overrideObj;
        else objToSearchIn = this.gameObject;

        foreach(Transform t in objToSearchIn.transform)
        {
            if (t.gameObject.name == ObjName)
            {
                return t.gameObject;
            }
        }
        return null;
    }
}

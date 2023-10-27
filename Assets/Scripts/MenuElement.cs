using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.Events;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    /// <summary>
    /// Class that is used to setup a Menu Gameobject, it acesses every button child element of this game Object and can set the onclick method 
    /// </summary>
    public class MenuElement : MonoBehaviour
    {
        UnityAction<GameObject> ChangeAction;


        private void Start()
        {
            
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
            gameObject.SetActive(false);
            changeTo.SetActive(true);
        }

        public void ConfigureTextElement(string TextElementName, string TextToShow)
        {
            TMP_Text text = GetTextByName(TextElementName);
            if (text == null) throw new Exception($"TMP_Text {TextElementName} does not Exist in {gameObject.name}");

            text.text = TextToShow;
        }

        private Button GetButtonByName(string name)
        {
            foreach (Button button in gameObject.GetComponentsInChildren<Button>())
            {
                if (button.name == name) return button;
            }
            throw new Exception($"Button {name} does not Exist in {gameObject.name}");
        }

        private TMP_Text GetTextByName(string name)
        {
            foreach (TMP_Text text in gameObject.GetComponentsInChildren<TMP_Text>())
            {
                if (text.name == name) return text;
            }
            return null;
        }
    }
}

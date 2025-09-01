using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

namespace StudioNAP
{
    public class pnlDialog : PopupBase
    {
        UnityAction _callback;
        void Start()
        {
            Init();
        }

        public void ShowDialog(string title, string content, UnityAction callback)
        {
            Init(); // Initialize the popup

            _callback = callback;
            _content.Find("lblTitle").GetComponent<TextMeshProUGUI>().text = title;
            _content.Find("lblContent").GetComponent<TextMeshProUGUI>().text = content;
        }

        public void OnClickOK()
        {
            _callback?.Invoke();
            ClosePopup();
        }

    }
}

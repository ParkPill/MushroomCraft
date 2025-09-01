using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace StudioNAP
{
    public class TabButton : MonoBehaviour
    {
        public Color EnableTextColor = Color.white;
        public Color DisableTextColor = Color.gray;
        public Sprite EnableSprite;
        public Sprite DisableSprite;
        public Sprite EnableSpriteIcon;
        public Sprite DisableSpriteIcon;
        public Color EnableBtnColor = Color.white;
        public Color DisableBtnColor = Color.white;
        public bool DisableImageWhenDisabled;
        public bool IsStartTab = false;
        Text _lbl;
        TextMeshProUGUI _lblTMPUGUI;
        bool _isInit;
        public bool AutoSelect = true;
        public Transform Tab;
        private void Start()
        {
            Button btn = GetComponent<Button>();
            if (AutoSelect) btn.onClick.AddListener(OnClicked);

            if (!_isInit) SetTabEnabled(IsStartTab);
            //if (IsStartTab)
            //{
            //    btn.onClick?.Invoke();
            //}
        }
        void Init()
        {
            if (_isInit) return;
            Transform trLbl = transform.Find("Text");
            if (trLbl)
            {
                _lbl = trLbl.GetComponent<Text>();
                if (_lbl == null) _lblTMPUGUI = trLbl.GetComponent<TextMeshProUGUI>();
            }
            _isInit = true;
        }

        public void SetTabEnabled(bool enabled)
        {
            Init();
            if (_lbl) _lbl.color = enabled ? EnableTextColor : DisableTextColor;
            if (_lblTMPUGUI) _lblTMPUGUI.color = enabled ? EnableTextColor : DisableTextColor;
            Image img = GetComponent<Image>();
            if (EnableSprite)
            {
                img.sprite = enabled ? EnableSprite : DisableSprite;
            }

            img.color = enabled ? EnableBtnColor : DisableBtnColor;
            if (EnableSpriteIcon)
            {
                img = transform.Find("Image").GetComponent<Image>();
                if (img)
                {
                    img.sprite = enabled ? EnableSpriteIcon : DisableSpriteIcon;
                }
            }

            if (DisableImageWhenDisabled) img.enabled = enabled;
            if (Tab) Tab.gameObject.SetActive(enabled);
        }

        public void OnClicked()
        {
            //print("clicked!");
            CheckEnabled();
        }
        void CheckEnabled()
        {
            foreach (Transform child in transform.parent)
            {
                TabButton tab = child.GetComponent<TabButton>();
                if (tab)
                {
                    tab.SetTabEnabled(tab == this);
                }
            }
        }
    }
}
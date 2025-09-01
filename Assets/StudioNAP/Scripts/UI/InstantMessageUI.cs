using System.Collections;
using System.Collections.Generic;
//using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StudioNAP
{

    public class InstantMessageUI : PopupBase
    {
        public Text Text;
        //public float TargetY = 34;
        //public float HideY = -34;
        public float PositionY = 150;
        [HideInInspector]
        public float ShowY = 158;
        [HideInInspector]
        public float InitY = -150;
        public float FloatingTime = 1.5f;
        private float _floatTimeChecker = 0;
        public float MoveSpeed = 5;
        private float scale = 1;
        [HideInInspector]
        public float StartY = 0;
        bool _isStartSet = false;
        private void Awake()
        {
            transform.localScale = new Vector3(scale, scale, scale);
        }
        // Start is called before the first frame update
        void Start()
        {
            ShowY = transform.localPosition.y;
            transform.localPosition = new Vector3(transform.localPosition.x, ShowY + InitY, transform.localPosition.z);
            StartY = transform.localPosition.y;
            _isStartSet = true;
        }

        // Update is called once per frame
        void Update()
        {
            if (_floatTimeChecker > 0)
            {
                float gap = ShowY - StartY;
                Vector3 targetPos = new Vector3(transform.localPosition.x, ShowY, transform.localPosition.z);
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetPos, MoveSpeed);
                Image img = GetComponent<Image>();
                Color color = img.color;
                float alpha = 1 - Mathf.Abs(ShowY - transform.localPosition.y) / gap;
                //print("emerge alpha: " + alpha);
                img.color = new Color(color.r, color.g, color.b, alpha * 0.5f);
                Text lbl = transform.Find("Text").GetComponent<Text>();
                color = lbl.color;
                lbl.color = new Color(color.r, color.g, color.b, alpha);
                if ((transform.localPosition - targetPos).sqrMagnitude < 1)
                {
                    _floatTimeChecker -= Time.deltaTime;
                    if (_floatTimeChecker <= 0)
                    {

                    }
                }
            }
            else
            {
                float gap = Mathf.Abs(InitY * 1.5f);
                float targetY = ShowY - InitY * 1.5f;
                Vector3 targetPos = new Vector3(transform.localPosition.x, targetY, transform.localPosition.z);
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetPos, MoveSpeed);
                Image img = GetComponent<Image>();
                Color color = img.color;
                float alpha = Mathf.Abs(transform.localPosition.y - targetY) / gap;
                img.color = new Color(color.r, color.g, color.b, alpha * 0.5f);
                Text lbl = transform.Find("Text").GetComponent<Text>();
                color = lbl.color;
                lbl.color = new Color(color.r, color.g, color.b, alpha);
                //print($"disappear alpha: {alpha} / {transform.localPosition.y}/{targetY}/{gap}" );
                if ((transform.localPosition - targetPos).sqrMagnitude < 1)
                {
                    //gameObject.SetActive(false);
                    Destroy(gameObject);
                }
            }
        }
        public void ShowMessage(string msg)
        {
            if (!_isStartSet)
            {
                transform.localPosition = new Vector3(transform.localPosition.x, PositionY, transform.localPosition.z);
                ShowY = transform.localPosition.y;
                transform.localScale = new Vector3(scale * 1.2f, scale * 1.2f, scale * 1.2f);
            }
            if (Text == null)
            {
                Text = transform.Find("Text").GetComponent<Text>();
            }
            // Text.font = LanguageManager.Instance.GetFont();
            Text.text = msg;
            _floatTimeChecker = FloatingTime;
            gameObject.SetActive(true);
            Vector3 targetPos = new Vector3(transform.localPosition.x, ShowY + InitY, transform.localPosition.z);
            transform.localPosition = targetPos;
            _isStartSet = true;
            //transform.DOScale(1, 0.3f);
        }
    }
}
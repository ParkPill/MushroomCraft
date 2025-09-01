using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StudioNAP
{
    public class TimerUI : MonoBehaviour
    {
        private Text Lbl;
        private TextMeshProUGUI LblTMP;
        float _timer = 0;
        public string PreText;
        public List<GameObject> AvailableShowList = new List<GameObject>();
        public List<GameObject> HideWithTimerList = new List<GameObject>();
        public bool HideAfterTime = false;
        int _timeLeft = 0;
        public float MaxTime = 10;
        // Start is called before the first frame update
        void Start()
        {
            Lbl = GetComponent<Text>();
            LblTMP = GetComponent<TextMeshProUGUI>();
        }
        public void SetTime(int time)
        {
            _timer = 0;
            MaxTime = time;
            _timeLeft = time;
            UpdateText();
        }


        // Update is called once per frame
        void Update()
        {
            _timer -= Time.deltaTime;
            if (_timer > 0)
            {
                return;
            }
            _timer += 1;

            if (_timeLeft > 0)
            {
                if (HideAfterTime)
                {
                    if (_timeLeft <= 0)
                    {
                        gameObject.SetActive(false);
                    }
                }
                UpdateText();
                _timeLeft--;
            }
        }
        void UpdateText()
        {
            if (Lbl != null)
            {
                Lbl.text = PreText + GetTimeLeftString(_timeLeft);
            }
            if (LblTMP != null)
            {
                LblTMP.text = PreText + GetTimeLeftString(_timeLeft);
            }
        }
        public string GetTimeLeftString(int seconds)
        {
            TimeSpan time = TimeSpan.FromSeconds(seconds);
            if (seconds >= 86400)
            {
                return string.Format("{0}D:{1}h", time.Days, time.Hours);
            }
            else if (seconds >= 3600)
            {
                return string.Format("{0}h:{1}m", time.Hours, time.Minutes);
            }
            else
            {
                return string.Format("{0:00}:{1:00}", seconds / 60, seconds % 60);
            }
        }
    }
}

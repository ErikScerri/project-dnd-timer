// Copyright 2020 Erik Scerri. All Rights Reserved.

using System;
using System.Timers;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Toasted.App
{
    public class TimerUI : MonoBehaviour
    {
        [SerializeField] private EventSystem eventSystem;

        [Header("Buttons")]
        [SerializeField] private Button buttonReset;
        [SerializeField] private Button buttonPlay;
        [SerializeField] private Button buttonPause;

        [Header("Input Fields")]
        [SerializeField] private TMP_InputField inputMinutes;
        [SerializeField] private TMP_InputField inputSeconds;
        [SerializeField] private TMP_InputField inputMilleseconds;
        [SerializeField] private TMP_InputField colonLeft;
        [SerializeField] private TMP_InputField colonRight;

        [Header("Hourglass")]
        [SerializeField] private Image fillTop;
        [SerializeField] private Image fillBot;

        private TimeSpan timeLeft;
        private TimeSpan cachedTimer;

        private Color currentColor;

        private bool isPlaying;
        
        protected void OnEnable()
        {
            buttonReset.onClick.AddListener(OnResetButton);
            buttonPlay.onClick.AddListener(OnPlayButton);
            buttonPause.onClick.AddListener(OnPauseButton);

            inputMinutes.onEndEdit.AddListener(OnMinutesSubmit);
            inputSeconds.onEndEdit.AddListener(OnSecondsSubmit);

            timeLeft = new TimeSpan(0,1,0);
            cachedTimer = timeLeft;

            isPlaying = false;

            UpdateTimer();
        }

        protected void OnDisable()
        {
            buttonReset.onClick.RemoveListener(OnResetButton);
            buttonPlay.onClick.RemoveListener(OnResetButton);
            buttonPause.onClick.RemoveListener(OnResetButton);

            inputMinutes.onEndEdit.RemoveListener(OnMinutesSubmit);
            inputSeconds.onEndEdit.RemoveListener(OnSecondsSubmit);
        }

        protected void Update()
        {
            if (isPlaying)
            {
                timeLeft = timeLeft.Subtract(TimeSpan.FromSeconds(Time.deltaTime));

                if (timeLeft.TotalSeconds < 10)
                {
                    ChangeColor(Color.red);
                }
                
                if (timeLeft.TotalSeconds < 0)
                {
                    timeLeft = TimeSpan.Zero;

                    ChangeColor(Color.green);

                    StopTimer();
                }
                else
                {
                    ChangeColor(Color.white);
                }

                UpdateTimer();
            }
        }

        private void StartTimer()
        {
            Debug.Log("Start");

            isPlaying = true;

            inputMinutes.interactable = false;
            inputSeconds.interactable = false;

            buttonPlay.interactable = false;
            buttonPause.interactable = true;
        }

        private void StopTimer()
        {
            Debug.Log("Stop");

            isPlaying = false;

            inputMinutes.interactable = true;
            inputSeconds.interactable = true;

            buttonPlay.interactable = true;
            buttonPause.interactable = false;

        }

        private void ChangeColor(Color color)
        {
            inputMinutes.textComponent.color = color;
            inputSeconds.textComponent.color = color;
            inputMilleseconds.textComponent.color = color;
            colonLeft.textComponent.color = color;
            colonRight.textComponent.color = color;
        }

        private void UpdateTimer()
        {
            inputMinutes.text = string.Format("{0:00}", timeLeft.Minutes);
            inputSeconds.text = string.Format("{0:00}", timeLeft.Seconds);
            inputMilleseconds.text = string.Format("{0:000}", timeLeft.Milliseconds);

            float fillPercentage = timeLeft == cachedTimer ? 1.0F : Convert.ToSingle(timeLeft.TotalSeconds / cachedTimer.TotalSeconds);

            fillTop.fillAmount = fillPercentage;
            fillBot.fillAmount = 1 - fillPercentage;
        }

        private void OnResetButton()
        {
            eventSystem.SetSelectedGameObject(null);

            ChangeColor(Color.white);
            timeLeft = cachedTimer;
            UpdateTimer();
        }

        private void OnPlayButton()
        {
            if (timeLeft == TimeSpan.Zero) return;

            eventSystem.SetSelectedGameObject(null);

            StartTimer();
        }

        private void OnPauseButton()
        {
            eventSystem.SetSelectedGameObject(null);

            StopTimer();
        }

        private void OnMinutesSubmit(string minStr)
        {
            int mins = int.Parse(minStr);

            mins = Mathf.Clamp(mins, 0, 99);

            inputMinutes.text = string.Format("{0:00}", mins);

            timeLeft = new TimeSpan(0, mins, timeLeft.Seconds);
            cachedTimer = timeLeft;
        }

        private void OnSecondsSubmit(string secStr)
        {
            int secs = int.Parse(secStr);

            secs = Mathf.Clamp(secs, 0, 60);

            inputSeconds.text = string.Format("{0:00}", secs);            

            timeLeft = new TimeSpan(0, timeLeft.Minutes, secs);
            cachedTimer = timeLeft;
        }
    }
}

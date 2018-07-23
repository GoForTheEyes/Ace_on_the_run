using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

namespace game
{


    public class StartGamePanel : MonoBehaviour
    {
        public Action StartCountdownComplete;

#pragma warning disable 0649
        [SerializeField] float TimeToWait;
        [SerializeField] TextMeshProUGUI secondsText;
        [SerializeField] RectTransform secondsTransform;

#pragma warning restore
        float timeSinceGameStarted = 0f;

        private void Awake()
        {
            secondsText.text = TimeToWait.ToString();
        }

        private void Update()
        {
            timeSinceGameStarted += Time.deltaTime;
            if (timeSinceGameStarted > TimeToWait)
            {
                StartCountdownComplete();
                gameObject.SetActive(false);
            }
            var secondsInt = Mathf.CeilToInt(TimeToWait - timeSinceGameStarted);
            secondsText.text = secondsInt.ToString();
            var timeBetweenSeconds = (timeSinceGameStarted % 1);
            if (timeBetweenSeconds > 0.5)
            {
                secondsTransform.localScale = Vector3.one;
            }
            else
            {
                secondsTransform.localScale = new Vector3(timeBetweenSeconds, timeBetweenSeconds, timeBetweenSeconds) *2f;
            }
        }



    }



}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class UIManager : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField] GameObject pausedGamePanel;
        [SerializeField] GameObject optionsPanel;
        [SerializeField] GameObject endGamePanel;
        [SerializeField] Button pauseButton;
        [SerializeField] Button optionsButton;
        [SerializeField] List<Button> mainMenuButtons;
        [SerializeField] List<Button> resumeButtons;
        [SerializeField] List<Button> restartButtons;



#pragma warning restore
        private void Awake()
        {
            InitializeButtons();
            StartSetting();
        }

        void InitializeButtons()
        {
            pauseButton.onClick.RemoveAllListeners();
            pauseButton.onClick.AddListener(() => PauseGame());
            optionsButton.onClick.RemoveAllListeners();
            optionsButton.onClick.AddListener(() => OptionsMenu());
            foreach (var item in resumeButtons)
            {
                item.onClick.RemoveAllListeners();
                item.onClick.AddListener(() => ResumeGame());
            }
            foreach (var item in restartButtons)
            {
                item.onClick.RemoveAllListeners();
                item.onClick.AddListener(() => RestartGame());
            }
            foreach (var item in mainMenuButtons)
            {
                item.onClick.RemoveAllListeners();
                item.onClick.AddListener(() => MainMenu());
            }
        }

        private void MainMenu()
        {
            LoadingManager.Instance.ChangeScene();
        }

        void StartSetting()
        {
            pauseButton.gameObject.SetActive(true);
            pausedGamePanel.SetActive(false);
            optionsPanel.SetActive(false);
            endGamePanel.SetActive(false);
        }

        void PauseGame()
        {
            pauseButton.gameObject.SetActive(false);
            pausedGamePanel.SetActive(true);
            optionsPanel.SetActive(false);
            endGamePanel.SetActive(false);
            GlobalManager.Instance.UpdateState(GlobalState.Paused);
        }

        void ResumeGame()
        {
            pauseButton.gameObject.SetActive(true);
            pausedGamePanel.SetActive(false);
            optionsPanel.SetActive(false);
            endGamePanel.SetActive(false);
            GlobalManager.Instance.UpdateState(GlobalState.Playing);
        }

        void RestartGame()
        {
            LoadingManager.Instance.RestartGameScene();
        }

        void OptionsMenu()
        {
            optionsPanel.SetActive(true);
            pausedGamePanel.SetActive(false);
            endGamePanel.SetActive(false);
        }

        void EndGame()
        {
            optionsPanel.SetActive(false);
            pausedGamePanel.SetActive(false);
            endGamePanel.SetActive(true);
        }

        public void OnPlayerDied()
        {
            Invoke("EndGame",1f);
        }



    }
}


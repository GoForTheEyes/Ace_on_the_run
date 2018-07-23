using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenu
{
    public class UIManager : MonoBehaviour
    {

#pragma warning disable 0649
        [Header("Views")]
        [SerializeField] GameObject MainMenuObjects;
        [SerializeField] GameObject CreditsPanel;
        [SerializeField] GameObject ButtonPanel;
        [SerializeField] GameObject OptionsPanel;
        [Header("Buttons")]
        [SerializeField] List<Button> BackButtons;
        [SerializeField] Button CreditsButton;
        [SerializeField] Button StartGameButton;
        [SerializeField] Button OptionsButton;
        [Header("Canvas")]
        [SerializeField] Canvas myCanvas;
#pragma warning restore

        void Awake()
        {
            Main();
            Initialize();
            SetUpCanvas();
        }

        void SetUpCanvas()
        {
            myCanvas.worldCamera = Camera.main;
        }

        void Initialize()
        {
            foreach (var button in BackButtons)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => Main());
            }
            CreditsButton.onClick.RemoveAllListeners();
            CreditsButton.onClick.AddListener(() => OpenCredits());
            StartGameButton.onClick.RemoveAllListeners();
            StartGameButton.onClick.AddListener(() => StartGame());
            OptionsButton.onClick.RemoveAllListeners();
            OptionsButton.onClick.AddListener(() => OpenOptions());
        }

        void StartGame()
        {
            LoadingManager.Instance.ChangeScene();
        }

        void OpenCredits()
        {
            MainMenuObjects.SetActive(false);
            CreditsPanel.SetActive(true);
            ButtonPanel.SetActive(false);
            OptionsPanel.SetActive(false);
        }

        void OpenOptions()
        {
            MainMenuObjects.SetActive(false);
            CreditsPanel.SetActive(false);
            ButtonPanel.SetActive(false);
            OptionsPanel.SetActive(true);
        }

        void Main()
        {
            MainMenuObjects.SetActive(true);
            CreditsPanel.SetActive(false);
            ButtonPanel.SetActive(true);
            OptionsPanel.SetActive(false);
        }

    }


}


